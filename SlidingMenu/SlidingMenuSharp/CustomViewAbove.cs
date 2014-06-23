using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace SlidingMenuSharp
{
    public delegate void PageSelectedEventHandler(object sender, PageSelectedEventArgs e);
    public delegate void PageScrolledEventHandler(object sender, PageScrolledEventArgs e);
    public delegate void PageScrollStateChangedEventHandler(object sender, PageScrollStateChangedEventArgs e);

    public class CustomViewAbove : ViewGroup
    {
        private new const string Tag = "CustomViewAbove";

        private const bool UseCache = false;

        private const int MaxSettleDuration = 600; // ms
        private const int MinDistanceForFling = 25; // dips

        private readonly IInterpolator _interpolator = new CVAInterpolator();

        private class CVAInterpolator : Java.Lang.Object, IInterpolator
        {
            public float GetInterpolation(float t)
            {
                t -= 1.0f;
                return t * t * t * t * t + 1.0f;
            }
        }

        private View _content;

	    private int _curItem;
	    private Scroller _scroller;

	    private bool _scrollingCacheEnabled;

	    private bool _scrolling;

	    private bool _isBeingDragged;
	    private bool _isUnableToDrag;
	    private int _touchSlop;
	    private float _initialMotionX;
	    /**
	     * Position of the last motion event.
	     */
	    private float _lastMotionX;
	    private float _lastMotionY;
	    /**
	     * ID of the active pointer. This is used to retain consistency during
	     * drags/flings if multiple pointers are used.
	     */
	    protected int ActivePointerId = InvalidPointer;
	    /**
	     * Sentinel value for no current active pointer.
	     * Used by {@link #ActivePointerId}.
	     */
	    private const int InvalidPointer = -1;

	    /**
	     * Determines speed during touch scrolling
	     */
	    protected VelocityTracker VelocityTracker;
	    private int _minimumVelocity;
	    protected int MaximumVelocity;
	    private int _flingDistance;

	    private CustomViewBehind _viewBehind;
	    //	private int mMode;
	    private bool _enabled = true;

        private readonly IList<View> _ignoredViews = new List<View>();

        private bool _quickReturn;
        private float _scrollX;

        public PageSelectedEventHandler PageSelected;
        public PageScrolledEventHandler PageScrolled;
        public PageScrollStateChangedEventHandler PageScrollState;
        public EventHandler Closed;
        public EventHandler Opened;


        public CustomViewAbove(Context context) : this(context, null)
        {
        }

        public CustomViewAbove(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitCustomViewAbove();
        }

        void InitCustomViewAbove()
        {
            TouchMode = TouchMode.Margin;
            SetWillNotDraw(false);
            DescendantFocusability = DescendantFocusability.AfterDescendants;
            Focusable = true;
            _scroller = new Scroller(Context, _interpolator);
            var configuration = ViewConfiguration.Get(Context);
            _touchSlop = ViewConfigurationCompat.GetScaledPagingTouchSlop(configuration);
            _minimumVelocity = configuration.ScaledMinimumFlingVelocity;
            MaximumVelocity = configuration.ScaledMaximumFlingVelocity;

            var density = Context.Resources.DisplayMetrics.Density;
            _flingDistance = (int) (MinDistanceForFling*density);

            PageSelected += (sender, args) =>
                {
                    if (_viewBehind == null) return;
                    switch (args.Position)
                    {
                        case 0:
                        case 2:
                            _viewBehind.ChildrenEnabled = true;
                            break;
                        case 1:
                            _viewBehind.ChildrenEnabled = false;
                            break;
                    }
                };
        }

        public void SetCurrentItem(int item)
        {
            SetCurrentItemInternal(item, true, false);
        }

        public void SetCurrentItem(int item, bool smoothScroll)
        {
            SetCurrentItemInternal(item, smoothScroll, false);
        }

        public int GetCurrentItem()
        {
            return _curItem;
        }

        void SetCurrentItemInternal(int item, bool smoothScroll, bool always, int velocity = 0)
        {
            if (!always && _curItem == item) {
                ScrollingCacheEnabled = false;
                return;
            }

            item = _viewBehind.GetMenuPage(item);

            var dispatchSelected = _curItem != item;
            _curItem = item;
            var destX = GetDestScrollX(_curItem);
            if (dispatchSelected && PageSelected != null)
            {
                PageSelected(this, new PageSelectedEventArgs { Position = item });
            }
            if (smoothScroll) {
                SmoothScrollTo(destX, 0, velocity);
            } else {
                CompleteScroll();
                ScrollTo(destX, 0);
            }
        }

        public void AddIgnoredView(View v)
        {
            if (!_ignoredViews.Contains(v))
                _ignoredViews.Add(v);
        }

        public void RemoveIgnoredView(View v)
        {
            _ignoredViews.Remove(v);
        }

        public void ClearIgnoredViews()
        {
            _ignoredViews.Clear();
        }

        static float DistanceInfluenceForSnapDuration(float f)
        {
            f -= 0.5f;
            f *= 0.3f*(float)Math.PI/2.0f;
            return FloatMath.Sin(f);
        }

        public int GetDestScrollX(int page)
        {
            switch (page)
            {
                case 0:
                case 2:
                    return _viewBehind.GetMenuLeft(_content, page);
                case 1:
                    return _content.Left;
            }
            return 0;
        }

        private int LeftBound
        {
            get { return _viewBehind.GetAbsLeftBound(_content); }
        }

        private int RightBound
        {
            get { return _viewBehind.GetAbsRightBound(_content); }
        }

        public int ContentLeft
        {
            get { return _content.Left + _content.PaddingLeft; }
        }

        public bool IsMenuOpen
        {
            get { return _curItem == 0 || _curItem == 2; }
        }

        private bool IsInIgnoredView(MotionEvent ev)
        {
            var rect = new Rect();
            foreach (var v in _ignoredViews)
            {
                v.GetHitRect(rect);
                if (rect.Contains((int) ev.GetX(), (int) ev.GetY())) 
                    return true;
            }
            return false;
        }

        public int BehindWidth
        {
            get {
                return _viewBehind == null ? 0 : _viewBehind.BehindWidth;
            }
        }

        public int GetChildWidth(int i)
        {
            switch (i)
            {
                case 0:
                    return BehindWidth;
                case 1:
                    return _content.Width;
                default:
                    return 0;
            }
        }

        public bool IsSlidingEnabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        void SmoothScrollTo(int x, int y, int velocity = 0)
        {
            if (ChildCount == 0)
            {
                ScrollingCacheEnabled = false;
                return;
            }

            var sx = ScrollX;
            var sy = ScrollY;
            var dx = x - sx;
            var dy = y - sy;
            if (dx == 0 && dy == 0)
            {
                CompleteScroll();
                if (IsMenuOpen)
                {
                    if (null != Opened)
                        Opened(this, EventArgs.Empty);
                }
                else
                {
                    if (null != Closed)
                        Closed(this, EventArgs.Empty);
                }
                return;
            }

            ScrollingCacheEnabled = true;
            _scrolling = true;

            var width = BehindWidth;
            var halfWidth = width / 2;
            var distanceRatio = Math.Min(1f, 1.0f * Math.Abs(dx) / width);
            var distance = halfWidth + halfWidth * DistanceInfluenceForSnapDuration(distanceRatio);
            int duration;
            velocity = Math.Abs(velocity);
            if (velocity > 0)
                duration = (int)(4 * Math.Round(1000 * Math.Abs(distance / velocity)));
            else
            {
                var pageDelta = (float) Math.Abs(dx) / width;
                duration = (int) ((pageDelta + 1) * 100);
            }
            duration = Math.Min(duration, MaxSettleDuration);

            _scroller.StartScroll(sx, sy, dx, dy, duration);
            Invalidate();
        }

        public View Content
        {
            get { return _content; }
            set
            {
                if (_content != null)
                    RemoveView(_content);
                _content = value;
                AddView(_content);
            }
        }

        public CustomViewBehind CustomViewBehind
        {
            set { _viewBehind = value; }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var width = GetDefaultSize(0, widthMeasureSpec);
            var height = GetDefaultSize(0, heightMeasureSpec);
            SetMeasuredDimension(width, height);

            var contentWidth = GetChildMeasureSpec(widthMeasureSpec, 0, width);
            var contentHeight = GetChildMeasureSpec(heightMeasureSpec, 0, height);
            _content.Measure(contentWidth, contentHeight);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            if (w == oldw) return;
            CompleteScroll();
            ScrollTo(GetDestScrollX(_curItem), ScrollY);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var width = r - l;
            var height = b - t;
            _content.Layout(0, 0, width, height);
        }

        public int AboveOffset
        {
            set
            {
                _content.SetPadding(value, _content.PaddingTop, _content.PaddingRight, _content.PaddingBottom);
            }
        }

        public override void ComputeScroll()
        {
            if (!_scroller.IsFinished)
            {
                if (_scroller.ComputeScrollOffset())
                {
                    var oldX = ScrollX;
                    var oldY = ScrollY;
                    var x = _scroller.CurrX;
                    var y = _scroller.CurrY;

                    if (oldX != x ||oldY != y)
                    {
                        ScrollTo(x, y);
                        OnPageScrolled(x);
                    }

                    Invalidate();
                    return;
                }
            }

            CompleteScroll();
        }

        protected void OnPageScrolled(int xpos)
        {
            var widthWithMargin = Width;
            var position = xpos / widthWithMargin;
            var offsetPixels = xpos % widthWithMargin;
            var offset = (float)offsetPixels / widthWithMargin;

            if (null != PageScrolled)
                PageScrolled(this, new PageScrolledEventArgs
                    {
                        Position = position, 
                        PositionOffset = offset, 
                        PositionOffsetPixels = offsetPixels
                    });
        }

        private void CompleteScroll()
        {
            var needPopulate = _scrolling;
            if (needPopulate)
            {
                ScrollingCacheEnabled = false;
                _scroller.AbortAnimation();
                var oldX = ScrollX;
                var oldY = ScrollY;
                var x = _scroller.CurrX;
                var y = _scroller.CurrY;
                if (oldX != x || oldY != y)
                    ScrollTo(x, y);

                if (IsMenuOpen)
                {
                    if (null != Opened)
                        Opened(this, EventArgs.Empty);
                }
                else
                {
                    if (null != Closed)
                        Closed(this, EventArgs.Empty);
                }
            }
            _scrolling = false;
        }

        public TouchMode TouchMode { get; set; }

        private bool ThisTouchAllowed(MotionEvent ev)
        {
            var x = (int) (ev.GetX() + _scrollX);
            if (IsMenuOpen)
            {
                return _viewBehind.MenuOpenTouchAllowed(_content, _curItem, x);
            }
            switch (TouchMode)
            {
                case TouchMode.Fullscreen:
                    return !IsInIgnoredView(ev);
                case TouchMode.None:
                    return false;
                case TouchMode.Margin:
                    return _viewBehind.MarginTouchAllowed(_content, x);
            }
            return false;
        }

        private bool ThisSlideAllowed(float dx)
        {
            var allowed = IsMenuOpen ? _viewBehind.MenuOpenSlideAllowed(dx) 
                               : _viewBehind.MenuClosedSlideAllowed(dx);
#if DEBUG
            Log.Verbose(Tag, "this slide allowed" + allowed + " dx: " + dx);
#endif
            return allowed;
        }

        private int GetPointerIndex(MotionEvent ev, int id)
        {
            var activePointerIndex = MotionEventCompat.FindPointerIndex(ev, id);
            if (activePointerIndex == -1)
                ActivePointerId = InvalidPointer;
            return activePointerIndex;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (!_enabled)
                return false;

            var action = (int) ev.Action & MotionEventCompat.ActionMask;

#if DEBUG
            if (action == (int) MotionEventActions.Down)
                Log.Verbose(Tag, "Recieved ACTION_DOWN");
#endif
            if (action == (int) MotionEventActions.Cancel || action == (int) MotionEventActions.Up ||
                (action != (int) MotionEventActions.Down && _isUnableToDrag))
            {
                EndDrag();
                return false;
            }

            switch (action)
            {
                case (int) MotionEventActions.Move:
                    DetermineDrag(ev);
                    break;
                case (int) MotionEventActions.Down:
                    var index = MotionEventCompat.GetActionIndex(ev);
                    ActivePointerId = MotionEventCompat.GetPointerId(ev, index);
                    if (ActivePointerId == InvalidPointer)
                        break;
                    _lastMotionX = _initialMotionX = MotionEventCompat.GetX(ev, index);
                    _lastMotionY = MotionEventCompat.GetY(ev, index);
                    if (ThisTouchAllowed(ev))
                    {
                        _isBeingDragged = false;
                        _isUnableToDrag = false;
                        if (IsMenuOpen && _viewBehind.MenuTouchInQuickReturn(_content, _curItem, 
                            ev.GetX() + _scrollX))
                            _quickReturn = true;
                    }
                    else
                        _isUnableToDrag = true;
                    break;
                case (int) MotionEventActions.PointerUp:
                    OnSecondaryPointerUp(ev);
                    break;
            }

            if (!_isBeingDragged)
            {
                if (VelocityTracker == null)
                    VelocityTracker = VelocityTracker.Obtain();
                VelocityTracker.AddMovement(ev);    
            }
            return _isBeingDragged || _quickReturn;
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (!_enabled)
                return false;

            if (!_isBeingDragged && !ThisTouchAllowed(ev))
                return false;

            if (VelocityTracker == null)
                VelocityTracker = VelocityTracker.Obtain();
            VelocityTracker.AddMovement(ev);

            var action = (int)ev.Action & MotionEventCompat.ActionMask;
            switch (action)
            {
                case (int) MotionEventActions.Down:
                    CompleteScroll();

                    var index = MotionEventCompat.GetActionIndex(ev);
                    ActivePointerId = MotionEventCompat.GetPointerId(ev, index);
                    _lastMotionX = _initialMotionX = ev.GetX();
                    break;
                case (int) MotionEventActions.Move:
                    if (!_isBeingDragged)
                    {
                        DetermineDrag(ev);
                        if (_isUnableToDrag)
                            return false;
                    }
                    if (_isBeingDragged)
                    {
                        var activePointerIndex = GetPointerIndex(ev, ActivePointerId);
                        if (ActivePointerId == InvalidPointer)
                            break;
                        var x = MotionEventCompat.GetX(ev, activePointerIndex);
                        var deltaX = _lastMotionX - x;
                        _lastMotionX = x;
                        var oldScrollX = ScrollX;
                        var scrollX = oldScrollX + deltaX;
                        var leftBound = LeftBound;
                        var rightBound = RightBound;
                        if (scrollX < leftBound)
                            scrollX = leftBound;
                        else if (scrollX > rightBound)
                            scrollX = rightBound;
                        _lastMotionX += scrollX - (int) scrollX;
                        ScrollTo((int) scrollX, ScrollY);
                        OnPageScrolled((int)scrollX);
                    }
                    break;
                case (int) MotionEventActions.Up:
                    if (_isBeingDragged)
                    {
                        var velocityTracker = VelocityTracker;
                        velocityTracker.ComputeCurrentVelocity(1000, MaximumVelocity);
                        var initialVelocity =
                            (int) VelocityTrackerCompat.GetXVelocity(velocityTracker, ActivePointerId);
                        var scrollX = ScrollX;
                        var pageOffset = (float) (scrollX - GetDestScrollX(_curItem)) / BehindWidth;
                        var activePointerIndex = GetPointerIndex(ev, ActivePointerId);
                        if (ActivePointerId != InvalidPointer)
                        {
                            var x = MotionEventCompat.GetX(ev, activePointerIndex);
                            var totalDelta = (int) (x - _initialMotionX);
                            var nextPage = DetermineTargetPage(pageOffset, initialVelocity, totalDelta);
                            SetCurrentItemInternal(nextPage, true, true, initialVelocity);
                        }
                        else
                            SetCurrentItemInternal(_curItem, true, true, initialVelocity);
                        ActivePointerId = InvalidPointer;
                        EndDrag();
                    }
                    else if (_quickReturn &&
                             _viewBehind.MenuTouchInQuickReturn(_content, _curItem, ev.GetX() + _scrollX))
                    {
                        SetCurrentItem(1);
                        EndDrag();
                    }
                    break;
                case (int) MotionEventActions.Cancel:
                    if (_isBeingDragged)
                    {
                        SetCurrentItemInternal(_curItem, true, true);
                        ActivePointerId = InvalidPointer;
                        EndDrag();
                    }
                    break;
                case MotionEventCompat.ActionPointerDown:
                    var indexx = MotionEventCompat.GetActionIndex(ev);
                    _lastMotionX = MotionEventCompat.GetX(ev, indexx);
                    ActivePointerId = MotionEventCompat.GetPointerId(ev, indexx);
                    break;
                case MotionEventCompat.ActionPointerUp:
                    OnSecondaryPointerUp(ev);
                    var pointerIndex = GetPointerIndex(ev, ActivePointerId);
                    if (ActivePointerId == InvalidPointer)
                        break;
                    _lastMotionX = MotionEventCompat.GetX(ev, pointerIndex);
                    break;
            }
            return true;
        }

        private void DetermineDrag(MotionEvent ev)
        {
            var activePointerId = ActivePointerId;
            var pointerIndex = GetPointerIndex(ev, activePointerId);
            if (activePointerId == InvalidPointer || pointerIndex == InvalidPointer)
                return;
            var x = MotionEventCompat.GetX(ev, pointerIndex);
            var dx = x - _lastMotionX;
            var xDiff = Math.Abs(dx);
            var y = MotionEventCompat.GetY(ev, pointerIndex);
            var dy = y - _lastMotionY;
            var yDiff = Math.Abs(dy);
            if (xDiff > (IsMenuOpen ? _touchSlop / 2 : _touchSlop) && xDiff > yDiff && ThisSlideAllowed(dx))
            {
                StartDrag();
                _lastMotionX = x;
                _lastMotionY = y;
                _scrollingCacheEnabled = true;
            }
            else if (xDiff > _touchSlop)
                _isUnableToDrag = true;
        }

        public override void ScrollTo(int x, int y)
        {
            base.ScrollTo(x, y);

            _scrollX = x;
            _viewBehind.ScrollBehindTo(_content, x, y);
#if __ANDROID_11__
            ((SlidingMenu) Parent).ManageLayers(PercentOpen);
#endif
        }

        private int DetermineTargetPage(float pageOffset, int velocity, int deltaX)
        {
            var targetPage = _curItem;
            if (Math.Abs(deltaX) > _flingDistance && Math.Abs(velocity) > _minimumVelocity)
            {
                if (velocity > 0 && deltaX > 0)
                    targetPage -= 1;
                else if (velocity < 0 && deltaX < 0)
                    targetPage += 1;
            }
            else
                targetPage = (int) Math.Round(_curItem + pageOffset);
            return targetPage;
        }

        public float PercentOpen { get { return Math.Abs(_scrollX - _content.Left) / BehindWidth; } }

        protected override void DispatchDraw(Canvas canvas)
        {
            base.DispatchDraw(canvas);

            _viewBehind.DrawShadow(_content, canvas);
            _viewBehind.DrawFade(_content, canvas, PercentOpen);
            _viewBehind.DrawSelector(_content, canvas, PercentOpen);
        }

        private void OnSecondaryPointerUp(MotionEvent ev)
        {
#if DEBUG
            Log.Verbose(Tag, "OnSecondaryPointerUp called");
#endif
            var pointerIndex = MotionEventCompat.GetActionIndex(ev);
            var pointerId = MotionEventCompat.GetPointerId(ev, pointerIndex);
            if (pointerId == ActivePointerId)
            {
                var newPointerIndex = pointerIndex == 0 ? 1 : 0;
                _lastMotionX = MotionEventCompat.GetX(ev, newPointerIndex);
                ActivePointerId = MotionEventCompat.GetPointerId(ev, newPointerIndex);
                if (VelocityTracker != null)
                    VelocityTracker.Clear();
            }
        }

        private void StartDrag()
        {
            _isBeingDragged = true;
            _quickReturn = false;
        }

        private void EndDrag()
        {
            _quickReturn = false;
            _isBeingDragged = false;
            _isUnableToDrag = false;
            ActivePointerId = InvalidPointer;

            if (VelocityTracker == null) return;
            VelocityTracker.Recycle();
            VelocityTracker = null;
        }

        private bool ScrollingCacheEnabled
        {
            set
            {
                if (_scrollingCacheEnabled != value)
                {
                    _scrollingCacheEnabled = value;
                    if (UseCache)
                    {
                        var size = ChildCount;
                        for (var i = 0; i < size; ++i)
                        {
                            var child = GetChildAt(i);
                            if (child.Visibility != ViewStates.Gone)
                                child.DrawingCacheEnabled = value;
                        }
                    }
                }
            }
        }

        protected bool CanScroll(View v, bool checkV, int dx, int x, int y)
        {
            var viewGroup = v as ViewGroup;
            if (viewGroup != null)
            {
                var scrollX = v.ScrollX;
                var scrollY = v.ScrollY;
                var count = viewGroup.ChildCount;

                for (var i = count - 1; i >= 0; i--)
                {
                    var child = viewGroup.GetChildAt(i);
                    if (x + scrollX >= child.Left && x + scrollX < child.Right &&
                        y + scrollY >= child.Top && y + scrollY < child.Bottom &&
                        CanScroll(child, true, dx, x + scrollX - child.Left, y + scrollY - child.Top))
                        return true;
                }
            }
            return checkV && ViewCompat.CanScrollHorizontally(v, -dx);
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            return base.DispatchKeyEvent(e) || ExecuteKeyEvent(e);
        }

        public bool ExecuteKeyEvent(KeyEvent ev)
        {
            var handled = false;
            if (ev.Action == KeyEventActions.Down)
            {
                switch (ev.KeyCode)
                {
                    case Keycode.DpadLeft:
                        handled = ArrowScroll(FocusSearchDirection.Left);
                        break;
                    case Keycode.DpadRight:
                        handled = ArrowScroll(FocusSearchDirection.Right);
                        break;
                    case Keycode.Tab:
                        if ((int)Build.VERSION.SdkInt >= 11)
                        {
                            if (KeyEventCompat.HasNoModifiers(ev))
                                handled = ArrowScroll(FocusSearchDirection.Forward);
#if __ANDROID_11__
                            else if (ev.IsMetaPressed)
                                handled = ArrowScroll(FocusSearchDirection.Backward);
#endif
                        }
                        break;
                }
            }
            return handled;
        }

        public bool ArrowScroll(FocusSearchDirection direction)
        {
            var currentFocused = FindFocus();

            var handled = false;

            var nextFocused = FocusFinder.Instance.FindNextFocus(this, currentFocused == this ? null : currentFocused, direction);
            if (nextFocused != null && nextFocused != currentFocused)
            {
                if (direction == FocusSearchDirection.Left)
                    handled = nextFocused.RequestFocus();
                else if (direction == FocusSearchDirection.Right)
                {
                    if (currentFocused != null && nextFocused.Left <= currentFocused.Left)
                        handled = PageRight();
                    else
                        handled = nextFocused.RequestFocus();
                }
            }
            else if (direction == FocusSearchDirection.Left || direction == FocusSearchDirection.Backward)
                handled = PageLeft();
            else if (direction == FocusSearchDirection.Right || direction == FocusSearchDirection.Forward)
                handled = PageRight();

            if (handled)
                PlaySoundEffect(SoundEffectConstants.GetContantForFocusDirection(direction));

            return handled;
        }

        bool PageLeft()
        {
            if (_curItem > 0)
            {
                SetCurrentItem(_curItem-1, true);
                return true;
            }
            return false;
        }

        bool PageRight()
        {
            if (_curItem < 1)
            {
                SetCurrentItem(_curItem+1, true);
                return true;
            }
            return false;
        }
    }
}
