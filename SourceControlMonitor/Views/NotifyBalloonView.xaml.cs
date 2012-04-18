using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;

namespace SourceControlMonitor.Views
{
	/// <summary>
	/// Interaction logic for NotifyBalloonView.xaml
	/// </summary>
	public partial class NotifyBalloonView
	{
		private bool _isClosing = false;

		#region Balloon dependency properties

		/// <summary>
		/// Description
		/// </summary>
		public static readonly DependencyProperty BalloonTextProperty =
			DependencyProperty.Register("BalloonText",
										typeof(string),
										typeof(NotifyBalloonView),
										new FrameworkPropertyMetadata(""));

		/// <summary>
		/// A property wrapper for the <see cref="BalloonTextProperty"/>
		/// dependency property:<br/>
		/// Description
		/// </summary>
		public string BalloonText
		{
			get { return (string)GetValue(BalloonTextProperty); }
			set { SetValue(BalloonTextProperty, value); }
		}

		/// <summary>
		/// Description
		/// </summary>
		public static readonly DependencyProperty BalloonDetailsProperty =
			DependencyProperty.Register("BalloonDetails",
										typeof(string),
										typeof(NotifyBalloonView),
										new FrameworkPropertyMetadata(""));

		/// <summary>
		/// A property wrapper for the <see cref="BalloonTextProperty"/>
		/// dependency property:<br/>
		/// Description
		/// </summary>
		public string BalloonDetails
		{
			get { return (string)GetValue(BalloonDetailsProperty); }
			set { SetValue(BalloonDetailsProperty, value); }
		}

		#endregion

		public NotifyBalloonView()
		{
			InitializeComponent();
			TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
			this.DataContext = ViewModelLocator.GetSharedViewModel<INotifyBalloonViewModel>();
		}


		/// <summary>
		/// By subscribing to the <see cref="TaskbarIcon.BalloonClosingEvent"/>
		/// and setting the "Handled" property to true, we suppress the popup
		/// from being closed in order to display the fade-out animation.
		/// </summary>
		private void OnBalloonClosing(object sender, RoutedEventArgs e)
		{
			this.DataContext = null;
			e.Handled = true;
			_isClosing = true;
		}


		/// <summary>
		/// Resolves the <see cref="TaskbarIcon"/> that displayed
		/// the balloon and requests a close action.
		/// </summary>
		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			//the tray icon assigned this attached property to simplify access
			var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
			if(taskbarIcon != null)
			{
				taskbarIcon.CloseBalloon();
			}
		}

		/// <summary>
		/// If the users hovers over the balloon, we don't close it.
		/// </summary>
		private void OnMouseEnter(object sender, MouseEventArgs e)
		{
			//if we're already running the fade-out animation, do not interrupt anymore
			//(makes things too complicated for the sample)
			if(_isClosing)
			{
				return;
			}

			//the tray icon assigned this attached property to simplify access
			var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
			if(taskbarIcon != null)
			{
				taskbarIcon.ResetBalloonCloseTimer();
			}
		}


		/// <summary>
		/// Closes the popup once the fade-out animation completed.
		/// The animation was triggered in XAML through the attached
		/// BalloonClosing event.
		/// </summary>
		private void OnFadeOutCompleted(object sender, EventArgs e)
		{
			var pp = Parent as Popup;
			if(pp != null)
			{
				pp.IsOpen = false;
			}
		}
	}
}
