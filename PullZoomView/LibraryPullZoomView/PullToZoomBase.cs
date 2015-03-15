using System;
using Android.Widget;
using Android.Views;
using Android.Content;
using Android.Util;
using Android.App;
using Android.Content.Res;

namespace LibraryPullZoomView
{
	public abstract class PullToZoomBase<T>: LinearLayout,IPullToZoom<T> where T : View
	{
		const float _friction = 2.0f;
		protected T _rootView;
		protected View _headerView;
		protected static View _zoomView;
		protected int _screenHeight;
		protected int _screenWidth;

		bool _isZoomEnabled = true;
		bool _isParallax = true;
		bool _isZooming = false;
		bool _isHideHeader = false;

		int _touchSlop;
		bool _isBeingDragged = false;
		float _lastMotionY;
		float _lastMotionX;
		float _initialMotionY;
		float _initialMotionX;
		OnPullZoomListener _onPullZoomListener;

		public PullToZoomBase (Context context) : this (context, null)
		{
		}

		public PullToZoomBase (Context context, IAttributeSet attrs) : base (context, attrs)
		{
			Init (context, attrs);
		}
		void Init (Context context, IAttributeSet attrs)
		{
			SetGravity (GravityFlags.Center);

			ViewConfiguration config = ViewConfiguration.Get (context);
			_touchSlop = config.ScaledTouchSlop;

			DisplayMetrics localDisplayMetrics = new DisplayMetrics ();
			((Activity)Context).WindowManager.DefaultDisplay.GetMetrics (localDisplayMetrics);
			_screenHeight = localDisplayMetrics.HeightPixels;
			_screenWidth = localDisplayMetrics.WidthPixels;

			// Refreshable View
			// By passing the attrs, we can add ListView/GridView params via XML
			_rootView = CreateRootView (context, attrs);

			if (attrs != null) {
				LayoutInflater layoutInflater = LayoutInflater.From (Context);
				TypedArray a = Context.ObtainStyledAttributes (attrs, Resource.Styleable.PullToZoomView);

				int zoomViewResId = a.GetResourceId (Resource.Styleable.PullToZoomView_zoomView, 0);
				if (zoomViewResId > 0) {
					_zoomView = layoutInflater.Inflate (zoomViewResId, null, false);
				}

				int headerViewResId = a.GetResourceId (Resource.Styleable.PullToZoomView_headerView, 0);
				if (headerViewResId > 0) {
					_headerView = layoutInflater.Inflate (headerViewResId, null, false);
				}

				_isParallax = a.GetBoolean (Resource.Styleable.PullToZoomListView_isHeadParallax, true);

				// Let the derivative classes have a go at handling attributes, then
				// recycle them...

				((IPullToZoom<T>)this).HandleStyledAttributes (a);
				a.Recycle ();
			}
			AddView (_rootView, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
		}
		public virtual void HandleStyledAttributes (TypedArray a)
		{
		}
		public void SetOnPullZoomListener (OnPullZoomListener onPullZoomListener)
		{
			_onPullZoomListener = onPullZoomListener;
		}

		#region IPullToZoom implementation

		View IPullToZoom<T>.GetZoomView ()
		{
			return _zoomView;
		}

		View IPullToZoom<T>.GetHeaderView ()
		{
			return _headerView;
		}

		T IPullToZoom<T>.GetPullRootView ()
		{
			return _rootView;
		}

		bool IPullToZoom<T>.IsPullToZoomEnabled ()
		{
			return _isZoomEnabled;
		}

		bool IPullToZoom<T>.IsZooming ()
		{
			return _isZooming;
		}

		bool IPullToZoom<T>.IsParallax ()
		{
			return _isParallax;
		}

		bool IPullToZoom<T>.IsHideHeader ()
		{
			return _isHideHeader;
		}

		void IPullToZoom<T>.HandleStyledAttributes (TypedArray a)
		{
		}

		#endregion

		public void SetZoomEnabled (bool isZoomEnabled)
		{
			_isZoomEnabled = isZoomEnabled;
		}

		public void SetParallax (bool isParallax)
		{
			_isParallax = isParallax;
		}

		public virtual void SetHideHeader (bool isHideHeader)
		{
			_isHideHeader = isHideHeader;
		}

		public override bool OnInterceptTouchEvent (MotionEvent ev)
		{
			if (!((IPullToZoom<T>)this).IsPullToZoomEnabled () || ((IPullToZoom<T>)this).IsHideHeader ()) {
				return false;
			}

			MotionEventActions action = ev.Action;
			if (action == MotionEventActions.Cancel || action == MotionEventActions.Up) {
				_isBeingDragged = false;
				return false;
			}

			if (action != MotionEventActions.Down && _isBeingDragged) {
				return true;
			}
			switch (action) {
			case MotionEventActions.Move:
				{
					if (IsReadyForPullStart ()) {
						float y = ev.GetY (), x = ev.GetX ();
						float diff, oppositeDiff, absDiff;

						// We need to use the correct values, based on scroll
						// direction
						diff = y - _lastMotionY;
						oppositeDiff = x - _lastMotionX;
						absDiff = Math.Abs (diff);

						if (absDiff > _touchSlop && absDiff > Math.Abs (oppositeDiff)) {
							if (diff >= 1f && IsReadyForPullStart ()) {
								_lastMotionY = y;
								_lastMotionX = x;
								_isBeingDragged = true;
							}
						}
					}
					break;
				}
			case MotionEventActions.Down:
				{
					if (IsReadyForPullStart ()) {
						_lastMotionY = _initialMotionY = ev.GetY ();
						_lastMotionX = _initialMotionX = ev.GetX ();
						_isBeingDragged = false;
					}
					break;
				}
			}

			return _isBeingDragged;
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			if (!((IPullToZoom<T>)this).IsPullToZoomEnabled () || ((IPullToZoom<T>)this).IsHideHeader ()) {
				return false;
			}

			if (e.Action == MotionEventActions.Down && e.EdgeFlags != 0) {
				return false;
			}

			switch (e.Action) {
			case MotionEventActions.Move:
				{
					if (_isBeingDragged) {
						_lastMotionY = e.GetY ();
						_lastMotionX = e.GetX ();
						PullEvent ();
						_isZooming = true;
						return true;
					}
					break;
				}

			case MotionEventActions.Down:
				{
					if (IsReadyForPullStart ()) {
						_lastMotionY = _initialMotionY = e.GetY ();
						_lastMotionX = _initialMotionX = e.GetX ();
						return true;
					}
					break;
				}

			case MotionEventActions.Cancel:
			case MotionEventActions.Up:
				{
					if (_isBeingDragged) {
						_isBeingDragged = false;
						// If we're already refreshing, just scroll back to the top
						if (((IPullToZoom<T>)this).IsZooming ()) {
							SmoothScrollToTop ();
							if (_onPullZoomListener != null) {
								_onPullZoomListener.OnPullZoomEnd ();
							}
							_isZooming = false;
							return true;
						}
						return true;
					}
					break;
				}
			}
			return false;
		}

		void PullEvent ()
		{
			double newScrollValue;
			float initialMotionValue, lastMotionValue;

			initialMotionValue = _initialMotionY;
			lastMotionValue = _lastMotionY;

			newScrollValue = Math.Round (Math.Min (initialMotionValue - lastMotionValue, 0) / _friction);

			PullHeaderToZoom (newScrollValue);
			if (_onPullZoomListener != null) {
				_onPullZoomListener.OnPullZooming (newScrollValue);
			}
		}

		protected abstract void PullHeaderToZoom (double newScrollValue);

		public abstract void SetHeaderView (View headerView);

		public abstract void SetZoomView (View zoomView);

		protected abstract T CreateRootView (Context context, IAttributeSet attrs);

		protected abstract void SmoothScrollToTop ();

		protected abstract bool IsReadyForPullStart ();

		public interface OnPullZoomListener
		{
			void OnPullZooming (double newScrollValue);

			void OnPullZoomEnd ();
		}
	}
}

