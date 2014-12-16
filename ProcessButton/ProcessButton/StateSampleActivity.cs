
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
	[Activity (Label = "StateSampleActivity")]			
	public class StateSampleActivity : Activity,View.IOnClickListener
	{
		private ActionProcessButton mBtnAction;
		private GenerateProcessButton mBtnGenerate;
		private SubmitProcessButton mBtnSubmit;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView(Resource.Layout.ac_states);

			mBtnAction = FindViewById<ActionProcessButton>(Resource.Id.btnAction);
			mBtnSubmit = FindViewById<SubmitProcessButton>(Resource.Id.btnSubmit);
			mBtnGenerate = FindViewById<GenerateProcessButton>(Resource.Id.btnGenerate);

			FindViewById(Resource.Id.btnProgressLoading).SetOnClickListener(this);
			FindViewById(Resource.Id.btnProgressError).SetOnClickListener(this);
			FindViewById(Resource.Id.btnProgressComplete).SetOnClickListener(this);
			FindViewById(Resource.Id.btnProgressNormal).SetOnClickListener(this);
		}

		void View.IOnClickListener.OnClick (View v)
		{
			switch (v.Id) {
			case Resource.Id.btnProgressLoading:
				mBtnAction.setProgress(50);
				mBtnSubmit.setProgress(50);
				mBtnGenerate.setProgress(50);
				break;
			case Resource.Id.btnProgressError:
				mBtnAction.setProgress(-1);
				mBtnSubmit.setProgress(-1);
				mBtnGenerate.setProgress(-1);
				break;
			case Resource.Id.btnProgressComplete:
				mBtnAction.setProgress(100);
				mBtnSubmit.setProgress(100);
				mBtnGenerate.setProgress(100);
				break;
			case Resource.Id.btnProgressNormal:
				mBtnAction.setProgress(0);
				mBtnSubmit.setProgress(0);
				mBtnGenerate.setProgress(0);
				break;
			}
		}
	}
}

