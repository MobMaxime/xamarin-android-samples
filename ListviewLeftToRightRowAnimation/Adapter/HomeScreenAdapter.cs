using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;

namespace BasicTableAdapter
{
	public class HomeScreenAdapter : BaseAdapter<string>
	{
		List<String> items;
		Activity context;

		public HomeScreenAdapter(Activity context, string[] item) : base()
		{
			this.context = context;
			this.items = new  List<string> ();
			this.items = item.ToList ();
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override string this[int position]
		{   
			get { return items[position]; } 
		}

		public override int Count
		{
			get { return items.Count; } 
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView; 
			if (view == null)
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.CustomListLayout, null);
			}
			view.FindViewById<TextView>(Resource.Id.txtList).Text = items[position];
			return view;
		}
	}
}