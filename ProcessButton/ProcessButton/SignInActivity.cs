
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
	[Activity (Label = "SignInActivity")]			
	public class SignInActivity : Activity,ProgressGenerator.OnCompleteListener
	{
		public static String EXTRAS_ENDLESS_MODE = "EXTRAS_ENDLESS_MODE";
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ac_sign_in);
			EditText editEmail = FindViewById<EditText>(Resource.Id.editEmail);
			EditText editPassword =FindViewById<EditText>(Resource.Id.editPassword);

			ProgressGenerator progressGenerator = new ProgressGenerator(this);
			ActionProcessButton btnSignIn = FindViewById<ActionProcessButton>(Resource.Id.btnSignIn);
			Bundle extras = Intent.Extras;
			if(extras != null && extras.GetBoolean(EXTRAS_ENDLESS_MODE)) {
				btnSignIn.setMode(ActionProcessButton.Mode.ENDLESS);
			} else {
				btnSignIn.setMode(ActionProcessButton.Mode.PROGRESS);
			}
			btnSignIn.Click += (object sender, EventArgs e) => {
				this.RunOnUiThread(()=>{
				progressGenerator.start(btnSignIn,this);
				btnSignIn.Enabled=false;
				editEmail.Enabled=false;
				editPassword.Enabled=false;
				});
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

