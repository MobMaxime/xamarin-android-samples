
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using LibraryPullZoomView;
using Android.Util;

namespace PullZoomView
{
	[Activity (Label = "PullToZoomListActivity")]			
	public class PullToZoomListActivity :Activity,Android.Widget.AdapterView.IOnItemClickListener
	{
		PullToZoomListViewEx _listView;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.PullToZoomListViewLayout);
			ActionBar.SetDisplayHomeAsUpEnabled(true);

			_listView = (PullToZoomListViewEx) FindViewById(Resource.Id.listView);

			String[] adapterData = new String[]{"Activity", "Service", "Content Provider", "Intent", "BroadcastReceiver", "ADT", "Sqlite3", "HttpClient",
				"DDMS", "Android Studio", "Fragment", "Loader", "Activity", "Service", "Content Provider", "Intent",
				"BroadcastReceiver", "ADT", "Sqlite3", "HttpClient", "Activity", "Service", "Content Provider", "Intent",
				"BroadcastReceiver", "ADT", "Sqlite3", "HttpClient"};

			_listView.SetAdapter(new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, adapterData));
			_listView.SetOnItemClickListener (this);

			DisplayMetrics localDisplayMetrics = new DisplayMetrics();
			WindowManager.DefaultDisplay.GetMetrics(localDisplayMetrics);
			int mScreenHeight = localDisplayMetrics.HeightPixels;
			int mScreenWidth = localDisplayMetrics.WidthPixels;
			AbsListView.LayoutParams localObject = new AbsListView.LayoutParams(mScreenWidth, (int) (9.0F * (mScreenWidth / 16.0F)));
			_listView.SetHeaderLayoutParams(localObject);
		}
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.list_view, menu);
			return true;
		}
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Android.Resource.Id.Home) {
				Finish();
				return true;
			} else if (id == Resource.Id.action_normal) {
				_listView.SetParallax(false);
				return true;
			} else if (id == Resource.Id.action_parallax) {
				_listView.SetParallax(true);
				return true;
			} else if (id == Resource.Id.action_show_head) {
				_listView.SetHideHeader(false);
				return true;
			} else if (id == Resource.Id.action_hide_head) {
				_listView.SetHideHeader(true);
				return true;
			} else if (id == Resource.Id.action_disable_zoom) {
				_listView.SetZoomEnabled(false);
				return true;
			} else if (id == Resource.Id.action_enable_zoom) {
				_listView.SetZoomEnabled(true);
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}
		void AdapterView.IOnItemClickListener.OnItemClick (AdapterView parent, View view, int position, long id)
		{
			Log.Error("zhuwenwu", "position = " + position);
		}
	}
}

