using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using Infrastructure.Interfaces;
using SourceControlMonitor.Views;

namespace SourceControlMonitor.Helpers
{
	public class BalloonHelper : DependencyObject
	{
		/// <summary>
		/// Description
		/// </summary>
		public static readonly DependencyProperty BalloonContextProperty =
			DependencyProperty.RegisterAttached("BalloonContext",
										typeof(object),
										typeof(BalloonHelper),
										new PropertyMetadata(null, PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var notifyIcon = sender as TaskbarIcon;
			var item = args.NewValue;

			if(notifyIcon == null || item == null)
			{
				return;
			}

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
		}

		/// <summary>
		/// A property wrapper for the <see cref="BalloonTextProperty"/>
		/// dependency property:<br/>
		/// Description
		/// </summary>
		public object BalloonText
		{
			get { return GetValue(BalloonContextProperty); }
			set { SetValue(BalloonContextProperty, value); }
		}

		/// <summary>
		/// Gets the Command property.  
		/// </summary>
		public static object GetBalloonContext(DependencyObject d)
		{
			return d.GetValue(BalloonContextProperty);
		}

		/// <summary>
		/// Sets the Command property. 
		/// </summary>
		public static void SetBalloonContext(DependencyObject d, object value)
		{
			d.SetValue(BalloonContextProperty, value);
		}
	}
}
