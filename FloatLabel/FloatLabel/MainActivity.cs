using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;

namespace FloatLabel
{
	[Activity (Label = "FloatLabel", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : ActionBarActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			if (bundle == null) {
				SupportFragmentManager.BeginTransaction().Add(Resource.Id.container,new PlaceholderFragment()).Commit();
			}
		}
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			// Inflate the menu; this adds items to the action bar if it is present.
			MenuInflater.Inflate(Resource.Menu.demo, menu);
			return true;	
		}
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			// Handle action bar item clicks here. The action bar will
			// automatically handle clicks on the Home/Up button, so long
			// as you specify a parent activity in AndroidManifest.xml.
			int Id = item.ItemId;
			if (Id == Resource.Id.action_settings) {
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}
		/**
     * A placeholder fragment containing a simple view.
     */
		public class PlaceholderFragment : Android.Support.V4.App.Fragment {

			public PlaceholderFragment() {
			}
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				View RootView = inflater.Inflate(Resource.Layout.fragmentdemo, container, false);
				return RootView;
			}
		}
	}
}