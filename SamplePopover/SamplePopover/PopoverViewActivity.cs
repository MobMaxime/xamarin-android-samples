
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
using Android.Graphics;
using Android.Util;
using PopOver;

namespace SamplePopover
{
	[Activity (Label = "PopoverViewActivity", MainLauncher = true)]			
	public class PopoverViewActivity : Activity ,PopoverView.PopoverViewDelegate,Android.Views.View.IOnClickListener
	{
		PopoverView popoverView;
		ViewGroup main;
		RelativeLayout rootView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Create your application here
			SetContentView (Resource.Layout.Main);

			rootView = (RelativeLayout)FindViewById (Resource.Id.rootLayout);  

			FindViewById (Resource.Id.button1).SetOnClickListener (this);
			FindViewById (Resource.Id.button2).SetOnClickListener (this);
			FindViewById (Resource.Id.btnTest).SetOnClickListener (this);
			FindViewById (Resource.Id.button4).SetOnClickListener (this);
			FindViewById (Resource.Id.button5).SetOnClickListener (this);
			FindViewById (Resource.Id.button6).SetOnClickListener (this);
			FindViewById (Resource.Id.button7).SetOnClickListener (this);
			FindViewById (Resource.Id.button8).SetOnClickListener (this);
			FindViewById (Resource.Id.button9).SetOnClickListener (this);
		}

		public void OnClick (View v)
		{

			//get root layout


			PopoverView popoverView = new PopoverView (this, Resource.Layout.popover_showed_view);
			popoverView.setContentSizeForViewInPopover (new Point (320, 340));

			popoverView.Del = this;
			popoverView.showPopoverFromRectInViewGroup (rootView, PopoverView.getFrameForView (v), PopoverView.PopoverArrowDirectionAny, true);
				
			ViewGroup view = (ViewGroup)popoverView.Superview;

			string str = "";
			if (main == null) {
				main =	view;
				str = "Hello";
			} else {
				view = main;
				str = "Hello1";
			}

			TextView txt = view.FindViewById<TextView> (Resource.Id.textView1);
			txt.Text = str;
			txt.Click += (object sender, EventArgs e) => {
				Console.WriteLine ("Hello");
			};
		}

		public void popoverViewWillShow (PopoverView view)
		{
			Log.Info ("POPOVER", "Will show");
		}


		public void popoverViewDidShow (PopoverView view)
		{
			Log.Info ("POPOVER", "Did show");
		}


		public void popoverViewWillDismiss (PopoverView view)
		{
			Log.Info ("POPOVER", "Will dismiss");
		}


		public  void popoverViewDidDismiss (PopoverView view)
		{
			Log.Info ("POPOVER", "Did dismiss");
		}
	}
}

