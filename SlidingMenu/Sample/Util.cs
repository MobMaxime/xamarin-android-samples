using Android.Content;

namespace Sample
{
    public class Util
    {
        public static void GoToGitHub(Context context)
        {
            var uriUrl = Android.Net.Uri.Parse("http://github.com/Cheesebaron/SlidingMenuSharp");
            var launchBrowser = new Intent(Intent.ActionView, uriUrl);
            context.StartActivity(launchBrowser);
        }
    }
}