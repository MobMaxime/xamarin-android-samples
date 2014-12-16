using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Java.Lang;

namespace CarouselViewProjectmaster
{
	[Activity (Label = "CarouselViewProject-master", MainLauncher = true)]
	public class MainActivity : FragmentActivity
	{
		public static int LOOPS = 10; 
		public static int FIRST_PAGE = 9;
		public static float BIG_SCALE = 1.0f;
		public static float SMALL_SCALE = 0.8f;
		public static float DIFF_SCALE = BIG_SCALE - SMALL_SCALE;

		public MyPagerAdapter adapter;
		public ViewPager pager;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);


			pager = FindViewById<ViewPager>(Resource.Id.myviewpager);
			adapter = new MyPagerAdapter(this, this.SupportFragmentManager);
			pager.Adapter = adapter;
			pager.SetOnPageChangeListener (adapter);

			pager.SetCurrentItem (FIRST_PAGE,true);

			pager.OffscreenPageLimit = 3;

			pager.PageMargin = Convert.ToInt32 (GetString(Resource.String.pagermargin));
		}
	}
}


