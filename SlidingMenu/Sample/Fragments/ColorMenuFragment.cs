using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

namespace Sample.Fragments
{
    public class ColorMenuFragment : ListFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup p1, Bundle p2)
        {
            return inflater.Inflate(Resource.Layout.list, null);
        }

        public override void OnActivityCreated(Bundle p0)
        {
            base.OnActivityCreated(p0);

            var colors = Resources.GetStringArray(Resource.Array.color_names);
            var colorAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1,
                                                        Android.Resource.Id.Text1, colors);
            ListAdapter = colorAdapter;
        }

        public override void OnListItemClick(ListView p0, View p1, int position, long p3)
        {
            Fragment newContent = null;
            switch (position)
            {
                case 0:
                    newContent = new ColorFragment(Resource.Color.red);
                    break;
                case 1:
                    newContent = new ColorFragment(Resource.Color.green);
                    break;
                case 2:
                    newContent = new ColorFragment(Resource.Color.blue);
                    break;
                case 3:
                    newContent = new ColorFragment(Resource.Color.white);
                    break;
                case 4:
                    newContent = new ColorFragment(Resource.Color.black);
                    break;
            }

            if (newContent != null)
                SwitchFragment(newContent);
        }

        private void SwitchFragment(Fragment fragment) 
        {
            if (Activity == null)
                return;

            var fca = Activity as FragmentChangeActivity;
            if (fca != null)
                fca.SwitchContent(fragment);

            var ra = Activity as ResponsiveUIActivity;
            if (ra != null) 
            {
                ra.SwitchContent(fragment);
            }
        }
    }
}