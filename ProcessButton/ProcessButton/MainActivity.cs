using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SampleProcessButton
{
	[Activity (Label = "SampleProcessButton", MainLauncher = true)]
	public class MainActivity : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			String[] items = Resources.GetStringArray (Resource.Array.sample_list);//getResources().getStringArray(R.array.sample_list);

			ArrayAdapter<String> adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItem1,items);
			ListAdapter = adapter;
		}
		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			base.OnListItemClick (l, v, position, id);
			switch (position) {
			case 0:
				startSignInActivity(false);
				break;
			case 1:
				startSignInActivity(true);
				break;
			case 2:
				startMessageActivity();
				break;
			case 3:
				startUploadActivity();
				break;
			case 4:
				startStateSampleActivity();
				break;
			}
		}
		void startStateSampleActivity() {
			Intent intent = new Intent(this, typeof(StateSampleActivity));
			StartActivity(intent);
		}

		void startUploadActivity() {
			Intent intent = new Intent(this, typeof(UploadActivity));
			StartActivity(intent);
		}

		void startSignInActivity(Boolean isEndlessMode) {
			Intent intent = new Intent(this, typeof(SignInActivity));
			intent.PutExtra(SignInActivity.EXTRAS_ENDLESS_MODE, isEndlessMode);
			StartActivity(intent);
		}

		void startMessageActivity() {
			Intent intent = new Intent(this, typeof(MessageActivity));
			StartActivity(intent);
		}
	}
}


