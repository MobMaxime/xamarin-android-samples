using Android.App;
using Android.OS;

namespace PullZoomView
{
	[Activity (Label = "PullZoomView", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);
			FindViewById (Resource.Id.btnList).Click += (object sender, System.EventArgs e) => {
				StartActivity(typeof(PullToZoomListActivity));
			};
		
			FindViewById (Resource.Id.btnScroll).Click += (object sender, System.EventArgs e) => {
				StartActivity(typeof(PullToZoomScrollActivity));
			};
		}
		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.main, menu);
			return true;
		}
		public override bool OnOptionsItemSelected (Android.Views.IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Resource.Id.action_settings) {
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}
	}
}


