using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using PinnedSectionLibrary;

namespace PinnedSectionSample
{
	[Activity (Label = "PinnedSectionList", MainLauncher = false)]
	public class MainActivity : ListActivity, View.IOnClickListener
	{
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

		private Boolean hasHeaderAndFooter;
		private Boolean isFastScroll;
		private Boolean addPadding;
		private Boolean isShadowVisible = true;

		int count = 1;

		public void OnClick(View v) {
			Toast.MakeText(this, "Item: " + v.Tag ,ToastLength.Short).Show();
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			if (bundle != null) {
				isFastScroll = bundle.GetBoolean("isFastScroll");
				addPadding = bundle.GetBoolean("addPadding");
				isShadowVisible = bundle.GetBoolean("isShadowVisible");
				hasHeaderAndFooter = bundle.GetBoolean("hasHeaderAndFooter");
			}
			initializeHeaderAndFooter();
			initializeAdapter();
			initializePadding();
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);

			outState.PutBoolean("isFastScroll", isFastScroll);
			outState.PutBoolean("addPadding", addPadding);
			outState.PutBoolean("isShadowVisible", isShadowVisible);
			outState.PutBoolean("hasHeaderAndFooter", hasHeaderAndFooter);
		}

		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			//			Item item = (Item) ListView.Adapter.GetItem(position);
			//			if (item != null) {
			//				Toast.MakeText(this, "Item " + position + ": " + item.text, ToastLength.Short).Show();
			//			} else {
			//				Toast.MakeText(this, "Item " + position, ToastLength.Short).Show();
			//			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.main, menu);
			menu.GetItem(0).SetChecked(isFastScroll);
			menu.GetItem(1).SetChecked(addPadding);
			menu.GetItem(2).SetChecked(isShadowVisible);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.action_fastscroll:
				isFastScroll = !isFastScroll;
				item.SetChecked(isFastScroll);
				initializeAdapter();
				break;
			case Resource.Id.action_addpadding:
				addPadding = !addPadding;
				item.SetChecked(addPadding);
				initializePadding();
				break;
			case Resource.Id.action_showShadow:
				isShadowVisible = !isShadowVisible;
				item.SetChecked(isShadowVisible);
				((PinnedSectionListView)ListView).setShadowVisible(isShadowVisible);
				break;
			case Resource.Id.action_showHeaderAndFooter:
				hasHeaderAndFooter = !hasHeaderAndFooter;
				item.SetChecked(hasHeaderAndFooter);
				initializeHeaderAndFooter();
				break;
			}
			return true;
		}

		private void initializePadding() {
			float density = Resources.DisplayMetrics.Density;
			int padding = addPadding ? (int) (16 * density) : 0;
			ListView.SetPadding(padding, padding, padding, padding);
		}

		private void initializeHeaderAndFooter() {
			ListAdapter = null;
			if (hasHeaderAndFooter) {
				ListView list = ListView;

				LayoutInflater inflater = LayoutInflater.From(this);
				TextView header1 = (TextView) inflater.Inflate(Android.Resource.Layout.SimpleListItem1, list, false);
				header1.Text = "First header";
				list.AddHeaderView(header1);

				TextView header2 = (TextView) inflater.Inflate(Android.Resource.Layout.SimpleListItem1, list, false);
				header2.Text = "Second header";
				list.AddHeaderView(header2);

				TextView footer = (TextView) inflater.Inflate(Android.Resource.Layout.SimpleListItem1, list, false);
				footer.Text = "Single footer";
				list.AddFooterView(footer);
			}

			initializeAdapter();
		}

		private void initializeAdapter() {
			ListAdapter = new SimpleAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1);

			//			ListView.FastScrollEnabled = isFastScroll;
			//			if (isFastScroll) {
			//				if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Honeycomb) {
			//					ListView.FastScrollAlwaysVisible = true;
			//				}
			//				ListAdapter = new FastScrollAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1);
			//			} else {
			//				ListAdapter = new SimpleAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1);
			//			}	
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
						Item item = new Item(Item.ITEM, section.text.ToUpper());//Locale.ENGLISH) + " - " + j);
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
					view.SetBackgroundColor(parent.Resources.GetColor(COLORS[item.sectionPosition % COLORS.Length]));
				}
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

		//		class FastScrollAdapter : SimpleAdapter, ISectionIndexer {
		//
		//			public Java.Lang.Object[] sections;
		//
		//			public FastScrollAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId) {
		//
		//			}
		//
		//			protected void prepareSections(int sectionsNumber) {
		//				sections = new Item[sectionsNumber];
		//			}
		//
		//		 	protected void onSectionAdded(Item section, int sectionPosition) {
		//				sections[sectionPosition] = section;
		//			}
		//
		//			public Java.Lang.Object[] GetSections() {
		//				return (Java.Lang.Object[])sections;
		//			}
		//
		//			public int GetPositionForSection(int section) {
		//				if (section >= sections.Length) {
		//					section = sections.Length - 1;
		//				}
		//				return ((Item)sections[section]).listPosition;
		//			}
		//
		//			public int GetSectionForPosition(int position) {
		//				if (position >= Count) {
		//					position = Count - 1;
		//				}
		//				return GetItem(position).sectionPosition;
		//			}
		//
		//
		//		}
	}
}


