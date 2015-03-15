using System;
using Android.Widget;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using LibraryFlabbyListView;
using Android.Views;

namespace FlabbyListView
{
	public class ListAdapter : ArrayAdapter<string>
	{
		public Context _context;
		private List<string> _items;
		private Random _randomizer = new Random();

		public ListAdapter(Context context, List<string> items):base(context,0,items) {
			_context = context;
			_items = items;
		}
		public override int Count {
			get {
				return _items.Count;
			}
		}
		public override long GetItemId (int position)
		{
			return position;
		}
		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			ViewHolder Holder;
			if (convertView == null) {
				convertView = LayoutInflater.From(_context).Inflate(Resource.Layout.item_list, parent, false);
				Holder = new ViewHolder();
				Holder.text = convertView.FindViewById<TextView> (Resource.Id.text); 
				convertView.Tag=Holder;
			} else {
				Holder = (ViewHolder) convertView.Tag;
			}
			Color color = Color.Argb(255, _randomizer.Next(256), _randomizer.Next(256), _randomizer.Next(256));
			((FlabbyLayout)convertView).SetFlabbyColor(color);
			Holder.text.Text =_items.ElementAt(position);
			return convertView;
		}
		public class ViewHolder : Java.Lang.Object{
			public TextView text{ get; set; }
		}
	}
}

