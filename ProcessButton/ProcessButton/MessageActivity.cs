
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
	[Activity (Label = "MessageActivity")]			
	public class MessageActivity : Activity,ProgressGenerator.OnCompleteListener
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView(Resource.Layout.ac_message);

			EditText editMessage =FindViewById<EditText>(Resource.Id.editMessage);

			ProgressGenerator progressGenerator = new ProgressGenerator(this);
			SubmitProcessButton btnSend =FindViewById<SubmitProcessButton>(Resource.Id.btnSend);
			btnSend.Click += (object sender, EventArgs e) => {
				progressGenerator.start(btnSend,this);
				btnSend.Enabled=false;
				editMessage.Enabled=false;
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

