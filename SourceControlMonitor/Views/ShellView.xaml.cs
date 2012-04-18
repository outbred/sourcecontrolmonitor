using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;
using Infrastructure;
using System.Windows;
using System.Linq;
using Infrastructure.Interfaces;

namespace SourceControlMonitor.Views
{
	/// <summary>
	/// Interaction logic for ShellView.xaml
	/// </summary>
	public partial class ShellView
	{
		private IMediatorService _mediator = null;

		public ShellView()
		{
			InitializeComponent();
			Container.Initialize(new List<string>() { "SourceControlMonitor.exe", "Infrastructure.dll", "DataServices.dll" });

			this.DataContext = ViewModelLocator.GetSharedViewModel<IShellViewModel>();
			this.notifyIcon.DataContext = ViewModelLocator.GetSharedViewModel<ITaskBarIconViewModel>();

			_mediator = MediatorLocator.GetSharedMediator();
			_mediator.Subscribe<ShowApplicationEvent>(ignore => OnDoubleClick(null, null));

			_mediator.Subscribe<CommitsPublishedEvent>(item =>
			{
				Application.Current.Dispatcher.BeginInvoke(new Action(() =>
				{
					if(notifyIcon.Visibility != Visibility.Visible)
					{
						return;
					}

					var commits = item as List<ICommitItem>;
					var commit = item as ICommitItem;

					if(commits != null && commits.Count > 1)
					{
						var balloon = new NotifyBalloonView
						{
							BalloonText = string.Format("{0}\n{1} new commits", commits[0].RepositoryName, commits.Count),
							BalloonDetails = string.Join(", ", commits.Select(c => c.Author))
						};

						notifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 4000);
						return;
					}
					else if(commits != null && commits.Count == 1)
					{
						commit = commits[0];
					}

					if(commit != null)
					{

						var balloon = new NotifyBalloonView
						{
							BalloonText = string.Format("{0} - {1}", commit.Author, commit.Date),
							BalloonDetails = commit.LogMessage
						};

						notifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 4000);
					}
				}));
			});

			this.StateChanged += (s, e) =>
			{
				if(this.WindowState == WindowState.Minimized)
				{
					_mediator.NotifyColleaguesAsync<ApplicationHiddenEvent>(null);
					Hide();
					notifyIcon.Visibility = Visibility.Visible;
				}
				else
				{
					notifyIcon.HideBalloonTip();
					notifyIcon.Visibility = Visibility.Collapsed;
				}
			};
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			//clean up notifyicon (would otherwise stay open until application finishes)
			notifyIcon.Dispose();

			base.OnClosing(e);
		}

		private void OnDoubleClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				if(this.WindowState == WindowState.Minimized)
				{
					Show();
					_mediator.NotifyColleaguesAsync<ApplicationRestoredEvent>(null);
					this.WindowState = WindowState.Normal;
				}
			}));
		}
	}
}