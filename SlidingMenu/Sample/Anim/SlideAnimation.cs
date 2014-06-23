using Android.App;
using SlidingMenuSharp;

namespace Sample.Anim
{
    [Activity(Label = "Slide Animation", Theme = "@style/ExampleTheme")]
    public class SlideAnimation : CustomAnimation
    {
        public SlideAnimation()
            : base(Resource.String.anim_slide)
        {
            Transformer = new SlideTransformer();
        }
    }
}