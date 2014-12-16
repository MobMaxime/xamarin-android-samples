
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
using Java.Lang;

namespace SampleProcessButton
{

	public class ProgressGenerator
	{
		Action action;
		Handler messageHandler = new Handler();
		public interface OnCompleteListener {

			void onComplete();
		}

		private OnCompleteListener mListener;
		private int mProgress;

		public ProgressGenerator(OnCompleteListener listener) {
			mListener = listener;
		}
		public void start(ProcessButton button,Activity activity) {
 
			activity.RunOnUiThread (() => {
				action = ()=> UpdateProgress(button,0);
				messageHandler.PostDelayed(action,generateDelay());
			}); 
		}
		private Random random = new Random();

		void UpdateProgress(ProcessButton button,int progress){

			mProgress += 10;
			button.setProgress(mProgress);
			if (mProgress < 100) {
				Console.WriteLine("Progress "+mProgress);
				action = ()=> UpdateProgress(button,mProgress);
				messageHandler.PostDelayed(action, generateDelay());
			} else {
				mListener.onComplete();
				Console.WriteLine("Progress Completed "+mProgress);

			}
		}

		private int generateDelay() {
			return random.Next(1000);
		}
	}
}

