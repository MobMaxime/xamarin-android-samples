
using Android.App;
using Android.Widget;
using Android.OS;
using Castorflex.SmoothProgressBar;
using Android.Views.Animations;
using Android.Content;


namespace SmoothProgressBar
{
	[Activity (Label = "SmoothProgressBar", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			RequestWindowFeature (Android.Views.WindowFeatures.NoTitle);
			SetContentView(Resource.Layout.Main);

			FindViewById<ProgressBar>(Resource.Id.progressBarLinearInterpolator).IndeterminateDrawable= new SmoothProgressDrawable.Builder(this).Interpolator(new LinearInterpolator()).Build();
			FindViewById<ProgressBar>(Resource.Id.progressBarAccelerateInterpolator).IndeterminateDrawable = new SmoothProgressDrawable.Builder(this).Interpolator(new AccelerateInterpolator()).Build();
			FindViewById<ProgressBar>(Resource.Id.progressBarDecelerateInterpolator).IndeterminateDrawable = new SmoothProgressDrawable.Builder(this).Interpolator(new DecelerateInterpolator()).Build();
			FindViewById<ProgressBar>(Resource.Id.progressBarAccelerateDecelerateInterpolator).IndeterminateDrawable = new SmoothProgressDrawable.Builder(this).Interpolator(new AccelerateDecelerateInterpolator()).Build();

			FindViewById<Button>(Resource.Id.button_make).Click += (s, e) =>
			{
				StartActivity(typeof (MakeCustomActivity));
			};
		}
	}
}


