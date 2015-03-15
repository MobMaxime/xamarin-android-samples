using System;
using Android.App;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.OS;
using Android;

namespace StickyListHeaders 
{
    [Activity(Label = "XamDroid.StickyListHeaders.Sample", MainLauncher = true, Icon = "@drawable/icon")]
#if __ANDROID_11__
    public class Activity1 : FragmentActivity, ViewPager.IOnPageChangeListener, ActionBar.ITabListener
#else
    public class Activity1 : FragmentActivity, ViewPager.IOnPageChangeListener
#endif
    {
        private ViewPager m_Pager;
        private MainPagerAdapter m_Adapter;
#if __ANDROID_11__

            public void OnTabReselected(ActionBar.Tab tab, Android.App.FragmentTransaction ft)
            {
            }

            public void OnTabSelected(ActionBar.Tab tab, Android.App.FragmentTransaction ft)
            {
               m_Pager.SetCurrentItem(tab.Position, true);
            }

            public void OnTabUnselected(ActionBar.Tab tab, Android.App.FragmentTransaction ft)
            {
            }

#endif

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            m_Pager = FindViewById<ViewPager>(Resource.Id.pager);
            m_Adapter = new MainPagerAdapter(SupportFragmentManager);
            m_Pager.Adapter = m_Adapter;

#if __ANDROID_11__
            m_Pager.SetOnPageChangeListener(this);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            var tab = ActionBar.NewTab();
            tab.SetText("1");
            tab.SetTabListener(this);

            ActionBar.AddTab(tab);

            tab = ActionBar.NewTab();
            tab.SetText("2");
            tab.SetTabListener(this);
            ActionBar.AddTab(tab);
            
            tab = ActionBar.NewTab();
            tab.SetText("3");
            tab.SetTabListener(this);
            ActionBar.AddTab(tab); 
            
            tab = ActionBar.NewTab();
            tab.SetText("4");
            tab.SetTabListener(this);
            ActionBar.AddTab(tab);
#endif

        }

        public void OnPageScrollStateChanged(int state)
        {
            
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            
        }

        public void OnPageSelected(int position)
        {

#if __ANDROID_11__
            ActionBar.SetSelectedNavigationItem(position);
#endif
        }


    }
}

