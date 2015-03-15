using Android.Views;
using Android.Content.Res;

namespace LibraryPullZoomView
{
	interface IPullToZoom<T> where T : View
	{
		/**
     * Get the Wrapped Zoom View. Anything returned here has already been
     * added to the content view.
     *
     * @return The View which is currently wrapped
     */
		View GetZoomView ();

		View GetHeaderView ();

		/**
     * Get the Wrapped root View.
     *
     * @return The View which is currently wrapped
     */
		T GetPullRootView ();

		/**
     * Whether Pull-to-Refresh is enabled
     *
     * @return enabled
     */
		bool IsPullToZoomEnabled ();

		/**
     * Returns whether the Widget is currently in the Zooming state
     *
     * @return true if the Widget is currently zooming
     */
		bool IsZooming ();

		/**
     * Returns whether the Widget is currently in the Zooming anim type
     *
     * @return true if the anim is parallax
     */
		bool IsParallax ();

		bool IsHideHeader ();

		void HandleStyledAttributes (TypedArray a);
	}
}

