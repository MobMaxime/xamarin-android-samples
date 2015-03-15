using System;
using Android.Widget;
using Android.Util;
using Android.Support.V4.View;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;

namespace LibrarySlideDateTimePicker
{
	/**
 * To be used with ViewPager to provide a tab indicator component which give constant feedback as to
 * the user's scroll progress.
 * <p>
 * To use the component, simply add it to your view hierarchy. Then in your
 * {@link android.app.Activity} or {@link android.support.v4.app.Fragment} call
 * {@link #setViewPager(ViewPager)} providing it the ViewPager this layout is being used for.
 * <p>
 * The colors can be customized in two ways. The first and simplest is to provide an array of colors
 * via {@link #setSelectedIndicatorColors(int...)} and {@link #setDividerColors(int...)}. The
 * alternative is via the {@link TabColorizer} interface which provides you complete control over
 * which color is used for any individual position.
 * <p>
 * The views used as tabs can be customized by calling {@link #setCustomTabView(int, int)},
 * providing the layout ID of your custom layout.
 *
 * Modifed by jjobes - Added _tabTitleViews SparseArray and setTabText().
 *                     Also modifed populateTabStrip() to fill _tabTitleViews.
 *
 */
	public class SlidingTabLayout : HorizontalScrollView
	{
		/**
     * Allows complete control over the colors drawn in the tab layout. Set with
     * {@link #setCustomTabColorizer(TabColorizer)}.
     */
		public interface ITabColorizer {

			/**
         * @return return the color of the indicator used when {@code position} is selected.
         */
			int GetIndicatorColor(int Position);

			/**
         * @return return the color of the divider drawn to the right of {@code position}.
         */
			int GetDividerColor(int Position);

		}

		static int _titleOffset;

		int _tabViewLayoutId;
		int _tabViewTextViewId;

		// A map of the TextViews in each tab.
		// Maps page index -> tab TextView
		SparseArray<TextView> _tabTitleViews = new SparseArray<TextView>();

		static ViewPager _viewPager;
		static ViewPager.IOnPageChangeListener _viewPagerPageChangeListener;

		static SlidingTabStrip _tabStrip;

		public SlidingTabLayout(Context context):this(context, null) {

		}

		public SlidingTabLayout(Context context, IAttributeSet Attrs) : this(context, Attrs, 0){
		}

		public SlidingTabLayout(Context context, IAttributeSet Attrs, int DefStyle):base(context, Attrs, DefStyle) {

			// Disable the Scroll Bar
			HorizontalScrollBarEnabled = false;
			// Make sure that the Tab Strips fills this View
			FillViewport=true;

			_titleOffset = (int) (24 * Resources.DisplayMetrics.Density);

			_tabStrip = new SlidingTabStrip(context);
			AddView(_tabStrip, LayoutParams.MatchParent, LayoutParams.WrapContent);
		}

		/**
     * Set the custom {@link TabColorizer} to be used.
     *
     * If you only require simple custmisation then you can use
     * {@link #setSelectedIndicatorColors(int...)} and {@link #setDividerColors(int...)} to achieve
     * similar effects.
     */
		public void SetCustomTabColorizer(ITabColorizer TabColorizer) {
			_tabStrip.SetCustomTabColorizer(TabColorizer);
		}

		/**
     * Sets the colors to be used for indicating the selected tab. These colors are treated as a
     * circular array. Providing one color will mean that all tabs are indicated with the same color.
     */
		public void SetSelectedIndicatorColors(int Colors) {
			_tabStrip.SetSelectedIndicatorColors(Colors);
		}

		/**
     * Sets the colors to be used for tab dividers. These colors are treated as a circular array.
     * Providing one color will mean that all tabs are indicated with the same color.
     */
		public void SetDividerColors(int Colors) {
			_tabStrip.SetDividerColors(Colors);
		}

		/**
     * Set the {@link ViewPager.OnPageChangeListener}. When using {@link SlidingTabLayout} you are
     * required to set any {@link ViewPager.OnPageChangeListener} through this method. This is so
     * that the layout can update it's scroll position correctly.
     *
     * @see ViewPager#setOnPageChangeListener(ViewPager.OnPageChangeListener)
     */
		public void SetOnPageChangeListener(ViewPager.IOnPageChangeListener Listener) {
			_viewPagerPageChangeListener = Listener;
		}

		/**
     * Set the custom layout to be inflated for the tab views.
     *
     * @param layoutResId Layout id to be inflated
     * @param textViewId id of the {@link TextView} in the inflated view
     */
		public void SetCustomTabView(int LayoutResId, int TextViewId) {
			_tabViewLayoutId = LayoutResId;
			_tabViewTextViewId = TextViewId;
		}

		/**
     * Sets the associated view pager. Note that the assumption here is that the pager content
     * (number of tabs and tab titles) does not change after this call has been made.
     */
		public void SetViewPager(ViewPager ViewPager) {
			_tabStrip.RemoveAllViews();

			_viewPager = ViewPager;
			if (ViewPager != null) {
				ViewPager.SetOnPageChangeListener(new InternalViewPagerListener());
				PopulateTabStrip();
			}
		}

		/**
     * Create a default view to be used for tabs. This is called if a custom tab view is not set via
     * {@link #setCustomTabView(int, int)}.
     */
		protected TextView CreateDefaultTabView(Context context) {
			TextView textView = new TextView(context);
			textView.Gravity = Android.Views.GravityFlags.Center;
			textView.SetTextSize(ComplexUnitType.Sp, 12);
			textView.Typeface = Typeface.DefaultBold;

			if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Honeycomb) {
				// If we're running on Honeycomb or newer, then we can use the Theme's
				// selectableItemBackground to ensure that the View has a pressed state
				TypedValue outValue = new TypedValue();
				Context.Theme.ResolveAttribute(Android.Resource.Attribute.SelectableItemBackground,outValue, true);
				textView.SetBackgroundResource(outValue.ResourceId);
			}

			if ((int)Build.VERSION.SdkInt >= 14 || (int)Build.VERSION.SdkInt <= 15){
				// If we're running on ICS or newer, enable all-caps to match the Action Bar tab style
				textView.SetAllCaps(true);
			}

			int padding = (int) (16 * Resources.DisplayMetrics.Density);
			textView.SetPadding(padding, padding, padding, padding);

			return textView;
		}

		void PopulateTabStrip() {
			PagerAdapter adapter = _viewPager.Adapter;
			View.IOnClickListener tabClickListener = new TabClickListener();

			for (int i = 0; i < adapter.Count; i++) {
				View tabView = null;
				TextView tabTitleView = null;

				if (_tabViewLayoutId != 0) {
					// If there is a custom tab view layout id set, try and inflate it
					tabView = LayoutInflater.From(Context).Inflate(_tabViewLayoutId, _tabStrip,
						false);
					tabTitleView = (TextView)tabView.FindViewById(_tabViewTextViewId);
				}

				if (tabView == null) {
					tabView = CreateDefaultTabView(Context);
				}
				if (tabTitleView == null && tabView is TextView ) {
					tabTitleView = (TextView) tabView;
				}

				tabTitleView.Text=adapter.GetPageTitle(i);
				tabView.SetOnClickListener(tabClickListener);

				// Used to get a reference to each tab's TextView in order to
				// update the text in setTabText().
				_tabTitleViews.Put(i, tabTitleView);

				_tabStrip.AddView(tabView);
			}
		}

		/**
     * Set the text on the specified tab's TextView.
     *
     * @param index  the index of the tab whose TextView you want to update
     * @param text  the text to display on the specified tab's TextView
     */
		public void SetTabText(int Index, String Text) {
			TextView tv = (TextView) _tabTitleViews.Get(Index);

			if (tv != null) {
				tv.Text=Text;
			}
		}
		protected override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();
			if (_viewPager != null) {
				ScrollToTab(_viewPager.CurrentItem, 0);
			}
		}
		static void ScrollToTab(int TabIndex, int PositionOffset) {
			int tabStripChildCount = _tabStrip.ChildCount;
			if (tabStripChildCount == 0 || TabIndex < 0 || TabIndex >= tabStripChildCount) {
				return;
			}

			View selectedChild = _tabStrip.GetChildAt(TabIndex);
			if (selectedChild != null) {
				int targetScrollX = selectedChild.Left + PositionOffset;

				if (TabIndex > 0 || PositionOffset > 0) {
					// If we're not at the first child and are mid-scroll, make sure we obey the offset
					targetScrollX -= _titleOffset;
				}

			}
		}
		class InternalViewPagerListener :Java.Lang.Object,ViewPager.IOnPageChangeListener 
		{
			int scrollState;

			void ViewPager.IOnPageChangeListener.OnPageScrollStateChanged (int state)
			{
				scrollState = state;

				if (_viewPagerPageChangeListener != null) {
					_viewPagerPageChangeListener.OnPageScrollStateChanged(state);
				}
			}
			void ViewPager.IOnPageChangeListener.OnPageScrolled (int position, float positionOffset, int positionOffsetPixels)
			{
				int tabStripChildCount = _tabStrip.ChildCount;
				if ((tabStripChildCount == 0) || (position < 0) || (position >= tabStripChildCount)) {
					return;
				}

				_tabStrip.OnViewPagerPageChanged(position, positionOffset);

				View selectedTitle = _tabStrip.GetChildAt(position);
				int extraOffset = (selectedTitle != null)
					? (int) (positionOffset * selectedTitle.Width)
					: 0;
				ScrollToTab(position, extraOffset);

				if (_viewPagerPageChangeListener != null) {
					_viewPagerPageChangeListener.OnPageScrolled(position, positionOffset,
						positionOffsetPixels);
				}
			}
			void ViewPager.IOnPageChangeListener.OnPageSelected (int position)
			{
				if (scrollState == ViewPager.ScrollStateIdle) {
					_tabStrip.OnViewPagerPageChanged(position, 0f);
					ScrollToTab(position, 0);
				}

				if (_viewPagerPageChangeListener != null) {
					_viewPagerPageChangeListener.OnPageSelected(position);
				}
			}
		}
		class TabClickListener :Java.Lang.Object, View.IOnClickListener {
			void IOnClickListener.OnClick (View v)
			{
				for (int i = 0; i < _tabStrip.ChildCount; i++) {
					if (v == _tabStrip.GetChildAt(i)) {
						_viewPager.CurrentItem=i;
						return;
					}
				}
			}
		}
	}
}

