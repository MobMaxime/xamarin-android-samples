using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.OS;

namespace LibraryPullZoomView
{
	public class PullToZoomListViewEx : PullToZoomBase<ListView>,AbsListView.IOnScrollListener
	{
		static FrameLayout _headerContainer;
		static int _headerHeight;
		ScalingRunnable _scalingRunnable;

		public PullToZoomListViewEx (Context context) : this (context, null)
		{
		}

		public PullToZoomListViewEx (Context context, IAttributeSet attrs) : base (context, attrs)
		{
			_headerContainer = new FrameLayout(Context);
			if (_zoomView != null) {
				_headerContainer.AddView(_zoomView);
			}
			if (_headerView != null) {
				_headerContainer.AddView(_headerView);
			}

			_rootView.AddHeaderView(_headerContainer);
			_rootView.SetOnScrollListener (this);
			_scalingRunnable = new ScalingRunnable (this);
		}

		public override void SetHideHeader (bool _isHideHeader)
		{
			if (_isHideHeader != ((IPullToZoom<ListView>)this).IsHideHeader ()) {
				base.SetHideHeader (_isHideHeader);
				if (_isHideHeader) {
					RemoveHeaderView ();
				} else {
					UpdateHeaderView ();
				}
			}

		}
		public override void SetHeaderView (View headerView)
		{
			if (headerView != null) {
				this._headerView = headerView;
				UpdateHeaderView ();
			}
		}

		public override void SetZoomView (View zoomView)
		{
			if (zoomView != null) {
				_zoomView = zoomView;
				UpdateHeaderView ();
			}
		}

		/**
     * 移除HeaderView
     * 如果要兼容API 9,需要修改此处逻辑，API 11以下不支持动态添加header
     */
		void RemoveHeaderView ()
		{
			if (_headerContainer != null) {
				_rootView.RemoveHeaderView (_headerContainer);
			}
		}

		/**
     * 更新HeaderView  先移除-->再添加zoomView、HeaderView -->然后添加到listView的head
     * 如果要兼容API 9,需要修改此处逻辑，API 11以下不支持动态添加header
     */
		void UpdateHeaderView ()
		{
			if (_headerContainer != null) {
				_rootView.RemoveHeaderView (_headerContainer);

				_headerContainer.RemoveAllViews ();

				if (_zoomView != null) {
					_headerContainer.AddView (_zoomView);
				}

				if (_headerView != null) {
					_headerContainer.AddView (_headerView);
				}

				_headerHeight = _headerContainer.Height;
				_rootView.AddHeaderView (_headerContainer);
			}
		}

		public void SetAdapter (IListAdapter adapter)
		{
			_rootView.Adapter = adapter;
		}
			
		public void SetOnItemClickListener (AdapterView.IOnItemClickListener listener)
		{
			_rootView.OnItemClickListener = listener;
		}

		/**
     * 创建listView 如果要兼容API9,需要修改此处
     *
     * @param context 上下文
     * @param attrs   AttributeSet
     * @return ListView
     */

		protected override ListView CreateRootView (Context context, IAttributeSet attrs)
		{
			ListView lv = new ListView (context, attrs);
			// Set it to this so it can be used in ListActivity/ListFragment
			lv.Id = Android.Resource.Id.List;
			return lv;
		}

		protected override void SmoothScrollToTop ()
		{
			Log.Debug ("PullToZoomListViewEx", "smoothScrollToTop --> ");
			_scalingRunnable.StartAnimation (200L);
		}

		/**
     * zoomView动画逻辑
     *
     * @param newScrollValue 手指Y轴移动距离值
     */
		protected override void PullHeaderToZoom (double newScrollValue)
		{
			Log.Debug ("PullToZoomListViewEx", "pullHeaderToZoom --> newScrollValue = " + newScrollValue);
			Log.Debug ("PullToZoomListViewEx", "pullHeaderToZoom --> mHeaderHeight = " + _headerHeight);
			if (_scalingRunnable != null && !_scalingRunnable.IsFinished ()) {
				_scalingRunnable.AbortAnimation ();
			}

			ViewGroup.LayoutParams localLayoutParams = _headerContainer.LayoutParameters;
			localLayoutParams.Height = (int)Math.Abs (newScrollValue) + _headerHeight;
			_headerContainer.LayoutParameters = localLayoutParams;
		}

		protected override bool IsReadyForPullStart ()
		{
			return IsFirstItemVisible ();
		}

		bool IsFirstItemVisible ()
		{
			IAdapter adapter = _rootView.Adapter;

			if (null == adapter || adapter.IsEmpty) {
				return true;
			} else {
				/**
             * This check should really just be:
             * mRootView.getFirstVisiblePosition() == 0, but PtRListView
             * internally use a HeaderView which messes the positions up. For
             * now we'll just add one to account for it and rely on the inner
             * condition which checks getTop().
             */
				if (_rootView.FirstVisiblePosition <= 1) {
					View firstVisibleChild = _rootView.GetChildAt (0);
					if (firstVisibleChild != null) {
						return firstVisibleChild.Top >= _rootView.Top;
					}
				}
			}

			return false;
		}
		public override void HandleStyledAttributes (Android.Content.Res.TypedArray a)
		{
			base.HandleStyledAttributes (a);
			_headerContainer = new FrameLayout(Context);
			if (_zoomView != null) {
				_headerContainer.AddView(_zoomView);
			}
			if (_headerView != null) {
				_headerContainer.AddView(_headerView);
			}

			_rootView.AddHeaderView(_headerContainer);
		}
		/**
     * 设置HeaderView高度
     *
     * @param width  宽
     * @param height 高
     */
		public void SetHeaderViewSize (int width, int height)
		{
			if (_headerContainer != null) {
				Object localObject = _headerContainer.LayoutParameters;
				if (localObject == null) {
					localObject = new AbsListView.LayoutParams (width, height);
				}
				((ViewGroup.LayoutParams)localObject).Width = width;
				((ViewGroup.LayoutParams)localObject).Height = height;
				_headerContainer.LayoutParameters = (ViewGroup.LayoutParams)localObject;
				_headerHeight = height;
			}
		}

		public void SetHeaderLayoutParams (AbsListView.LayoutParams layoutParams)
		{
			if (_headerContainer != null) {
				_headerContainer.LayoutParameters = layoutParams;
				_headerHeight = layoutParams.Height;
			}
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			base.OnLayout (changed, l, t, r, b);
			Log.Debug ("PullToZoomListViewEx", "onLayout --> ");
			if (_headerHeight == 0 && _headerContainer != null) {
				_headerHeight = _headerContainer.Height;
			}
		}

		void AbsListView.IOnScrollListener.OnScroll (AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
		{
			if (_zoomView != null && !((IPullToZoom<ListView>)this).IsHideHeader () && ((IPullToZoom<ListView>)this).IsPullToZoomEnabled ()) {
				float f = _headerHeight - _headerContainer.Bottom;
				Log.Debug ("PullToZoomListViewEx", "onScroll --> f = " + f);
				if (((IPullToZoom<ListView>)this).IsParallax ()) {
					if ((f > 0.0F) && (f < _headerHeight)) {
						int i = (int)(0.65D * f);
						_headerContainer.ScrollTo (0, -i);
					} else if (_headerContainer.ScrollY != 0) {
						_headerContainer.ScrollTo (0, 0);
					}
				}
			}
		}

		void AbsListView.IOnScrollListener.OnScrollStateChanged (AbsListView view, ScrollState scrollState)
		{
			Log.Debug ("PullToZoomListViewEx", "onScrollStateChanged --> ");
		}

		public float GetInterpolation (float paramAnonymousFloat)
		{
			float f = paramAnonymousFloat - 1.0F;
			return 1.0F + f * (f * (f * (f * f)));
		}

		class ScalingRunnable : Java.Lang.Object,Java.Lang.IRunnable
		{
			protected long duration;
			protected bool isFinished = true;
			protected float scale;
			protected long startTime;
			PullToZoomListViewEx pullToZoomListViewEx;

			public ScalingRunnable (PullToZoomListViewEx pullToZoomListViewEx)
			{
				this.pullToZoomListViewEx = pullToZoomListViewEx;
			}

			public void AbortAnimation ()
			{
				isFinished = true;
			}

			public bool IsFinished ()
			{
				return isFinished;
			}

			void Java.Lang.IRunnable.Run ()
			{
				if (_zoomView != null) {
					float f2;
					ViewGroup.LayoutParams localLayoutParams;
					if ((!isFinished) && (scale > 1.0D)) {
						float f1 = ((float)SystemClock.CurrentThreadTimeMillis () - (float)startTime) / (float)duration;
						f2 = scale - (scale - 1.0F) * pullToZoomListViewEx.GetInterpolation (f1);
						localLayoutParams = _headerContainer.LayoutParameters;
						Log.Debug ("PullToZoomListViewEx", "ScalingRunnable --> f2 = " + f2);
						if (f2 > 1.0F) {
							localLayoutParams.Height = ((int)(f2 * _headerHeight));
							_headerContainer.LayoutParameters = localLayoutParams;
							pullToZoomListViewEx.Post (this);
							return;
						}
						isFinished = true;
					}
				}
			}

			public void StartAnimation (long paramLong)
			{
				if (_zoomView != null) {
					startTime = SystemClock.CurrentThreadTimeMillis ();
					duration = paramLong;
					scale = ((float)(_headerContainer.Bottom) / _headerHeight);
					isFinished = false;
					pullToZoomListViewEx.Post (this);
				}
			}
		}
	
	}
}

