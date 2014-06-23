using Android.App;
using Android.OS;
using SlidingMenuSharp;
using SlidingMenuSharp.App;

namespace Sample
{
	[Activity(MainLauncher = true,Label = "Left and Right")]
	public class LeftAndRightActivity : BaseActivity
    {
		public LeftAndRightActivity() : base()
        { }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

			SlidingMenu.Mode = MenuMode.LeftRight;
            SlidingMenu.TouchModeAbove = TouchMode.Fullscreen;
            
		    SetContentView(Resource.Layout.content_frame);

	
		
        }
    }
}