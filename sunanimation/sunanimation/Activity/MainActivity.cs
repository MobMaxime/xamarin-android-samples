using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Views.Animations;

namespace sunanimation
{
	[Activity (Label = "sunanimation", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			ImageView sun = (ImageView)FindViewById (Resource.Id.sun);
			//Animation sunRise = AnimationUtils.loadAnimation(this, R.anim.sun_rise);
			Animation sunRise = AnimationUtils.LoadAnimation(this, Resource.Animation.sunrise);
			//apply the animation to the View
			sun.StartAnimation(sunRise);

			ImageView clock = (ImageView)FindViewById (Resource.Id.clock);
			Animation clockTurn = AnimationUtils.LoadAnimation (this,Resource.Animation.clockturn);
			clock.StartAnimation (clockTurn);

			ImageView hour = (ImageView)FindViewById (Resource.Id.hour);
			Animation hourTurn = AnimationUtils.LoadAnimation (this,Resource.Animation.hourturn);
			hour.StartAnimation (hourTurn);
		}
	}
}


