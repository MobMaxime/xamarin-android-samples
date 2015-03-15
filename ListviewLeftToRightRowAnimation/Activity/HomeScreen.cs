using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Views.Animations;

namespace BasicTableAdapter {
    [Activity(MainLauncher = true)]
	public class HomeScreen : Activity {
        string[] items;
		Button _btn ;
		ListView _lstcategory;
		RelativeLayout _rltBar;
		TextView _txtCategory;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			RequestWindowFeature (WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.Main);
			items = new string[] { "Zero","One","Two","Three","Four","Five"};
			_btn = FindViewById<Button> (Resource.Id.btnCategory);
			 _lstcategory = FindViewById<ListView> (Resource.Id.listcategory);
			_rltBar = FindViewById<RelativeLayout> (Resource.Id.rltBar);
			_txtCategory = FindViewById<TextView> (Resource.Id.txtCategory);
			_lstcategory.Visibility = ViewStates.Gone;
			_btn.Click+= delegate {
				_rltBar.Visibility=ViewStates.Gone;
				_lstcategory.Visibility=ViewStates.Visible;
				_lstcategory.Adapter = new HomeScreenAdapter(this, items);

					LayoutAnimationController controller 
					= AnimationUtils.LoadLayoutAnimation(
						this, Resource.Animation.list_layout_controller);
					_lstcategory.LayoutAnimation=(controller);
			};
			_lstcategory.ItemClick+= OnListItemClick;
        }

        void OnListItemClick (object sender, AdapterView.ItemClickEventArgs e)
        {
			var t = items[e.Position];
			_txtCategory.Text = t;
			_lstcategory.Visibility=ViewStates.Gone;
			_rltBar.Visibility=ViewStates.Visible;
			_lstcategory.Adapter = null;
        }
    }
}

