using System;
using Android.App;
using Android.OS;
using Android.Views;

namespace SlidingMenuSharp.App
{
    public class SlidingActivityHelper
    {
        private readonly Activity _activity;
        private SlidingMenu _slidingMenu;
        private View _viewAbove;
        private View _viewBehind;
        private bool _broadcasting;
        private bool _onPostCreateCalled;
        private bool _enableSlide = true;

        public SlidingActivityHelper(Activity activity)
        {
            ResourceIdManager.UpdateIdValues();
            _activity = activity;
        }

        public void OnCreate(Bundle savedInstanceState)
        {
            _slidingMenu = (SlidingMenu) LayoutInflater.From(_activity).Inflate(Resource.Layout.slidingmenumain, null);
        }

        public void OnPostCreate(Bundle savedInstanceState)
        {
            if (null == _viewBehind && null == _viewAbove)
                throw new InvalidOperationException("Both SetBehindContentView must be called " +
                    "in OnCreate in addition to SetContentView.");

            _onPostCreateCalled = true;

            _slidingMenu.AttachToActivity(_activity, 
                _enableSlide ? SlideStyle.Window : SlideStyle.Content);

            bool open, secondary;
            if (null != savedInstanceState)
            {
                open = savedInstanceState.GetBoolean("SlidingActivityHelper.open");
                secondary = savedInstanceState.GetBoolean("SlidingActivityHelper.secondary");
            }
            else
            {
                open = false;
                secondary = false;
            }

            new Handler().Post(() =>
                {
                    if (open)
                    {
                        if (secondary)
                            _slidingMenu.ShowSecondaryMenu(false);
                        else
                            _slidingMenu.ShowMenu(false);
                    }
                    else
                        _slidingMenu.ShowContent(false);
                });
        }

        public bool SlidingActionBarEnabled
        {
            get { return _enableSlide; }
            set
            {
                if (_onPostCreateCalled)
                    throw new InvalidOperationException("EnableSlidingActionBar must be called in OnCreate.");
                _enableSlide = value;
            }
        }

        public View FindViewById(int id)
        {
            if (_slidingMenu != null)
            {
                var v = _slidingMenu.FindViewById(id);
                if (v != null)
                    return v;
            }
            return null;
        }

        public void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean("SlidingActivityHelper.open", _slidingMenu.IsMenuShowing);
            outState.PutBoolean("SlidingActivityHelper.secondary", _slidingMenu.IsSecondaryMenuShowing);
        }

        public void RegisterAboveContentView(View v, ViewGroup.LayoutParams layoutParams)
        {
            if (_broadcasting)
                _viewAbove = v;
        }

        public void SetContentView(View v)
        {
            _broadcasting = true;
            _activity.SetContentView(v);
        }

        public void SetBehindContentView(View view, ViewGroup.LayoutParams layoutParams)
        {
            _viewBehind = view;
            _slidingMenu.SetMenu(_viewBehind);
        }

        public SlidingMenu SlidingMenu
        {
            get { return _slidingMenu; }
        }

        public void Toggle()
        {
            _slidingMenu.Toggle();
        }

        public void ShowContent()
        {
            _slidingMenu.ShowContent();
        }

        public void ShowMenu()
        {
            _slidingMenu.ShowMenu();
        }

        public void ShowSecondaryMenu()
        {
            _slidingMenu.ShowSecondaryMenu();
        }

        public bool OnKeyUp(Keycode keycode, KeyEvent keyEvent)
        {
            if (keycode == Keycode.Back && _slidingMenu.IsMenuShowing)
            {
                ShowContent();
                return true;
            }
            return false;
        }
    }
}