using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences; 

namespace Sample
{
    [Activity(Label = "SlidingMenu Demo", Theme = "@style/ExampleTheme")]
    public class ExampleListActivity : PreferenceActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTitle(Resource.String.app_name);
            AddPreferencesFromResource(Resource.Xml.main);
        }

        public override bool OnPreferenceTreeClick(PreferenceScreen preferenceScreen, Preference preference)
        {
            Type cls = null;
            var title = preference.Title;

			if (title.Equals(GetString(Resource.String.left_and_right)))
				cls = typeof(LeftAndRightActivity);

//            if (title.Equals(GetString(Resource.String.properties)))
//                cls = typeof(PropertiesActivity);
//            else if (title.Equals(GetString(Resource.String.attach)))
//                cls = typeof(AttachExample);
//            else if (title.Equals(GetString(Resource.String.changing_fragments)))
//                cls = typeof(FragmentChangeActivity);
//            else if (title.Equals(GetString(Resource.String.left_and_right)))
//                cls = typeof(LeftAndRightActivity);
//            else if (title.Equals(GetString(Resource.String.responsive_ui)))
//                cls = typeof(ResponsiveUIActivity);
//            else if (title.Equals(GetString(Resource.String.viewpager)))
//                cls = typeof(ViewPagerActivity);
//            else if (title.Equals(GetString(Resource.String.title_bar_slide)))
//                cls = typeof(SlidingTitleBar);
//            else if (title.Equals(GetString(Resource.String.title_bar_content)))
//                cls = typeof(SlidingContent);
//            else if (title.Equals(GetString(Resource.String.anim_zoom)))
//                cls = typeof(ZoomAnimation);
//            else if (title.Equals(GetString(Resource.String.anim_scale)))
//                cls = typeof(ScaleAnimation);
//            else if (title.Equals(GetString(Resource.String.anim_slide)))
//                cls = typeof(SlideAnimation);

            var intent = new Intent(this, cls);
            StartActivity(intent);
            return true;
        }
    }
}