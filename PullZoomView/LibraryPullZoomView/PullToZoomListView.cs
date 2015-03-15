using Android.Widget;
using Android.Views;
using Android.Content;
using Android.Util;
using Android.Content.Res;
using Android.App;
using Java.Lang;
using Android.OS;

namespace LibraryPullZoomView
{
	public class PullToZoomListView : ListView,AbsListView.IOnScrollListener
	{
		static FrameLayout _headerContainer;
		static View _headerView;

		IOnScrollListener _onScrollListener;
		ScalingRunnable _scalingRunnable;

		int _screenHeight;
		int _screenWidth;
		static int _headerHeight;
		int _activePointerId = -1;
		float _lastMotionY = -1.0F;
		float _lastScale = -1.0F;
		float _maxScale = -1.0F;
		bool _isParallax = true;
		bool _isHideHeader = false;
		bool _isEnableZoom = true;

		public PullToZoomListView(Context paramContext) :this(paramContext, null){
		}

		public PullToZoomListView(Context paramContext, IAttributeSet paramAttributeSet) :this(paramContext, paramAttributeSet, 0){
		}

		public PullToZoomListView(Context paramContext, IAttributeSet attrs, int paramInt) :base(paramContext, attrs, paramInt){
			Init(attrs);
		}
		void EndScaling() {
			if (_headerContainer.Bottom >= _headerHeight)
				Log.Debug("PullToZoomListView", "endScaling");
			_scalingRunnable.StartAnimation(200L);
		}

		void Init(IAttributeSet attrs) {
			_headerContainer = new FrameLayout(Context);
			if (attrs != null) {
				LayoutInflater layoutInflater = LayoutInflater.From(Context);
				//初始化状态View
				TypedArray a = Context.ObtainStyledAttributes(attrs, Resource.Styleable.PullToZoomListView);

				int headViewResId = a.GetResourceId(Resource.Styleable.PullToZoomListView_listHeadView, 0);
				if (headViewResId > 0) {
					_headerView = layoutInflater.Inflate(headViewResId, null, false);
					_headerContainer.AddView(_headerView);
					_isHideHeader = false;
				} else {
					_isHideHeader = true;
				}

				_isParallax = a.GetBoolean(Resource.Styleable.PullToZoomListView_isHeadParallax, true);

				a.Recycle();
			}

			DisplayMetrics localDisplayMetrics = new DisplayMetrics();
			((Activity) Context).WindowManager.DefaultDisplay.GetMetrics(localDisplayMetrics);
			_screenHeight = localDisplayMetrics.HeightPixels;
			_screenWidth = localDisplayMetrics.WidthPixels;
			if (_headerView != null) {
				SetHeaderViewSize(_screenWidth, (int) (9.0F * (_screenWidth / 16.0F)));
				AddHeaderView(_headerContainer);
			}
			_scalingRunnable = new ScalingRunnable(this);
			base.SetOnScrollListener(this);
		}
		public void SetParallax(bool isParallax) {
			_isParallax = isParallax;
		}

		public void SetEnableZoom(bool isEnableZoom) {
			_isEnableZoom = isEnableZoom;
		}

		public void SetHeaderView(View headerView) {
			if (_headerView != null) {
				RemoveHeaderView(_headerContainer);
			}
			_headerView = headerView;
			UpdateHeaderView(headerView);
		}

		void UpdateHeaderView(View headerView) {
			if (headerView != null) {
				_headerContainer.RemoveAllViews();
				_headerContainer.AddView(_headerView);
				SetHeaderViewSize(_screenWidth, (int) (9.0F * (_screenWidth / 16.0F)));
				_headerHeight = _headerContainer.Height;
				AddHeaderView(_headerContainer);
			}
		}

		public View GetHeaderView() {
			return _headerView;
		}

		public bool IsHideHeader() {
			return _isHideHeader;
		}

		public void ShowHeadView() {
			if (_headerView != null && _isHideHeader) {
				_isHideHeader = false;
				UpdateHeaderView(_headerView);
			}
		}

		public void HideHeadView() {
			if (_headerView != null && !_isHideHeader) {
				_isHideHeader = true;
				RemoveHeaderView(_headerContainer);
			}
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
		public override bool OnInterceptTouchEvent (MotionEvent ev)
		{
			return base.OnInterceptTouchEvent (ev);
		}
		protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout (changed, left, top, right, bottom);
			if (_headerHeight == 0 && _headerView != null) {
				_headerHeight = _headerContainer.Height;
			}
		}

		void IOnScrollListener.OnScroll (AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
		{
			Log.Debug("PullToZoomListView", "onScroll");
			if (_headerView != null && !_isHideHeader && _isEnableZoom) {
				float f = _headerHeight - _headerContainer.Bottom;
				Log.Debug("PullToZoomListView", "f = " + f);
				if (_isParallax) {
					if ((f > 0.0F) && (f < _headerHeight)) {
						int i = (int) (0.65D * f);
						_headerContainer.ScrollTo(0, -i);
					} else if (_headerContainer.ScrollY != 0) {
						_headerContainer.ScrollTo(0, 0);
					}
				}
			}

			if (_onScrollListener != null) {
				_onScrollListener.OnScroll(view, firstVisibleItem, visibleItemCount, totalItemCount);
			}
		}

		void IOnScrollListener.OnScrollStateChanged (AbsListView view, ScrollState scrollState)
		{
			if (_onScrollListener != null) {
				_onScrollListener.OnScrollStateChanged(view, scrollState);
			}
		}
		public override bool OnTouchEvent (MotionEvent e)
		{
			Log.Debug("PullToZoomListView", "action = " + (0xFF & (int)e.Action));
			if (_headerView != null && !_isHideHeader && _isEnableZoom) {
				switch ((MotionEventActions)0xFF & e.Action) {
				case MotionEventActions.Down:
				case MotionEventActions.Outside:
					if (!_scalingRunnable.IsFinished()) {
						_scalingRunnable.AbortAnimation();
					}
					_lastMotionY = e.GetY();
					_activePointerId = e.GetPointerId(0);
					_maxScale = (_screenHeight / _headerHeight);
					_lastScale = (_headerContainer.Bottom / _headerHeight);
					break;
				case MotionEventActions.Move:
					Log.Debug("PullToZoomListView", "_activePointerId" + _activePointerId);
					int j = e.FindPointerIndex(_activePointerId);
					if (j == -1) {
						Log.Error("PullToZoomListView", "Invalid pointerId=" + _activePointerId + " in onTouchEvent");
					} else {
						if (_lastMotionY == -1.0F) {
							_lastMotionY = e.GetY(j);
						}
						if (_headerContainer.Bottom >= _headerHeight) {
							ViewGroup.LayoutParams localLayoutParams = _headerContainer.LayoutParameters;
							float f = ((e.GetY(j) - _lastMotionY + _headerContainer.Bottom) / _headerHeight - _lastScale) / 2.0F + _lastScale;
							if ((_lastScale <= 1.0D) && (f < _lastScale)) {
								localLayoutParams.Height = _headerHeight;
								_headerContainer.LayoutParameters=localLayoutParams;
								return base.OnTouchEvent(e);
							}
							_lastScale = Java.Lang.Math.Min(Java.Lang.Math.Max(f, 1.0F), _maxScale);
							localLayoutParams.Height = ((int) (_headerHeight * _lastScale));
							if (localLayoutParams.Height < _screenHeight) {
								_headerContainer.LayoutParameters=localLayoutParams;
							}
							_lastMotionY = e.GetY(j);
							return true;
						}
						_lastMotionY = e.GetY(j);
					}
					break;
				case MotionEventActions.Up:
					Reset();
					EndScaling();
					break;
				case MotionEventActions.Cancel:
					int i = e.ActionIndex;
					_lastMotionY = e.GetY(i);
					_activePointerId = e.GetPointerId(i);
					break;
				case MotionEventActions.PointerDown:
					OnSecondaryPointerUp(e);
					_lastMotionY = e.GetY(e.FindPointerIndex(_activePointerId));
					break;
				}
			}
			return base.OnTouchEvent (e);
		}
		public void SetHeaderViewSize(int paramInt1, int paramInt2) {
			if (_headerView != null) {
				Object localObject = _headerContainer.LayoutParameters;
				if (localObject == null) {
					localObject = new LayoutParams(paramInt1, paramInt2);
				}
				((ViewGroup.LayoutParams) localObject).Width = paramInt1;
				((ViewGroup.LayoutParams) localObject).Height = paramInt2;
				_headerContainer.LayoutParameters=(ViewGroup.LayoutParams) localObject;
				_headerHeight = paramInt2;
			}
		}

		public void setOnScrollListener(IOnScrollListener paramOnScrollListener) {
			_onScrollListener = paramOnScrollListener;
		}
		public float GetInterpolation(float paramAnonymousFloat) {
			float f = paramAnonymousFloat - 1.0F;
			return 1.0F + f * (f * (f * (f * f)));
		}
		public class ScalingRunnable :Object, IRunnable
		{
			protected long duration;
			protected bool isFinished = true;
			protected float scale;
			protected long startTime;
			PullToZoomListView p;
			public ScalingRunnable(PullToZoomListView p) {
				this.p=p;
			}

			public void AbortAnimation() {
				isFinished = true;
			}

			public bool IsFinished() {
				return isFinished;
			}
			void IRunnable.Run ()
			{
				if (_headerView != null) {
					float f2;
					ViewGroup.LayoutParams localLayoutParams;
					if ((!isFinished) && (scale > 1.0D)) {
						float f1 = ((float) SystemClock.CurrentThreadTimeMillis() - (float) startTime) / (float) duration;
						f2 = scale - (scale - 1.0F) * p.GetInterpolation(f1);
						localLayoutParams = _headerContainer.LayoutParameters;
						if (f2 > 1.0F) {
							Log.Debug("PullToZoomListView", "f2>1.0");
							localLayoutParams.Height = ((int) (f2 * _headerHeight));
							_headerContainer.LayoutParameters=localLayoutParams;
							p.Post (this);
							return;
						}
						isFinished = true;
					}
				}
			}
			public void StartAnimation(long paramLong) {
				if (_headerView != null) {
					startTime = SystemClock.CurrentThreadTimeMillis();
					duration = paramLong;
					scale = ((float) (_headerContainer.Bottom) / _headerHeight);
					isFinished = false;
					p.Post(this);
				}
			}
		}
	}
}

