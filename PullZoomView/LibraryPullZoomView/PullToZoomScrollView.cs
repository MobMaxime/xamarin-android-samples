using System;
using Android.Widget;
using Android.Views;
using Android.Content;
using Android.Util;
using Android.Content.Res;
using Android.App;
using Android.OS;

namespace LibraryPullZoomView
{
	public class PullToZoomScrollView : ScrollView
	{
		View _contentView;//中间View
		View _headView;//头部View
		View _zoomView;//缩放拉伸View

		FrameLayout _contentContainer;
		static FrameLayout _headerContainer;
		static FrameLayout _zoomContainer;
		LinearLayout _rootContainer;

		OnScrollViewChangedListener _onScrollListener;
		OnScrollViewZoomListener _onScrollViewZoomListener;
		ScalingRunnable _scalingRunnable;

		int _screenHeight;
		static int _zoomHeight;
		static int _zoomWidth;

		int _activePointerId = -1;
		float _lastMotionY = -1.0F;
		float _lastScale = -1.0F;
		float _maxScale = -1.0F;
		bool _isHeaderTop = true;
		bool _isEnableZoom = true;
		bool _isParallax = false;

		public PullToZoomScrollView(Context context) :this(context, null){
		}

		public PullToZoomScrollView(Context context, IAttributeSet attrs) :this(context, attrs, 0){
		}

		public PullToZoomScrollView(Context context, IAttributeSet attrs, int defStyle) :base(context, attrs, defStyle){
			Init(attrs);
		}
		void Init(IAttributeSet attrs) {
			_headerContainer = new FrameLayout(Context);
			_zoomContainer = new FrameLayout(Context);
			_contentContainer = new FrameLayout(Context);

			_rootContainer = new LinearLayout(Context);
			_rootContainer.Orientation = Android.Widget.Orientation.Vertical;

			if (attrs != null) {
				LayoutInflater layoutInflater = LayoutInflater.From(Context);
				//初始化状态View
				TypedArray a = Context.ObtainStyledAttributes(attrs, Resource.Styleable.PullToZoomScrollView);

				int zoomViewResId = a.GetResourceId(Resource.Styleable.PullToZoomScrollView_scrollZoomView, 0);
				if (zoomViewResId > 0) {
					_zoomView = layoutInflater.Inflate(zoomViewResId, null, false);
					_zoomContainer.AddView(_zoomView);
					_headerContainer.AddView(_zoomContainer);
				}

				int headViewResId = a.GetResourceId(Resource.Styleable.PullToZoomScrollView_scrollHeadView, 0);
				if (headViewResId > 0) {
					_headView = layoutInflater.Inflate(headViewResId, null, false);
					_headerContainer.AddView(_headView);
				}
				int contentViewResId = a.GetResourceId(Resource.Styleable.PullToZoomScrollView_scrollContentView, 0);
				if (contentViewResId > 0) {
					_contentView = layoutInflater.Inflate(contentViewResId, null, false);
					_contentContainer.AddView(_contentView);
				}

				a.Recycle();
			}

			DisplayMetrics localDisplayMetrics = new DisplayMetrics();
			((Activity) Context).WindowManager.DefaultDisplay.GetMetrics(localDisplayMetrics);
			_screenHeight = localDisplayMetrics.HeightPixels;
			_zoomWidth = localDisplayMetrics.WidthPixels;
			_scalingRunnable = new ScalingRunnable(this);

			_rootContainer.AddView(_headerContainer);
			_rootContainer.AddView(_contentContainer);

			_rootContainer.SetClipChildren(false);
			_headerContainer.SetClipChildren(false);

			AddView(_rootContainer);
		}
		protected override void OnFinishInflate ()
		{
			base.OnFinishInflate ();
		}
		public void SetEnableZoom(bool isEnableZoom) {
			_isEnableZoom = isEnableZoom;
		}

		public void SetParallax(bool isParallax) {
			_isParallax = isParallax;
		}

		public void SetOnScrollListener(OnScrollViewChangedListener mOnScrollListener) {
			_onScrollListener = mOnScrollListener;
		}

		public void SetOnScrollViewZoomListener(OnScrollViewZoomListener onScrollViewZoomListener) {
			_onScrollViewZoomListener = onScrollViewZoomListener;
		}

		public void SetContentContainerView(View view) {
			if (_contentContainer != null) {
				_contentContainer.RemoveAllViews();
				_contentView = view;
				_contentContainer.AddView(view);
			}
		}

		public void SetHeaderContainer(View view) {
			if (_headerContainer != null && view != null) {
				_headerContainer.RemoveAllViews();
				_headView = view;
				if (_zoomView != null && _zoomContainer != null) {
					_zoomContainer.RemoveAllViews();
					_zoomContainer.AddView(_zoomView);
					_headerContainer.AddView(_zoomContainer);
				}
				_headerContainer.AddView(_headView);
			}
		}

		public void SetZoomView(View view) {
			if (_zoomContainer != null && view != null) {
				_zoomView = view;
				_zoomContainer.RemoveAllViews();
				_zoomContainer.AddView(_zoomView);
				if (_headerContainer != null) {
					_headerContainer.RemoveAllViews();
					_headerContainer.AddView(_zoomContainer);
					if (_headView != null) {
						_headerContainer.AddView(_headView);
					}
				}
			}
		}

		public void ShowHeaderView() {
			if (_zoomView != null || _headView != null) {
				_headerContainer.Visibility=ViewStates.Visible;
			}
		}

		public void HideHeaderView() {
			if (_zoomView != null || _headView != null) {
				_headerContainer.Visibility = ViewStates.Gone;
			}
		}

		public FrameLayout GetZoomContainer() {
			return _zoomContainer;
		}

		public FrameLayout GetHeaderContainer() {
			return _headerContainer;
		}

		public View GetZoomView() {
			return _zoomView;
		}

		public View GetContentView() {
			return _contentView;
		}

		public View GetHeadView() {
			return _headView;
		}

		public void SetZoomHeight(int ZoomHeight) {
			_zoomHeight = ZoomHeight;
		}

		public LinearLayout GetRootContainer() {
			return _rootContainer;
		}

		void EndScaling() {
			if (_zoomContainer.Bottom >= _zoomHeight) {
				Log.Debug("PullToZoomScrollView", "endScaling");
			}
			_scalingRunnable.StartAnimation(200L);
		}
		public override bool OnInterceptTouchEvent (MotionEvent ev)
		{
			return base.OnInterceptTouchEvent (ev);
		}
		protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout (changed, left, top, right, bottom);
			if (_zoomHeight == 0) {
				if (_zoomContainer != null) {
					_zoomHeight = _zoomContainer.Height;
					_zoomWidth = _zoomContainer.Width;
				}
			}
		}
		protected override void OnScrollChanged (int l, int t, int oldl, int oldt)
		{
			base.OnScrollChanged (l, t, oldl, oldt);
			if (_isEnableZoom) {
				_isHeaderTop = ScrollY <= 0;

				Log.Debug("PullToZoomScrollView", "onScrollChanged --> ");
				if (_isParallax) {
					float f = _zoomHeight - _zoomContainer.Bottom + ScrollY;
					Log.Debug("PullToZoomScrollView", "f = " + f);
					if ((f > 0.0F) && (f < _zoomHeight)) {
						int i = (int) (0.65D * f);
						_headerContainer.ScrollTo(0, -i);
					} else if (_headerContainer.ScrollY != 0) {
						_headerContainer.ScrollTo(0, 0);
					}
				}
			}
			if (_onScrollListener != null) {
				_onScrollListener.onScrollChanged(l, t, oldl, oldt);
			}
		}
		public override bool OnTouchEvent (MotionEvent ev)
		{
			Log.Debug("PullToZoomScrollView", "onTouchEvent --> action = " + (0xFF & (int)ev.Action));
			if (_isHeaderTop && _isEnableZoom) {
				switch ((MotionEventActions)0xFF & ev.Action) {
				case MotionEventActions.Down:
				case MotionEventActions.Outside:
					if (!_scalingRunnable.IsFinished()) {
						_scalingRunnable.AbortAnimation();
					}
					_lastMotionY = ev.GetY();
					_activePointerId = ev.GetPointerId(0);
					_maxScale = (_screenHeight / _zoomHeight);
					_lastScale = (_zoomContainer.Bottom / _zoomHeight);
					if (_onScrollViewZoomListener != null) {
						_onScrollViewZoomListener.onStart();
					}
					break;
				case MotionEventActions.Move:
					Log.Debug("PullToZoomScrollView", "_activePointerId = " + _activePointerId);
					int j = ev.FindPointerIndex(_activePointerId);
					if (j == -1) {
						Log.Error("PullToZoomScrollView", "Invalid pointerId = " + _activePointerId + " in onTouchEvent");
					} else {
						if (_lastMotionY == -1.0F) {
							_lastMotionY = ev.GetY(j);
						}
						if (_zoomContainer.Bottom >= _zoomHeight) {
							FrameLayout.LayoutParams localLayoutParams = (FrameLayout.LayoutParams) _zoomContainer.LayoutParameters;
							ViewGroup.LayoutParams headLayoutParams = _headerContainer.LayoutParameters;
							float f = ((ev.GetY(j) - _lastMotionY + _zoomContainer.Bottom) / _zoomHeight - _lastScale) / 2.0F + _lastScale;
							if ((_lastScale <= 1.0D) && (f < _lastScale)) {
								localLayoutParams.Height = _zoomHeight;
								localLayoutParams.Width = _zoomWidth;
								localLayoutParams.Gravity = GravityFlags.Center;
								headLayoutParams.Height = _zoomHeight;
								_zoomContainer.LayoutParameters=localLayoutParams;
								_headerContainer.LayoutParameters=headLayoutParams;
								return base.OnTouchEvent(ev);
							}
							_lastScale = Math.Min(Math.Max(f, 1.0F), _maxScale);
							localLayoutParams.Height = ((int) (_zoomHeight * _lastScale));
							localLayoutParams.Width = ((int) (_zoomWidth * _lastScale));
							localLayoutParams.Gravity = GravityFlags.Center;
							headLayoutParams.Height = ((int) (_zoomHeight * _lastScale));
							if (localLayoutParams.Height < _screenHeight) {
								_zoomContainer.LayoutParameters=localLayoutParams;
								_headerContainer.LayoutParameters=headLayoutParams;
							}
							_lastMotionY = ev.GetY(j);
							return true;
						}
						_lastMotionY = ev.GetY(j);
					}
					break;
				case MotionEventActions.Up:
					Reset();
					EndScaling();
					if (_onScrollViewZoomListener != null) {
						_onScrollViewZoomListener.onFinish();
					}
					break;
				case MotionEventActions.Cancel:
					int i = ev.ActionIndex;
					_lastMotionY = ev.GetY(i);
					_activePointerId = ev.GetPointerId(i);
					break;
				case MotionEventActions.PointerDown:
					OnSecondaryPointerUp(ev);
					_lastMotionY = ev.GetY(ev.FindPointerIndex(_activePointerId));
					break;
				}
			}
			return base.OnTouchEvent (ev);
		}
		void OnSecondaryPointerUp(MotionEvent paramMotionEvent) {
			int i = ((int)paramMotionEvent.Action) >> 8;
			if (paramMotionEvent.GetPointerId(i) == _activePointerId)
			if (i != 0) {
				_lastMotionY = paramMotionEvent.GetY(0);
				_activePointerId = paramMotionEvent.GetPointerId(0);
			}
		}

		void Reset() {
			_activePointerId = -1;
			_lastMotionY = -1.0F;
			_maxScale = -1.0F;
			_lastScale = -1.0F;
		}
		public float GetInterpolation(float paramAnonymousFloat) {
			float f = paramAnonymousFloat - 1.0F;
			return 1.0F + f * (f * (f * (f * f)));
		}
		class ScalingRunnable : Java.Lang.Object, Java.Lang.IRunnable
		{
			long duration;
			bool isFinished = true;
			float scale;
			long startTime;
			PullToZoomScrollView pullToZoomScrollView;
			public ScalingRunnable(PullToZoomScrollView pullToZoomScrollView) {
				this.pullToZoomScrollView=pullToZoomScrollView;
			}

			public void AbortAnimation() {
				isFinished = true;
			}

			public bool IsFinished() {
				return isFinished;
			}
			void Java.Lang.IRunnable.Run ()
			{
				float f2;
				FrameLayout.LayoutParams localLayoutParams;
				ViewGroup.LayoutParams headLayoutParams;
				if ((!isFinished) && (scale > 1.0D)) {
					float f1 = ((float) SystemClock.CurrentThreadTimeMillis() - (float) startTime) / (float) duration;
					f2 = scale - (scale - 1.0F) * pullToZoomScrollView.GetInterpolation(f1);

					localLayoutParams = (FrameLayout.LayoutParams) _zoomContainer.LayoutParameters;
					headLayoutParams = _headerContainer.LayoutParameters;
					if (f2 > 1.0F) {
						Log.Debug("PullToZoomScrollView", "f2 > 1.0");
						localLayoutParams.Height = ((int) (f2 * _zoomHeight));
						localLayoutParams.Width = ((int) (f2 * _zoomWidth));
						localLayoutParams.Gravity = GravityFlags.Center;
						_zoomContainer.LayoutParameters=localLayoutParams;
						headLayoutParams.Height = ((int) (f2 * _zoomHeight));
						_headerContainer.LayoutParameters=headLayoutParams;
						pullToZoomScrollView.Post(this);
						return;
					}
					isFinished = true;
				}
			}
			public void StartAnimation(long paramLong) {
				startTime = SystemClock.CurrentThreadTimeMillis();
				duration = paramLong;
				scale = ((float) (_zoomContainer.Bottom) / _zoomHeight);
				isFinished = false;
				pullToZoomScrollView.Post(this);
			}
		}
		public interface OnScrollViewChangedListener {
			void onScrollChanged(int left, int top, int oldLeft, int oldTop);
		}

		public interface OnScrollViewZoomListener {
			void onStart();

			void onFinish();
		}
	}
}

