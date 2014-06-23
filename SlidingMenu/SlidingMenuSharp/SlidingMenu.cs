using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Interop;

namespace SlidingMenuSharp
{
    public class SlidingMenu : RelativeLayout
    {
        private new const string Tag = "SlidingMenu";
	    private bool _mActionbarOverlay;

	    private readonly CustomViewAbove _viewAbove;
	    private readonly CustomViewBehind _viewBehind;

        public event EventHandler Open;
        public event EventHandler Close;
        public event EventHandler Opened;
        public event EventHandler Closed;

        public SlidingMenu(Context context) 
            : this(context, null)
        {
        }

        public SlidingMenu(Context context, IAttributeSet attrs) 
            : this(context, attrs, 0)
        {
        }

        public SlidingMenu(Context context, IAttributeSet attrs, int defStyle) 
            : base(context, attrs, defStyle)
        {
            var behindParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            _viewBehind = new CustomViewBehind(context);
            AddView(_viewBehind, behindParams);

            var aboveParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            _viewAbove = new CustomViewAbove(context);
            AddView(_viewAbove, aboveParams);

            _viewAbove.CustomViewBehind = _viewBehind;
            _viewBehind.CustomViewAbove = _viewAbove;

            _viewAbove.PageSelected += (sender, args) =>
                {
                    if (args.Position == 0 && null != Open) //position open
                        Open(this, EventArgs.Empty);
                    else if (args.Position == 2 && null != Close) //position close
                        Close(this, EventArgs.Empty);
                };

            _viewAbove.Opened += (sender, args) => { if (null != Opened) Opened(sender, args); };
            _viewAbove.Closed += (sender, args) => { if (null != Closed) Closed(sender, args); };

            var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.SlidingMenu);
            var mode = a.GetInt(Resource.Styleable.SlidingMenu_mode, (int) MenuMode.Left);
            Mode = (MenuMode) mode;
            
            var viewAbove = a.GetResourceId(Resource.Styleable.SlidingMenu_viewAbove, -1);
            if (viewAbove != -1)
                SetContent(viewAbove);
            else
                SetContent(new FrameLayout(context));

            TouchModeAbove = (TouchMode)a.GetInt(Resource.Styleable.SlidingMenu_touchModeAbove, (int)TouchMode.Margin);
            TouchModeBehind = (TouchMode)a.GetInt(Resource.Styleable.SlidingMenu_touchModeBehind, (int)TouchMode.Margin);

            var offsetBehind = (int) a.GetDimension(Resource.Styleable.SlidingMenu_behindOffset, -1);
            var widthBehind = (int) a.GetDimension(Resource.Styleable.SlidingMenu_behindWidth, -1);
            if (offsetBehind != -1 && widthBehind != -1)
                throw new ArgumentException("Cannot set both behindOffset and behindWidth for SlidingMenu, check your XML");
            if (offsetBehind != -1)
                BehindOffset = offsetBehind;
            else if (widthBehind != -1)
                BehindWidth = widthBehind;
            else
                BehindOffset = 0;

            var shadowRes = a.GetResourceId(Resource.Styleable.SlidingMenu_shadowDrawable, -1);
            if (shadowRes != -1)
                ShadowDrawableRes = shadowRes;

            BehindScrollScale = a.GetFloat(Resource.Styleable.SlidingMenu_behindScrollScale, 0.33f);
            ShadowWidth = ((int)a.GetDimension(Resource.Styleable.SlidingMenu_shadowWidth, 0));
            FadeEnabled = a.GetBoolean(Resource.Styleable.SlidingMenu_fadeEnabled, true);
            FadeDegree = a.GetFloat(Resource.Styleable.SlidingMenu_fadeDegree, 0.33f);
            SelectorEnabled = a.GetBoolean(Resource.Styleable.SlidingMenu_selectorEnabled, false);
            var selectorRes = a.GetResourceId(Resource.Styleable.SlidingMenu_selectorDrawable, -1);
            if (selectorRes != -1)
                SelectorDrawable = selectorRes;

            a.Recycle();
        }

        public void AttachToActivity(Activity activity, SlideStyle slideStyle)
        {
            AttachToActivity(activity, slideStyle, false);
        }

        public void AttachToActivity(Activity activity, SlideStyle slideStyle, bool actionbarOverlay)
        {
            if (Parent != null)
                throw new ArgumentException("This SlidingMenu appears to already be attached");

            // get the window background
            var a = activity.Theme.ObtainStyledAttributes(new[] { Android.Resource.Attribute.WindowBackground });
            var background = a.GetResourceId(0, 0);
            a.Recycle();

            switch (slideStyle)
            {
                case SlideStyle.Window:
                    _mActionbarOverlay = false;
                    var decor = (ViewGroup)activity.Window.DecorView;
                    var decorChild = (ViewGroup)decor.GetChildAt(0);
                    // save ActionBar themes that have transparent assets
                    decorChild.SetBackgroundResource(background);
                    decor.RemoveView(decorChild);
                    decor.AddView(this);
                    SetContent(decorChild);
                    break;
                case SlideStyle.Content:
                    _mActionbarOverlay = actionbarOverlay;
                    // take the above view out of
                    var contentParent = (ViewGroup)activity.FindViewById(Android.Resource.Id.Content);
                    var content = contentParent.GetChildAt(0);
                    contentParent.RemoveView(content);
                    contentParent.AddView(this);
                    SetContent(content);
                    // save people from having transparent backgrounds
                    if (content.Background == null)
                        content.SetBackgroundResource(background);
                    break;
            }
        }

        public void SetContent(int res)
        {
            SetContent(LayoutInflater.From(Context).Inflate(res, null));
        }

        public void SetContent(View view)
        {
            _viewAbove.Content = view;
            ShowContent();
        }

        public View GetContent()
        {
            return _viewAbove.Content;
        }

        public void SetMenu(int res)
        {
            SetMenu(LayoutInflater.From(Context).Inflate(res, null));
        }

        public void SetMenu(View v)
        {
            _viewBehind.Content = v;
        }

        public View GetMenu()
        {
            return _viewBehind.Content;
        }

        public void SetSecondaryMenu(int res)
        {
            SetSecondaryMenu(LayoutInflater.From(Context).Inflate(res, null));
        }

        public void SetSecondaryMenu(View v)
        {
            _viewBehind.SecondaryContent = v;
        }

        public View GetSecondaryMenu()
        {
            return _viewBehind.SecondaryContent;
        }

        public bool IsSlidingEnabled
        {
            get { return _viewAbove.IsSlidingEnabled; }
            set { _viewAbove.IsSlidingEnabled = value; }
        }

        public MenuMode Mode
        {
            get { return _viewBehind.Mode; }
            set
            {
                if (value != MenuMode.Left && value != MenuMode.Right && value != MenuMode.LeftRight)
                {
                    throw new ArgumentException("SlidingMenu mode must be LEFT, RIGHT, or LEFT_RIGHT", "value");
                }
                _viewBehind.Mode = value;
            }
        }

        public bool Static
        {
            set
            {
                if (value)
                {
                    IsSlidingEnabled = false;
                    _viewAbove.CustomViewBehind = null;
                    _viewAbove.SetCurrentItem(1);
                }
                else
                {
                    _viewAbove.SetCurrentItem(1);
                    _viewAbove.CustomViewBehind = _viewBehind;
                    IsSlidingEnabled = true;
                }
            }
        }

        public void ShowMenu()
        {
            ShowMenu(true);
        }

        public void ShowMenu(bool animate)
        {
            _viewAbove.SetCurrentItem(0, animate);
        }

        public void ShowSecondaryMenu()
        {
            ShowSecondaryMenu(true);
        }

        public void ShowSecondaryMenu(bool animate)
        {
            _viewAbove.SetCurrentItem(2, animate);
        }

        public void ShowContent(bool animate = true)
        {
            _viewAbove.SetCurrentItem(1, animate);
        }

        public void Toggle()
        {
            Toggle(true);
        }

        public void Toggle(bool animate)
        {
            if (IsMenuShowing)
            {
                ShowContent(animate);
            }
            else
            {
                ShowMenu(animate);
            }
        }

        public bool IsMenuShowing
        {
            get { return _viewAbove.GetCurrentItem() == 0 || _viewAbove.GetCurrentItem() == 2; }
        }

        public bool IsSecondaryMenuShowing
        {
            get { return _viewAbove.GetCurrentItem() == 2; }
        }

        public int BehindOffset
        {
            get { return _viewBehind.WidthOffset; }
            set
            {
                _viewBehind.WidthOffset = value;
            }
        }

        public int BehindOffsetRes
        {
            set
            { 
                var i = (int) Context.Resources.GetDimension(value);
                BehindOffset = i;
            }
        }

        public int AboveOffset
        {
            set { _viewAbove.AboveOffset = value; }
        }

        public int AboveOffsetRes
        {
            set 
            {
                var i = (int) Context.Resources.GetDimension(value);
                AboveOffset = i;
            }
        }

        public int BehindWidth
        {
            set
            {
                var windowService = Context.GetSystemService(Context.WindowService);
                var windowManager = windowService.JavaCast<IWindowManager>();
                var width = windowManager.DefaultDisplay.Width;
                BehindOffset = width - value;
            }
        }

        public int BehindWidthRes
        {
            set
            {
                var i = (int)Context.Resources.GetDimension(value);
                BehindWidth = i;
            }
        }

        public float BehindScrollScale
        {
            get { return _viewBehind.ScrollScale; }
            set
            {
                if (value < 0f && value > 1f)
                    throw new ArgumentOutOfRangeException("value", "ScrollScale must be between 0f and 1f");
                _viewBehind.ScrollScale = value;
            }
        }

        public int TouchmodeMarginThreshold
        {
            get { return _viewBehind.MarginThreshold; }
            set { _viewBehind.MarginThreshold = value; }
        }

        public TouchMode TouchModeAbove
        {
            get { return _viewAbove.TouchMode; }
            set
            {
                if (value != TouchMode.Fullscreen && value != TouchMode.Margin
                    && value != TouchMode.None)
                {
                    throw new ArgumentException("TouchMode must be set to either" +
                            "TOUCHMODE_FULLSCREEN or TOUCHMODE_MARGIN or TOUCHMODE_NONE.", "value");
                }
                _viewAbove.TouchMode = value;
            }
        }

        public TouchMode TouchModeBehind
        {
            set
            {
                if (value != TouchMode.Fullscreen && value != TouchMode.Margin
                    && value != TouchMode.None)
                {
                    throw new ArgumentException("TouchMode must be set to either" +
                            "TOUCHMODE_FULLSCREEN or TOUCHMODE_MARGIN or TOUCHMODE_NONE.", "value");
                }
                _viewBehind.TouchMode = value;
            }
        }

        public int ShadowDrawableRes
        {
            set { _viewBehind.ShadowDrawable = Context.Resources.GetDrawable(value); }
        }

        public Drawable ShadowDrawable
        {
            set { _viewBehind.ShadowDrawable = value; }
        }

        public int SecondaryShadowDrawableRes
        {
            set { _viewBehind.SecondaryShadowDrawable = Context.Resources.GetDrawable(value); }
        }

        public Drawable SecondaryShadowDrawable
        {
            set { _viewBehind.SecondaryShadowDrawable = value; }
        }

        public int ShadowWidthRes
        {
            set { ShadowWidth = (int)Context.Resources.GetDimension(value); }
        }

        public int ShadowWidth
        {
            set { _viewBehind.ShadowWidth = value; }
        }

        public bool FadeEnabled
        {
            set { _viewBehind.FadeEnabled = value; }
        }

        public float FadeDegree
        {
            set { _viewBehind.FadeDegree = value; }
        }

        public bool SelectorEnabled
        {
            set { _viewBehind.SelectorEnabled = value; }
        }

        public View SelectedView
        {
            set { _viewBehind.SelectedView = value; }
        }

        public int SelectorDrawable
        {
            set { SelectorBitmap = BitmapFactory.DecodeResource(Resources, value); }
        }

        public Bitmap SelectorBitmap
        {
            set { _viewBehind.SelectorBitmap = value; }
        }

        public void AddIgnoredView(View v)
        {
            _viewAbove.AddIgnoredView(v);
        }

        public void RemoveIgnoredView(View v)
        {
            _viewAbove.RemoveIgnoredView(v);
        }

        public void ClearIgnoredViews()
        {
            _viewAbove.ClearIgnoredViews();
        }

        public ICanvasTransformer BehindCanvasTransformer
        {
            set { _viewBehind.CanvasTransformer = value; }
        }

        public class SavedState: BaseSavedState
        {
            public int Item { get; private set; }

            public SavedState(IParcelable superState, int item)
                : base(superState)
            {
                Item = item;
            }

            public SavedState(Parcel parcel)
                : base(parcel)
            {
                Item = parcel.ReadInt();
            }

            public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
            {
                base.WriteToParcel(dest, flags);
                dest.WriteInt(Item);
            }

            [ExportField("CREATOR")]
            static SavedStateCreator InitializeCreator()
            {
                return new SavedStateCreator();
            }

            class SavedStateCreator : Java.Lang.Object, IParcelableCreator
            {
                public Java.Lang.Object CreateFromParcel(Parcel source)
                {
                    return new SavedState(source);
                }

                public Java.Lang.Object[] NewArray(int size)
                {
                    return new SavedState[size];
                }
            }
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            try
            {
                var savedState = (SavedState)state;
                base.OnRestoreInstanceState(savedState.SuperState);
                _viewAbove.SetCurrentItem(savedState.Item);
            }
            catch
            {
                base.OnRestoreInstanceState(state);
                // Ignore, this needs to support IParcelable...
            }
        }

        protected override IParcelable OnSaveInstanceState()
        {
            var superState = base.OnSaveInstanceState();
            var savedState = new SavedState(superState, _viewAbove.GetCurrentItem());
            return savedState;
        }

        protected override bool FitSystemWindows(Rect insets)
        {
            if (!_mActionbarOverlay)
            {
                Log.Verbose(Tag, "setting padding");
                SetPadding(insets.Left, insets.Top, insets.Right, insets.Bottom);
            }
            return true;
        }

#if __ANDROID_11__
        public void ManageLayers(float percentOpen)
        {
            if ((int) Build.VERSION.SdkInt < 11) return;

            var layer = percentOpen > 0.0f && percentOpen < 1.0f;
            var layerType = layer ? LayerType.Hardware : LayerType.None;

            if (layerType != GetContent().LayerType)
            {
                Handler.Post(() =>
                    {
                        Log.Verbose(Tag, "changing layerType, hardware? " + (layerType == LayerType.Hardware));
                        GetContent().SetLayerType(layerType, null);
                        GetMenu().SetLayerType(layerType, null);
                        if (GetSecondaryMenu() != null)
                            GetSecondaryMenu().SetLayerType(layerType, null);
                    });
            }
        }
#endif
    }
}
