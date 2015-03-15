using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Animation;
using Android.Graphics;
using Android.Util;

namespace HoloCircularProgressBar
{
	[Activity (Label = "HoloCircularProgressBar", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity,CompoundButton.IOnCheckedChangeListener,Animator.IAnimatorListener,ValueAnimator.IAnimatorUpdateListener
	{
		protected bool _animationHasEnded = false;

		ToggleButton togAnimation;

		/**
     * The Switch button.
     */
		Button btnRandomColor,btnZero,btnOne;

		LibraryHoloCircularProgressBar.HoloCircularProgressBar holoCircularProgressBar;

		ObjectAnimator _progressBarAnimator;

		LibraryHoloCircularProgressBar.HoloCircularProgressBar progressBar1;
		float progress1;
		/*
     * (non-Javadoc)
     *
     * @see android.app.Activity#onCreate(android.os.Bundle)
     */
		protected override void OnCreate (Bundle bundle)
		{
			if (Intent != null) {
				Bundle extras = Intent.Extras;
				if (extras != null) {
					int theme = extras.GetInt ("theme");
					if (theme != 0) {
						SetTheme (theme);
					}
				}
			}
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			holoCircularProgressBar = FindViewById<LibraryHoloCircularProgressBar.HoloCircularProgressBar> (Resource.Id.holoCircularProgressBar);

			btnRandomColor = FindViewById<Button> (Resource.Id.btnRandomColor);
			btnRandomColor.Click += (object sender, EventArgs e) => {
				SwitchColor ();
			};
			btnZero = FindViewById<Button> (Resource.Id.btnZero);
			btnZero.Click += (object sender, EventArgs e) => {
				if (_progressBarAnimator != null) {
					_progressBarAnimator.Cancel ();
				}
				Animate (holoCircularProgressBar, null, 0f, 1000);
				holoCircularProgressBar.SetMarkerProgress (0f);
			};
			btnOne = FindViewById<Button> (Resource.Id.btnOne);
			btnOne.Click += (object sender, EventArgs e) => {
				if (_progressBarAnimator != null) {
					_progressBarAnimator.Cancel ();
				}
				Animate (holoCircularProgressBar, null, 1f, 1000);
				holoCircularProgressBar.SetMarkerProgress (1f);
			};
			togAnimation = FindViewById<ToggleButton> (Resource.Id.togAnimation);
			togAnimation.SetOnCheckedChangeListener (this);
		}

		void CompoundButton.IOnCheckedChangeListener.OnCheckedChanged (CompoundButton buttonView, bool isChecked)
		{
			if (isChecked) {

				btnOne.Enabled=false;
				btnZero.Enabled=false;
				Animate (holoCircularProgressBar,this);
			}
			else {
				_animationHasEnded = true;
				_progressBarAnimator.Cancel();

				btnOne.Enabled=true;
				btnZero.Enabled=true;
			}
		}
		/**
     * generates random colors for the ProgressBar
     */
		protected void SwitchColor() {
			Random r = new Random();
			int randomColor = Color.Rgb(r.Next(256), r.Next(256), r.Next(256));
			holoCircularProgressBar.SetProgressColor(randomColor);

			randomColor = Color.Rgb(r.Next(256), r.Next(256), r.Next(256));
			holoCircularProgressBar.SetProgressBackgroundColor(randomColor);
		}

		/*
     * (non-Javadoc)
     *
     * @see android.app.Activity#onCreateOptionsMenu(android.view.Menu)
     */
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.circular_progress_bar_sample, menu);
			return true;
		}
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_switch_theme:
				SwitchTheme();
				break;

			default:
				Log.Wtf("CircularProgressBarSample", "couldn't map a click action for " + item);
				break;
			}
			return base.OnOptionsItemSelected (item);
		}
		/**
     * Switch theme.
     */
		public void SwitchTheme() {

			Intent intent = Intent;
			Bundle extras = Intent.Extras;
			if (extras != null) {
				int theme = extras.GetInt("theme", -1);
				if (theme == Resource.Style.AppThemeLight) {
					Intent.RemoveExtra("theme");
				} else {
					intent.PutExtra("theme", Resource.Style.AppThemeLight);
				}
			} else {
				intent.PutExtra("theme", Resource.Style.AppThemeLight);
			}
			Finish();
			StartActivity(intent);
		}
		/**
     * Animate.
     *
     * @param progressBar the progress bar
     * @param listener    the listener
     */
		void Animate(LibraryHoloCircularProgressBar.HoloCircularProgressBar progressBar,Android.Animation.Animator.IAnimatorListener listener) {
			float progress = (float) (Java.Lang.Math.Random() * 2);
			int duration = 3000;
			Animate(progressBar, listener, progress, duration);
		}

		void Animate(LibraryHoloCircularProgressBar.HoloCircularProgressBar progressBar, Android.Animation.Animator.IAnimatorListener listener,float progress, int duration) {

			_progressBarAnimator = ObjectAnimator.OfFloat(progressBar, "progress", progress);
			_progressBarAnimator.SetDuration(duration);
			progressBar1 = progressBar;
			progress1 = progress;
			_progressBarAnimator.AddListener (this);
			if (listener != null) {
				_progressBarAnimator.AddListener(listener);
			}
			_progressBarAnimator.Reverse();
			_progressBarAnimator.AddUpdateListener (this);

			progressBar.SetMarkerProgress(progress);
			_progressBarAnimator.Start();
		}

		void Animator.IAnimatorListener.OnAnimationCancel (Animator animation)
		{
			//throw new NotImplementedException ();
			animation.End();
		}

		void Animator.IAnimatorListener.OnAnimationEnd (Animator animation)
		{
			progressBar1.SetProgress(progress1);

			if (!_animationHasEnded) {
				Animate(holoCircularProgressBar, this);
			} else {
				_animationHasEnded = false;
			}
		}

		void Animator.IAnimatorListener.OnAnimationRepeat (Animator animation)
		{
			//throw new NotImplementedException ();
		}

		void Animator.IAnimatorListener.OnAnimationStart (Animator animation)
		{
			//throw new NotImplementedException ();
		}

		void ValueAnimator.IAnimatorUpdateListener.OnAnimationUpdate (ValueAnimator animation)
		{
			progressBar1.SetProgress((float) animation.AnimatedValue);
		}
	}
}


