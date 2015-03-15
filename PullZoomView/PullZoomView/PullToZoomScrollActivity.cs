using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using LibraryPullZoomView;
using Android.Util;

namespace PullZoomView
{
	[Activity (Label = "PullToZoomScrollActivity")]			
	public class PullToZoomScrollActivity : Activity
	{
		PullToZoomScrollViewEx scrollView;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.PullToZoomScrollViewLayout);
			ActionBar.SetDisplayHomeAsUpEnabled(true);

			scrollView = (PullToZoomScrollViewEx) FindViewById(Resource.Id.scrollView);

			DisplayMetrics localDisplayMetrics = new DisplayMetrics();
			WindowManager.DefaultDisplay.GetMetrics(localDisplayMetrics);
			int mScreenHeight = localDisplayMetrics.HeightPixels;
			int mScreenWidth = localDisplayMetrics.WidthPixels;
			LinearLayout.LayoutParams localObject = new LinearLayout.LayoutParams(mScreenWidth, (int) (9.0F * (mScreenWidth / 16.0F)));
			scrollView.SetHeaderLayoutParams(localObject);
		}
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.scroll_view, menu);
			return true;
		}
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Android.Resource.Id.Home) {
				Finish();
				return true;
			}
//			else if (id == Resource.Id.action_settings) {
//			            loadViewForCode();
//			            return true;
//			        }
			else if (id == Resource.Id.action_normal) {
				scrollView.SetParallax(false);
				return true;
			} else if (id == Resource.Id.action_parallax) {
				scrollView.SetParallax(true);
				return true;
			} else if (id == Resource.Id.action_show_head) {
				scrollView.SetHideHeader(false);
				return true;
			} else if (id == Resource.Id.action_hide_head) {
				scrollView.SetHideHeader(true);
				return true;
			} else if (id == Resource.Id.action_disable_zoom) {
				scrollView.SetZoomEnabled(false);
				return true;
			} else if (id == Resource.Id.action_enable_zoom) {
				scrollView.SetZoomEnabled(true);
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}
		void loadViewForCode() {
			PullToZoomScrollView scrollView = (PullToZoomScrollView) FindViewById(Resource.Id.scrollView);
			View headView = LayoutInflater.From(this).Inflate(Resource.Layout.profile_head_view, null, false);
			View zoomView = LayoutInflater.From(this).Inflate(Resource.Layout.profile_zoom_view, null, false);
			View contentView = LayoutInflater.From(this).Inflate(Resource.Layout.profile_content_view, null, false);
			scrollView.SetHeaderContainer(headView);
			scrollView.SetZoomView(zoomView);
			scrollView.SetContentContainerView(contentView);
		}
	}
}

