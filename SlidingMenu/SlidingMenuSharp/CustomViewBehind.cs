using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Math = System.Math;

namespace SlidingMenuSharp
{
    public sealed class CustomViewBehind : ViewGroup
    {
        private new const string Tag = "CustomViewBehind";

        private const int MARGIN_THRESHOLD = 48;

        private View _content;
        private View _secondaryContent;
        private int _widthOffset;
        private ICanvasTransformer _transformer;
        private MenuMode _mode;
        private readonly Paint _fadePaint = new Paint();
        private Drawable _shadowDrawable;
        private Drawable _secondaryShadowDrawable;
        private int _shadowWidth;
        private float _fadeDegree;
        private Bitmap _selectorDrawable;
        private View _selectedView;

        public CustomViewBehind(Context context) 
            : this(context, null)
        {
        }

        public CustomViewBehind(Context context, IAttributeSet attrs) 
            : base(context, attrs)
        {
            TouchMode = TouchMode.Margin;
            MarginThreshold =
                (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, MARGIN_THRESHOLD, Resources.DisplayMetrics);
        }

        public CustomViewAbove CustomViewAbove { get; set; }

        public ICanvasTransformer CanvasTransformer
        {
            set { _transformer = value; }
        }

        public int WidthOffset
        {
            get { return _widthOffset; }
            set 
            {
                _widthOffset = value;
                RequestLayout();
            }
        }

        public int MarginThreshold { get; set; }

        public int BehindWidth
        {
            get { return _content.Width; }
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

        public View SecondaryContent
        {
            get { return _secondaryContent; }
            set
            {
                if (_secondaryContent != null)
                    RemoveView(_secondaryContent);
                _secondaryContent = value;
                AddView(_secondaryContent);    
            }
        }

        public bool ChildrenEnabled { get; set; }

        public override void ScrollTo(int x, int y)
        {
            base.ScrollTo(x, y);
            if (_transformer != null)
                Invalidate();
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return !ChildrenEnabled;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return !ChildrenEnabled;
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            if (_transformer != null)
            {
                canvas.Save();
                _transformer.TransformCanvas(canvas, CustomViewAbove.PercentOpen);
                base.DispatchDraw(canvas);
                canvas.Restore();
            }
            else
                base.DispatchDraw(canvas);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{ 
            var width = r - l;
            var height = b - t;
 
			var widthper = (width * 25) / 100;

			Content.Layout(0, 0, width - _widthOffset, height);
            if (SecondaryContent != null)
                SecondaryContent.Layout(0, 0, width - _widthOffset, height);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var width = GetDefaultSize(0, widthMeasureSpec);
            var height = GetDefaultSize(0, heightMeasureSpec);
            SetMeasuredDimension(width, height);
            var contentWidth = GetChildMeasureSpec(widthMeasureSpec, 0, width - _widthOffset);
            var contentHeight = GetChildMeasureSpec(heightMeasureSpec, 0, height);
            Content.Measure(contentWidth, contentHeight);
            if (SecondaryContent != null)
                SecondaryContent.Measure(contentWidth, contentHeight);
        }

        public MenuMode Mode
        {
            get { return _mode; }
            set
            {
                if (value == MenuMode.Left || value == MenuMode.Right)
                {
                    if (_content != null)
                        _content.Visibility = ViewStates.Visible;
                    if (_secondaryContent != null)
                        _secondaryContent.Visibility = ViewStates.Invisible;
                }
                _mode = value;
            }
        }

        public float ScrollScale { get; set; }

        public Drawable ShadowDrawable
        {
            get { return _shadowDrawable; }
            set 
            {
                _shadowDrawable = value;
                Invalidate();
            }
        }

        public Drawable SecondaryShadowDrawable
        {
            get { return _secondaryShadowDrawable; }
            set
            {
                _secondaryShadowDrawable = value;
                Invalidate();
            }
        }

        public int ShadowWidth
        {
            get { return _shadowWidth; }
            set
            {
                _shadowWidth = value;
                Invalidate();
            }
        }

        public bool FadeEnabled { get; set; }

        public float FadeDegree
        {
            get { return _fadeDegree; }
            set
            {
                if (value > 1.0f || value < 0.0f)
                    throw new ArgumentOutOfRangeException("value", "The BehindFadeDegree must be between 0.0f and 1.0f");
                _fadeDegree = value;
            }
        }

        public int GetMenuPage(int page)
        {
            page = (page > 1) ? 2 : ((page < 1) ? 0 : page);
            if (Mode == MenuMode.Left && page > 1)
                return 0;
            if (Mode == MenuMode.Right && page < 1)
                return 2;
            return page;
        }

        public void ScrollBehindTo(View content, int x, int y)
        {
            var vis = ViewStates.Visible;
            switch (Mode)
            {
                case MenuMode.Left:
                    if (x >= content.Left)
                        vis = ViewStates.Invisible;
                    ScrollTo((int)((x + BehindWidth) * ScrollScale), y);
                    break;
                case MenuMode.Right:
                    if (x <= content.Left)
                        vis = ViewStates.Invisible;
                    ScrollTo((int)(BehindWidth - Width + (x - BehindWidth) * ScrollScale), y);
                    break;
                case MenuMode.LeftRight:
                    Content.Visibility = x >= content.Left ? ViewStates.Invisible : ViewStates.Visible;
					SecondaryContent.Visibility = x <= content.Left ? ViewStates.Invisible : ViewStates.Visible;
                    vis = x == 0 ? ViewStates.Invisible : ViewStates.Visible;
                    if (x <= content.Left)
                        ScrollTo((int)((x+BehindWidth)*ScrollScale), y);
                    else
                        ScrollTo((int)(BehindWidth - Width + (x-BehindWidth)*ScrollScale), y);
                    break;
            }
            if (vis == ViewStates.Invisible)
                Log.Verbose(Tag, "behind INVISIBLE");
            Visibility = vis;
        }

        public int GetMenuLeft(View content, int page)
        {
            switch (Mode)
            {
                case MenuMode.Left:
                    switch (page)
                    {
                        case 0:
                            return content.Left - BehindWidth;
                        case 2:
                            return content.Left;
                    }
                    break;
                case MenuMode.Right:
                    switch (page)
                    {
                        case 0:
                            return content.Left;
                        case 2:
                            return content.Left + BehindWidth;
                    }
                    break;
                case MenuMode.LeftRight:
                    switch (page)
                    {
                        case 0:
                            return content.Left - BehindWidth;
                        case 2:
                            return content.Left + BehindWidth;
                    }
                    break;
            }
            return content.Left;
        }

        public int GetAbsLeftBound(View content)
        {
            if (Mode == MenuMode.Left || Mode == MenuMode.LeftRight)
                return content.Left - BehindWidth;
            return Mode == MenuMode.Right ? content.Left : 0;
        }

        public int GetAbsRightBound(View content)
        {
            if (Mode == MenuMode.Right || Mode == MenuMode.LeftRight)
                return content.Left + BehindWidth;
            return Mode == MenuMode.Left ? content.Left : 0;
        }

        public bool MarginTouchAllowed(View content, int x)
        {
            var left = content.Left;
            var right = content.Right;
            if (Mode == MenuMode.Left)
                return (x >= left && x <= MarginThreshold + left);
            if (Mode == MenuMode.Right)
                return (x <= right && x >= right - MarginThreshold);
            if (Mode == MenuMode.LeftRight)
                return (x >= left && x <= MarginThreshold + left) ||
                       (x <= right && x >= right - MarginThreshold);
            return false;
        }

        public TouchMode TouchMode { get; set; }

        public bool MenuOpenTouchAllowed(View content, int currPage, float x)
        {
            switch (TouchMode)
            {
                case TouchMode.Fullscreen:
                    return true;
                case TouchMode.Margin:
                    return MenuTouchInQuickReturn(content, currPage, x);
            }
            return false;
        }

        public bool MenuTouchInQuickReturn(View content, int currPage, float x)
        {
            if (Mode == MenuMode.Left || (Mode == MenuMode.LeftRight && currPage == 0))
                return x >= content.Left;
            if (Mode == MenuMode.Right || (Mode == MenuMode.LeftRight && currPage == 2))
                return x <= content.Right;
            return false;
        }

        public bool MenuClosedSlideAllowed(float dx)
        {
            switch (Mode)
            {
                case MenuMode.Left:
                    return dx > 0;
                case MenuMode.Right:
                    return dx < 0;
                case MenuMode.LeftRight:
                    return true;
            }
            return false;
        }

        public bool MenuOpenSlideAllowed(float dx)
        {
            switch (Mode)
            {
                case MenuMode.Left:
                    return dx < 0;
                case MenuMode.Right:
                    return dx > 0;
                case MenuMode.LeftRight:
                    return true;
            }
            return false;
        }

        public void DrawShadow(View content, Canvas canvas)
        {
            if (ShadowDrawable == null || ShadowWidth <= 0) return;
            var left = 0;
            switch (Mode)
            {
                case MenuMode.Left:
                    left = content.Left - ShadowWidth;
                    break;
                case MenuMode.Right:
                    left = content.Right;
                    break;
                case MenuMode.LeftRight:
                    if (SecondaryShadowDrawable != null)
                    {
                        left = content.Right;
                        SecondaryShadowDrawable.SetBounds(left, 0, left + ShadowWidth, Height);
                        SecondaryShadowDrawable.Draw(canvas);
                    }
                    left = content.Left - ShadowWidth;
                    break;
            }
            ShadowDrawable.SetBounds(left, 0, left + ShadowWidth, Height);
            ShadowDrawable.Draw(canvas);
        }

        public void DrawFade(View content, Canvas canvas, float openPercent)
        {
            if (!FadeEnabled) return;

            var alpha = (int) (FadeDegree * 255 * Math.Abs(1 - openPercent));
            _fadePaint.Color = Color.Argb(alpha, 0, 0, 0);
            var left = 0;
            var right = 0;
            switch (Mode)
            {
                case MenuMode.Left:
                    left = content.Left - BehindWidth;
                    right = content.Left;
                    break;
                case MenuMode.Right:
                    left = content.Right;
                    right = content.Right + BehindWidth;
                    break;
                case MenuMode.LeftRight:
                    left = content.Left - BehindWidth;
                    right = content.Left;
                    canvas.DrawRect(left, 0, right, Height, _fadePaint);
                    left = content.Right;
                    right = content.Right + BehindWidth;
                    break;
            }
            canvas.DrawRect(left, 0, right, Height, _fadePaint);
        }

        public void DrawSelector(View content, Canvas canvas, float openPercent)
        {
            if (!SelectorEnabled) return;
            if (_selectorDrawable != null && SelectedView != null)
            {
                var tag = (string)SelectedView.GetTag(Resource.Id.selected_view);
                if (tag.Equals(Tag + "SelectedView"))
                {
                    canvas.Save();
                    int left, right;
                    var offset = (int)(SelectorBitmap.Width * openPercent);
                    if (Mode == MenuMode.Left)
                    {
                        right = content.Left;
                        left = right - offset;
                        canvas.ClipRect(left, 0, right, Height);
                        canvas.DrawBitmap(SelectorBitmap, left, SelectorTop, null);
                    }
                    else if (Mode == MenuMode.Right)
                    {
                        left = content.Right;
                        right = left + offset;
                        canvas.ClipRect(left, 0, right, Height);
                        canvas.DrawBitmap(SelectorBitmap, right - SelectorBitmap.Width, SelectorTop, null);
                    }
                    canvas.Restore();
                }
            }
        }

        public bool SelectorEnabled { get; set; }

        public View SelectedView
        {
            get { return _selectedView; }
            set
            {
                if (_selectedView != null)
                {
                    _selectedView.SetTag(Resource.Id.selected_view, null);
                    _selectedView = null;
                }
                if (value != null && value.Parent != null)
                {
                    _selectedView = value;
                    _selectedView.SetTag(Resource.Id.selected_view, Tag+"SelectedView");
                    Invalidate();
                }
            }
        }

        private int SelectorTop
        {
            get
            {
                var y = SelectedView.Top;
                y += (SelectedView.Height - _selectorDrawable.Height) / 2;
                return y;
            }
        }

        public Bitmap SelectorBitmap
        {
            get { return _selectorDrawable; }
            set
            {
                _selectorDrawable = value;
                RefreshDrawableState();
            }
        }
    }
}