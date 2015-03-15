using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace FlabbyListView
{
	[Activity (Label = "FlabbyListView", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Android.App.ListActivity
	{
		private static int _numListItem = 500;
		private ListAdapter _adapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			List<string> items = GetListItems();
			_adapter = new ListAdapter(this,items);
			ListAdapter = _adapter;
			ListView.SetSelection(items.Count/2);
		}
		private List<string> GetListItems() {
			List<string> list = new List<string>();
			for(int i=0;i<_numListItem;i++){
				list.Add("Item"+i);
			}
			return list;
		}
	}
}


