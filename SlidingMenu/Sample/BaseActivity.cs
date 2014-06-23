using Android.OS;
using Android.Views;
using SlidingMenuSharp;
using SlidingMenuSharp.App;
using ListFragment = Android.Support.V4.App.ListFragment;
using Android.Widget;

namespace Sample
{
    public class BaseActivity : SlidingFragmentActivity
    {
       
        public BaseActivity()
		{ 
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
 
            SetBehindContentView(Resource.Layout.menu_frame);

			SlidingMenu.SetSecondaryMenu(Resource.Layout.menu_frame_two);
			SlidingMenu.SecondaryShadowDrawableRes = Resource.Drawable.shadowright;

            SlidingMenu.ShadowWidthRes = Resource.Dimension.shadow_width;
            SlidingMenu.BehindOffsetRes = Resource.Dimension.slidingmenu_offset;
            SlidingMenu.ShadowDrawableRes = Resource.Drawable.shadow;
            SlidingMenu.FadeDegree = 0.25f;
            SlidingMenu.TouchModeAbove = TouchMode.Fullscreen;

            ActionBar.SetDisplayHomeAsUpEnabled(true);
			 
        }
    }
}