using Android.Support.V4.App;

namespace StickyListHeaders
{
    public class MainPagerAdapter : FragmentPagerAdapter 
    {
        public MainPagerAdapter(FragmentManager fm) : base(fm)
        {
            
        }



        public override Fragment GetItem(int p0)
        {
            return new TestFragment();
        }

        public override int Count
        {
            get { return 1; }
        }
    }
}