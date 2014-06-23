using Android.OS;
using SlidingMenuSharp;

namespace Sample.Anim
{
    public abstract class CustomAnimation : BaseActivity
    {
        public ICanvasTransformer Transformer { get; set; }

        protected CustomAnimation(int titleRes)
            : base(titleRes)
        { }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.content_frame);
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.content_frame, new SampleListFragment())
                .Commit();

            SlidingMenu.BehindScrollScale = 0.0f;
            SlidingMenu.BehindCanvasTransformer = Transformer;
        }
    }
}