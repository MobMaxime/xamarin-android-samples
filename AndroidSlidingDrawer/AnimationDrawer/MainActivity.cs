using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace AnimationDrawer
{
	[Activity (Label = "AnimationDrawer", MainLauncher = true)]
	public class MainActivity : Activity
	{
		public Button mCloseButton;
		public Button mOpenButton;
		MultiDirectionSlidingDrawer mDrawer;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			RequestWindowFeature(WindowFeatures.NoTitle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
 
			mCloseButton = (Button) FindViewById( Resource.Id.button_close );
			mOpenButton = (Button) FindViewById( Resource.Id.button_open );
			mDrawer = (MultiDirectionSlidingDrawer) FindViewById( Resource.Id.drawer );

			mCloseButton.Click += (sender, e) => {
				mDrawer.animateClose();
			};

			mOpenButton.Click += (sender, e) => {
				if( !mDrawer.isOpened() )
					mDrawer.animateOpen();
			}; 
		}



//		public override void OnContentChanged ()
//		{
//			base.OnContentChanged ();
//			mCloseButton = (Button) FindViewById( Resource.Id.button_close );
//			mOpenButton = (Button) FindViewById( Resource.Id.button_open );
//			mDrawer = (MultiDirectionSlidingDrawer) FindViewById( Resource.Id.drawer );
//		}

	}
}


