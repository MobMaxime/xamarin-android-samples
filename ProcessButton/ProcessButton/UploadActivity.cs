
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Library;

namespace SampleProcessButton
{
	[Activity (Label = "UploadActivity")]			
	public class UploadActivity : Activity,ProgressGenerator.OnCompleteListener
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView(Resource.Layout.ac_upload);

			ProgressGenerator progressGenerator = new ProgressGenerator(this);
			GenerateProcessButton btnUpload =FindViewById<GenerateProcessButton>(Resource.Id.btnUpload);
			btnUpload.Click += (object sender, EventArgs e) => {
				progressGenerator.start(btnUpload,this);
				btnUpload.Enabled=false;
			};
		}

		#region OnCompleteListener implementation

		void ProgressGenerator.OnCompleteListener.onComplete ()
		{
			Toast.MakeText(this, Resource.String.Loading_Complete, ToastLength.Long).Show();
		}

		#endregion
	}
}

