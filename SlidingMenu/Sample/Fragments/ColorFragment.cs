using Android.OS;
using Android.Views;
using Android.Widget;

namespace Sample.Fragments
{
    public class ColorFragment : Android.Support.V4.App.Fragment
    {
        private int _colorRes = -1;

        public ColorFragment() 
            : this(Resource.Color.white)
        { }

        public ColorFragment(int colorRes)
        {
            _colorRes = colorRes;
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (null != savedInstanceState)
                _colorRes = savedInstanceState.GetInt("_colorRes");
            var color = Resources.GetColor(_colorRes);
            var v = new RelativeLayout(Activity);
            v.SetBackgroundColor(color);
            return v;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt("_colorRes", _colorRes);
        }
    }
}