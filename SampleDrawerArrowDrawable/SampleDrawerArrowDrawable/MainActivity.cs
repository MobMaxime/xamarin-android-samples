using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.Res;
using Android.Support.V4.Widget;
  
namespace DrawerArrowDrawable
{
	[Activity (Label = "DrawerArrowDrawable", MainLauncher = true)]
	public class MainActivity : Activity,DrawerLayout.IDrawerListener
	{
		public DrawerArrowDrawable drawerArrowDrawable;
		public float offset;
		public Boolean flipped;
		DrawerLayout drawer;
		TextView styleButton;
		ImageView imageView;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			drawer =  FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			imageView =FindViewById<ImageView>(Resource.Id.drawer_indicator);
			styleButton = FindViewById<TextView>(Resource.Id.indicator_style);
			Resources resources = Resources;

			drawerArrowDrawable = new DrawerArrowDrawable(resources);
			drawerArrowDrawable.setStrokeColor(resources.GetColor(Resource.Color.light_gray));
			imageView.SetImageDrawable (drawerArrowDrawable);

			drawer.SetDrawerListener (this);
			imageView.Click += ImageClickHandle ;
			styleButton.Click += TextviewClickHandle;
		}
		public void ImageClickHandle(object sender, EventArgs e) 
		{
			if (drawer.IsDrawerVisible(GravityFlags.Start)) {
				drawer.CloseDrawer(GravityFlags.Start);
			} else {
				drawer.OpenDrawer(GravityFlags.Start);
			}
		}
		public void TextviewClickHandle(object sender, EventArgs e) 
		{
			Boolean rounded = false;
			styleButton.Text = rounded ? Resources.GetString (Resource.String.rounded) //
				: Resources.GetString (Resource.String.squared);

			rounded = !rounded;

			drawerArrowDrawable = new DrawerArrowDrawable(Resources, rounded);
			drawerArrowDrawable.setParameter(offset);
			drawerArrowDrawable.setFlip(flipped);
			drawerArrowDrawable.setStrokeColor(Resources.GetColor(Resource.Color.light_gray));

			imageView.SetImageDrawable(drawerArrowDrawable);
		}

		public void OnDrawerClosed (View drawerView)
		{
			//throw new NotImplementedException ();
		}

		public void OnDrawerOpened (View drawerView)
		{
			//throw new NotImplementedException ();
		}

		public void OnDrawerSlide (View drawerView, float slideOffset)
		{
			offset = slideOffset;

			// Sometimes slideOffset ends up so close to but not quite 1 or 0.
			if (slideOffset >= .995) {
				flipped = true;
				drawerArrowDrawable.setFlip(flipped);
			} else if (slideOffset <= .005) {
				flipped = false;
				drawerArrowDrawable.setFlip(flipped);
			}

			drawerArrowDrawable.setParameter(offset);
			//throw new NotImplementedException ();
		}

		public void OnDrawerStateChanged (int newState)
		{
			//throw new NotImplementedException ();
		}
	}
}


