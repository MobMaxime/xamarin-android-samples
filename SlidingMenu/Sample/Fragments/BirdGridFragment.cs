using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Object = Java.Lang.Object;

namespace Sample.Fragments
{
    public class BirdGridFragment : Android.Support.V4.App.Fragment
    {
        private int _pos = -1;
        private int _imgRes;

        public BirdGridFragment() { }
        public BirdGridFragment(int pos)
        {
            _pos = pos;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (_pos == -1 && savedInstanceState != null)
                _pos = savedInstanceState.GetInt("_pos");
            var imgs = Resources.ObtainTypedArray(Resource.Array.birds_img);
            _imgRes = imgs.GetResourceId(_pos, -1);

            var gv = (GridView) inflater.Inflate(Resource.Layout.list_grid, null);
            gv.SetBackgroundResource(Android.Resource.Color.Black);
            gv.Adapter = new GridAdapter(Activity, _imgRes);
            gv.ItemClick += (sender, args) =>
                {
                    if (Activity == null)
                        return;
                    ((ResponsiveUIActivity)Activity).OnBirdPressed(_pos);
                };

            return gv;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt("_pos", _pos);
        }

        private class GridAdapter : BaseAdapter
        {
            private readonly Activity _activity;
            private readonly int _imgRes;
            public GridAdapter(Activity activity, int imgRes)
            {
                _activity = activity;
                _imgRes = imgRes;
            }

            public override Object GetItem(int position)
            {
                return null;
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                if (convertView == null)
                    convertView = _activity.LayoutInflater.Inflate(Resource.Layout.grid_item, null);

                var img = convertView.FindViewById<ImageView>(Resource.Id.grid_item_img);
                img.SetImageResource(_imgRes);
                return convertView;
            }

            public override int Count
            {
                get { return 30; }
            }
        }
    }
}