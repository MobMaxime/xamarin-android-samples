using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace CustomGridView
{
	public class myGVItemAdapter : BaseAdapter<CustomGridView.Info>
	{
		Activity _CurrentContext;
		List<Info> _lstInfo;

		public myGVItemAdapter (Activity currentContext,List<Info> lstFlimInfo)
		{
			_CurrentContext = currentContext;
			_lstInfo = lstFlimInfo;
			
		}
		#region implemented abstract members of BaseAdapter
		public override long GetItemId (int position)
		{
			return position;
		}
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var item = _lstInfo [position];

			if(convertView == null)
				convertView = _CurrentContext.LayoutInflater.Inflate(Resource.Layout.custGridViewItem,null);

			convertView.FindViewById<TextView> (Resource.Id.txtName).Text = item.Name;
			convertView.FindViewById<TextView> (Resource.Id.txtAge).Text = item.Age.ToString();
			convertView.FindViewById<ImageView> (Resource.Id.imgPers).SetImageResource (item.ImageID);

			return convertView;
		}
		public override int Count {
			get {
				return _lstInfo == null ? -1 : _lstInfo.Count;
			}
		}
		#endregion
		#region implemented abstract members of BaseAdapter
		public override Info this [int position] {
			get {
				return _lstInfo == null ? null : _lstInfo[position];
			}
		}
		#endregion
	}
}

