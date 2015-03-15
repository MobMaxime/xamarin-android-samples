using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;


namespace CustomGridView
{
	[Activity (Label = "GridViews", MainLauncher = true)]
	public class GridViewsHeaderFooterView : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			List<Info> lstInfo = getInformation ();

			CustomGridView.GridViewWithHeaderAndFooter gvObject = FindViewById<CustomGridView.GridViewWithHeaderAndFooter> (Resource.Id.gvCtrl);
			LayoutInflater layoutInflater = LayoutInflater.From(this);
			View headerView = layoutInflater.Inflate(Resource.Layout.Header,null );
			View footerView =  layoutInflater.Inflate(Resource.Layout.Footer, null);
			gvObject.AddHeaderView(headerView);
			gvObject.AddFooterView(footerView);

			gvObject.Adapter = new myGVItemAdapter (this, lstInfo);
			gvObject.ItemClick += OnGridView_ItemClicked;

		}

		void OnGridView_ItemClicked (object sender, AdapterView.ItemClickEventArgs e)
		{
			if (e.Position != 0 && e.Position != 26) {
				string selectedName = e.View.FindViewById<TextView> (Resource.Id.txtName).Text;
				Toast.MakeText (this, "You Click on name " + selectedName, ToastLength.Long).Show ();
			} else if (e.Position == 0) {
				Toast.MakeText (this, "You Click Header", ToastLength.Long).Show ();
			} else if (e.Position == 26) {
				Toast.MakeText (this, "You Click Footer", ToastLength.Long).Show ();
			}
		}

		List<Info> getInformation ()
		{
			List<Info> listItem = new List<Info> () {
				new Info(){	Name ="ABC", Age=47,ImageID= Resource.Drawable.img1},
				new Info(){	Name ="PQR", Age=47,ImageID= Resource.Drawable.img2},
				new Info(){	Name ="XYZ", Age=47,ImageID= Resource.Drawable.img3},
				new Info(){	Name ="ABC", Age=47,ImageID= Resource.Drawable.img4},
				new Info(){	Name ="PQR", Age=47,ImageID= Resource.Drawable.img5},
				new Info(){	Name ="XYZ", Age=47,ImageID= Resource.Drawable.img6},
				new Info(){	Name ="ABC", Age=47,ImageID= Resource.Drawable.img1},
				new Info(){	Name ="PQR", Age=47,ImageID= Resource.Drawable.img2},
				new Info(){	Name ="XYZ", Age=47,ImageID= Resource.Drawable.img3},
				new Info(){	Name ="ABC", Age=47,ImageID= Resource.Drawable.img4},
				new Info(){	Name ="PQR", Age=47,ImageID= Resource.Drawable.img5},
				new Info(){	Name ="XYZ", Age=47,ImageID= Resource.Drawable.img6},
				new Info(){	Name ="ABC", Age=47,ImageID= Resource.Drawable.img1},
				new Info(){	Name ="PQR", Age=47,ImageID= Resource.Drawable.img2},
				new Info(){	Name ="XYZ", Age=47,ImageID= Resource.Drawable.img3},
				new Info(){	Name ="ABC", Age=47,ImageID= Resource.Drawable.img4},
				new Info(){	Name ="PQR", Age=47,ImageID= Resource.Drawable.img5},
				new Info(){	Name ="XYZ", Age=47,ImageID= Resource.Drawable.img6},
				new Info(){	Name ="ABC", Age=47,ImageID= Resource.Drawable.img1},
				new Info(){	Name ="PQR", Age=47,ImageID= Resource.Drawable.img2},
				new Info(){	Name ="XYZ", Age=47,ImageID= Resource.Drawable.img3},
				new Info(){	Name ="ABC", Age=47,ImageID= Resource.Drawable.img4},
				new Info(){	Name ="PQR", Age=47,ImageID= Resource.Drawable.img5},
				new Info(){	Name ="XYZ", Age=47,ImageID= Resource.Drawable.img6}
			};

			return listItem;

		}
	}
}


