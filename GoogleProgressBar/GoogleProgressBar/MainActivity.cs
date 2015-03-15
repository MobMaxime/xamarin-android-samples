using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Preferences;
using LibraryGoogleProgressBar;

namespace GoogleProgressBar
{
	[Activity (Label = "GoogleProgressBar", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		ProgressBar _progressBar;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			RequestWindowFeature (WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.Main);

			_progressBar = FindViewById<ProgressBar> (Resource.Id.google_progress);

			Rect bounds = _progressBar.IndeterminateDrawable.Bounds;
			_progressBar.IndeterminateDrawable= new FoldingCirclesDrawable.Builder(this).Colors(GetProgressDrawableColors()).Build();
			_progressBar.IndeterminateDrawable.Bounds=bounds;
		}
		private int[] GetProgressDrawableColors() {
			int[] Colors = new int[4];
			Android.Content.ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
			Colors[0] = prefs.GetInt(GetString(Resource.String.firstcolor_pref_key),Resources.GetColor(Resource.Color.red));
			Colors[1] = prefs.GetInt(GetString(Resource.String.secondcolor_pref_key),Resources.GetColor(Resource.Color.blue));
			Colors[2] = prefs.GetInt(GetString(Resource.String.thirdcolor_pref_key),Resources.GetColor(Resource.Color.yellow));
			Colors[3] = prefs.GetInt(GetString(Resource.String.fourthcolor_pref_key), Resources.GetColor(Resource.Color.green));
			return Colors;
		}
	}
}


