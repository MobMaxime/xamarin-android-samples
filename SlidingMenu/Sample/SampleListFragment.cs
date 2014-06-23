using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

namespace Sample
{
    public class SampleListFragment : ListFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup p1, Bundle p2)
        {
            return inflater.Inflate(Resource.Layout.list, null);
        }

        public override void OnActivityCreated(Bundle p0)
        {
            base.OnActivityCreated(p0);
            var adapter = new SampleAdapter(Activity);
            for (var i = 0; i<20; i++)
				adapter.Add(new SampleItem { IconRes = Android.Resource.Drawable.IcMenuSearch, Tag = "Sample Demo List" });
            ListAdapter = adapter;
        }

        private class SampleItem
        {
            public string Tag { get; set; }
            public int IconRes { get; set; }
        }

        private class SampleAdapter : ArrayAdapter<SampleItem>
        {
            public SampleAdapter(Context context)
                : base(context, 0)
            {}

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                if (null == convertView)
                    convertView = LayoutInflater.From(Context).Inflate(Resource.Layout.row, null);

//                var icon = convertView.FindViewById<ImageView>(Resource.Id.row_icon);
//                icon.SetImageResource(GetItem(position).IconRes);
                var title = convertView.FindViewById<TextView>(Resource.Id.row_title);
                title.Text = GetItem(position).Tag;

                return convertView;
            }
        }
    }
}