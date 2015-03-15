
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using com.refractored.components.stickylistheaders;
using com.refractored.components.stickylistheaders.Interfaces;

namespace StickyListHeaders
{
    public class TestFragment : Fragment, AdapterView.IOnItemClickListener, IOnHeaderListClickListener
    {
        private TestBaseAdapter m_Adapter;
        private StickyListHeadersListView m_StickyList;

        public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
        {
            var v = p0.Inflate(Resource.Layout.fragment_test, p1, false);
            m_StickyList = v.FindViewById<StickyListHeadersListView>(Resource.Id.list);
            m_StickyList.OnItemClickListener = this;
            m_StickyList.OnHeaderListClickListener = this;
            m_StickyList.AddHeaderView(p0.Inflate(Resource.Layout.list_header, null));
            m_StickyList.AddFooterView(p0.Inflate(Resource.Layout.list_footer, null));

            m_Adapter = new TestBaseAdapter(Activity);
            m_StickyList.EmptyView = v.FindViewById(Resource.Id.empty);
            m_StickyList.Adapter = m_Adapter;
            return v;
        }



        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            Toast.MakeText(Activity, "Item " + position + " clicked", ToastLength.Short).Show();
        }

		public void OnHeaderClick(StickyListHeadersListView listView, View header, int itemPosition, string headerId, bool currentlySticky)
        {
            Toast.MakeText(Activity, "header " + headerId, ToastLength.Short).Show();
//#if __ANDROID_11__
//
//            if ((int) Build.VERSION.SdkInt >= 11) //HC
//            {
//                m_StickyList.SmoothScrollToPositionFromTop(m_Adapter.GetSectionStart(itemPosition) +
//                                                           m_StickyList.HeaderViewsCount, -m_StickyList.PaddingTop);
//            }
//#endif
        }
    }
}