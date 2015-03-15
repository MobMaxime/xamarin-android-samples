using System;
using Android.Widget;
using Android.Views;
using Android.Content;
using Android.Util;
using Android.OS;

namespace LibraryPullZoomView
{
	public class PullToZoomScrollViewEx : PullToZoomBase<ScrollView>,OnScrollViewChangedListener
	{
		static bool _isCusto_headerHeight = false;
		//自定义header高度之后可能导致zoomView拉伸不正确
		static FrameLayout _headerContainer;
		LinearLayout _rootContainer;
		View _contentView;
		static int _headerHeight;
		ScalingRunnable _scalingRunnable;

		public PullToZoomScrollViewEx (Context context) : this (context, null)
		{
		}

		public PullToZoomScrollViewEx (Context context, IAttributeSet attrs) : base (context, attrs)
		{
			Android.Content.Res.TypedArray a = Context.ObtainStyledAttributes (attrs, Resource.Styleable.PullToZoomView);
			_rootContainer = new LinearLayout (Context);
			_rootContainer.Orientation = Orientation.Vertical;
			_headerContainer = new FrameLayout (Context);

			if (_zoomView != null) {
				_headerContainer.AddView (_zoomView);
			}
			if (_headerView != null) {
				_headerContainer.AddView (_headerView);
			}
			int contentViewResId = a.GetResourceId (Resource.Styleable.PullToZoomScrollView_scrollContentView, 0);
			if (contentViewResId > 0) {
				LayoutInflater mLayoutInflater = LayoutInflater.From (Context);
				_contentView = mLayoutInflater.Inflate (contentViewResId, null, false);
			}

			_rootContainer.AddView (_headerContainer);
			if (_contentView != null) {
				_rootContainer.AddView (_contentView);
			}

			_rootContainer.SetClipChildren (false);
			_headerContainer.SetClipChildren (false);

			_rootView.AddView (_rootContainer);

			_scalingRunnable = new ScalingRunnable (this);
			((InternalScrollView)_rootView).SetOnScrollViewChangedListener (this);
		}

		#region OnScrollViewChangedListener implementation

		void OnScrollViewChangedListener.OnInternalScrollChanged (int left, int top, int oldLeft, int oldTop)
		{
//			if (((IPullToZoom<ListView>)this).isPullToZoomEnabled () && ((IPullToZoom<ListView>)this).isParallax ()) {
			Log.Debug ("PullToZoomScrollViewEx", "onScrollChanged --> getScrollY() = " + _rootView.ScrollY);
			float f = _headerHeight - _headerContainer.Bottom + _rootView.ScrollY;
				Log.Debug ("PullToZoomScrollViewEx", "onScrollChanged --> f = " + f);
				if ((f > 0.0F) && (f < _headerHeight)) {
					int i = (int)(0.65D * f);
					_headerContainer.ScrollTo (0, -i);
				} else if (_headerContainer.ScrollY != 0) {
					_headerContainer.ScrollTo (0, 0);
				}
//			}
		}

		#endregion


		protected override void PullHeaderToZoom (double newScrollValue)
		{
			Log.Debug ("PullToZoomScrollViewEx", "pullHeaderToZoom --> newScrollValue = " + newScrollValue);
			Log.Debug ("PullToZoomScrollViewEx", "pullHeaderToZoom --> _headerHeight = " + _headerHeight);
			if (_scalingRunnable != null && !_scalingRunnable.IsFinished ()) {
				_scalingRunnable.AbortAnimation ();
			}

			ViewGroup.LayoutParams localLayoutParams = _headerContainer.LayoutParameters;
			localLayoutParams.Height = (int)Math.Abs (newScrollValue) + _headerHeight;
			_headerContainer.LayoutParameters = localLayoutParams;

			if (_isCusto_headerHeight) {
				ViewGroup.LayoutParams zoomLayoutParams = _zoomView.LayoutParameters;
				zoomLayoutParams.Height = (int)Math.Abs (newScrollValue) + _headerHeight;
				_zoomView.LayoutParameters = zoomLayoutParams;
			}
		}

		/**
     * 是否显示headerView
     *
     * @param isHideHeader true: show false: hide
     */
		public override void SetHideHeader (bool _isHideHeader)
		{
//			if (_isHideHeader != ((IPullToZoom<ListView>)this).IsHideHeader () && _headerContainer != null) {
				base.SetHideHeader (_isHideHeader);

				if (_isHideHeader) {
					_headerContainer.Visibility = ViewStates.Gone;
				} else {
					_headerContainer.Visibility = ViewStates.Visible;
				}
//			}
		}

		public override void SetHeaderView (View headerView)
		{
			if (_headerContainer != null && headerView != null) {
				_headerContainer.RemoveAllViews ();
				_headerView = headerView;
				if (_zoomView != null) {
					_headerContainer.AddView (_zoomView);
				}
				_headerContainer.AddView (_headerView);
			}
		}

		public override void SetZoomView (View zoomView)
		{
			if (_headerContainer != null && zoomView != null) {
				_headerContainer.RemoveAllViews ();
				_headerContainer.AddView (_zoomView);
				if (_headerView != null) {
					_headerContainer.AddView (_headerView);
				}
			}
		}

		protected override ScrollView CreateRootView (Context context, IAttributeSet attrs)
		{
			ScrollView scrollView = new InternalScrollView (context, attrs);
			scrollView.Id = Resource.Id.scrollview;
			return scrollView;
		}

		protected override void SmoothScrollToTop ()
		{
			Log.Debug ("PullToZoomScrollViewEx", "smoothScrollToTop --> ");
			_scalingRunnable.StartAnimation (200L);
		}

		protected override bool IsReadyForPullStart ()
		{
			return _rootView.ScrollY == 0;
		}

		public override void HandleStyledAttributes (Android.Content.Res.TypedArray a)
		{
			base.HandleStyledAttributes (a);
			_rootContainer = new LinearLayout (Context);
			_rootContainer.Orientation = Orientation.Vertical;
			_headerContainer = new FrameLayout (Context);

			if (_zoomView != null) {
				_headerContainer.AddView (_zoomView);
			}
			if (_headerView != null) {
				_headerContainer.AddView (_headerView);
			}
			int contentViewResId = a.GetResourceId (Resource.Styleable.PullToZoomScrollView_scrollContentView, 0);
			if (contentViewResId > 0) {
				LayoutInflater mLayoutInflater = LayoutInflater.From (Context);
				_contentView = mLayoutInflater.Inflate (contentViewResId, null, false);
			}

			_rootContainer.AddView (_headerContainer);
			if (_contentView != null) {
				_rootContainer.AddView (_contentView);
			}

			_rootContainer.SetClipChildren (false);
			_headerContainer.SetClipChildren (false);

			_rootView.AddView (_rootContainer);
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
					localObject = new ViewGroup.LayoutParams (width, height);
				}
				((ViewGroup.LayoutParams)localObject).Width = width;
				((ViewGroup.LayoutParams)localObject).Height = height;
				_headerContainer.LayoutParameters = (ViewGroup.LayoutParams)localObject;
				_headerHeight = height;
				_isCusto_headerHeight = true;
			}
		}

		/**
     * 设置HeaderView LayoutParams
     *
     * @param layoutParams LayoutParams
     */
		public void SetHeaderLayoutParams (LinearLayout.LayoutParams layoutParams)
		{
			if (_headerContainer != null) {
				_headerContainer.LayoutParameters = layoutParams;
				_headerHeight = layoutParams.Height;
				_isCusto_headerHeight = true;
			}
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			base.OnLayout (changed, l, t, r, b);
			Log.Debug ("PullToZoomScrollViewEx", "onLayout --> ");
			if (_headerHeight == 0 && _zoomView != null) {
				_headerHeight = _headerContainer.Height;
			}
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
			PullToZoomScrollViewEx pullToZoomScrollViewEx;

			public ScalingRunnable (PullToZoomScrollViewEx pullToZoomScrollViewEx)
			{
				this.pullToZoomScrollViewEx = pullToZoomScrollViewEx;
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
						f2 = scale - (scale - 1.0F) * pullToZoomScrollViewEx.GetInterpolation (f1);

						localLayoutParams = _headerContainer.LayoutParameters;
						Log.Debug ("PullToZoomScrollViewEx", "ScalingRunnable --> f2 = " + f2);
						if (f2 > 1.0F) {
							localLayoutParams.Height = ((int)(f2 * _headerHeight));
							_headerContainer.LayoutParameters = localLayoutParams;
							if (_isCusto_headerHeight) {
								ViewGroup.LayoutParams zoomLayoutParams;
								zoomLayoutParams = _zoomView.LayoutParameters;
								zoomLayoutParams.Height = ((int)(f2 * _headerHeight));
								_zoomView.LayoutParameters = zoomLayoutParams;
							}
							pullToZoomScrollViewEx.Post (this);
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
					pullToZoomScrollViewEx.Post (this);
				}
			}
		}

		protected class InternalScrollView : ScrollView
		{
			OnScrollViewChangedListener onScrollViewChangedListener;

			public InternalScrollView (Context context) : this (context, null)
			{
			}

			public InternalScrollView (Context context, IAttributeSet attrs) : base (context, attrs)
			{
			}

			public void SetOnScrollViewChangedListener (OnScrollViewChangedListener onScrollViewChangedListener)
			{
				this.onScrollViewChangedListener = onScrollViewChangedListener;
			}

			protected override void OnScrollChanged (int l, int t, int oldl, int oldt)
			{
				base.OnScrollChanged (l, t, oldl, oldt);
				if (onScrollViewChangedListener != null) {
					onScrollViewChangedListener.OnInternalScrollChanged (l, t, oldl, oldt);
				}
			}
		}

	}

	public interface OnScrollViewChangedListener
	{
		void OnInternalScrollChanged (int left, int top, int oldLeft, int oldTop);
	}
}

