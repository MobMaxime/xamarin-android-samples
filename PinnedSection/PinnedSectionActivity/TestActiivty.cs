
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace PinnedSectionActivity
{
	[Activity (Label = "TestActiivty", MainLauncher = true)]			
	public class TestActiivty : Activity, View.IOnClickListener
	{  
		private Boolean hasHeaderAndFooter;
		private Boolean isFastScroll;
		private Boolean addPadding;
		private Boolean isShadowVisible = true;

		public PinnedSectionListView listview;

		int count = 1;

		public void OnClick(View v) {
			Toast.MakeText(this, "Item: " + v.Tag ,ToastLength.Short).Show();
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			listview = FindViewById<PinnedSectionListView> (Resource.Id.list);

			//listview.Adapter = new SimpleAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1);
			if (bundle != null) {
				isFastScroll = bundle.GetBoolean("isFastScroll");
				addPadding = bundle.GetBoolean("addPadding");
				isShadowVisible = bundle.GetBoolean("isShadowVisible");
				hasHeaderAndFooter = bundle.GetBoolean("hasHeaderAndFooter");
			}
			initializeHeaderAndFooter();
			initializeAdapter();
			initializePadding();

			// Create your application here
		} 
		private void initializePadding() {
			float density = Resources.DisplayMetrics.Density;
			int padding = addPadding ? (int) (16 * density) : 0;
			listview.SetPadding(padding, padding, padding, padding);
		}

		private void initializeHeaderAndFooter() {
			if (hasHeaderAndFooter) {

				LayoutInflater inflater = LayoutInflater.From(this);
				TextView header1 = (TextView) inflater.Inflate(Android.Resource.Layout.SimpleListItem1, listview, false);
				header1.Text = "First header";
				listview.AddHeaderView(header1);

				TextView header2 = (TextView) inflater.Inflate(Android.Resource.Layout.SimpleListItem1, listview, false);
				header2.Text = "Second header";
				listview.AddHeaderView(header2);

				TextView footer = (TextView) inflater.Inflate(Android.Resource.Layout.SimpleListItem1, listview, false);
				footer.Text = "Single footer";
				listview.AddFooterView(footer);
			}

			initializeAdapter();
		}

		private void initializeAdapter() {
			listview.Adapter = new SimpleAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1);

			//			ListView.FastScrollEnabled = isFastScroll;
			//				if (isFastScroll) {
			//				if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Honeycomb) {
			//					ListView.FastScrollAlwaysVisible = true;
			//				}
			//				ListAdapter = new FastScrollAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1);
			//			} else {
			//				ListAdapter = new SimpleAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1);
			//			}
		}

		class Item {

			public static int ITEM = 0;
			public static int SECTION = 1;

			public int type;
			public String text;

			public int sectionPosition;
			public int listPosition;

			public Item(int type, String text) {
				this.type = type;
				this.text = text;
			}

			public String toString() {
				return text;
			}

		}

	class SimpleAdapter : ArrayAdapter<Item>, PinnedSectionListView.PinnedSectionListAdapter {

		private static int[] COLORS = new int[] {
			Resource.Color.green_light, Resource.Color.orange_light,
			Resource.Color.blue_light, Resource.Color.red_light };

		public SimpleAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId) {


			int sectionsNumber = 'Z' - 'A' + 1;
			prepareSections(sectionsNumber);

			int sectionPosition = 0, listPosition = 0;

			for (int i=0; i<sectionsNumber; i++) {
				Item section = new Item(Item.SECTION,((char)('A' + i)).ToString());
				section.sectionPosition = sectionPosition;
				section.listPosition = listPosition++;
				onSectionAdded(section, sectionPosition);
				Add(section);

				int itemsNumber = (int) Math.Abs((Math.Cos(2f*Math.PI/3f * sectionsNumber / (i+1f)) * 25f));
				for (int j=0;j<itemsNumber;j++) {
						Item item = new Item(Item.ITEM, section.text.ToUpper() + " - " + j);
					item.sectionPosition = sectionPosition;
					item.listPosition = listPosition++;
					Add(item);
				}

				sectionPosition++;
			}
		}

		protected void prepareSections(int sectionsNumber) { }
		protected void onSectionAdded(Item section, int sectionPosition) { }

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			TextView view = (TextView) base.GetView(position, convertView, parent);
			view.SetTextColor(Color.DarkGray);
			view.Tag = ("" + position);
			Item item = GetItem(position);
				if (item.type == Item.SECTION) {
					//view.setOnClickListener(PinnedSectionListActivity.this);
					view.SetBackgroundColor (parent.Resources.GetColor (COLORS [item.sectionPosition % COLORS.Length]));
				}

				view.Text = item.text;
			return view;
		}

		public override int ViewTypeCount {
			get {
				return 2;
			}
		}

		public override int GetItemViewType (int position)
		{

			return GetItem (position).type;
		}

		public Boolean isItemViewTypePinned(int viewType) {
			return viewType == Item.SECTION;
		}

	}
}
}

