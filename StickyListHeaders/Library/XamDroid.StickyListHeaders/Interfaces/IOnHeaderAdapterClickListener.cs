using Android.Views;

namespace com.refractored.components.stickylistheaders.Interfaces
{
    public interface IOnHeaderAdapterClickListener
    {
		void OnHeaderClick(View header, int itemPosition, string headerId);
    }
}