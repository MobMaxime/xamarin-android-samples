using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using Java.Text;
using LibrarySlideDateTimePicker;
using Java.Util;
using Fragment=Android.Support.V4.App.Fragment;
namespace SlideDateTimePicker
{
	[Activity (Label = "SlideDateTimePicker", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			Button btnClick = FindViewById<Button>(Resource.Id.btnClick);
			SlideDateTimeListenerImplementation slideDateTimeListenerImplementationtobj = new SlideDateTimeListenerImplementation (this);
			btnClick.Click += (object sender, EventArgs e) => {
				new LibrarySlideDateTimePicker.SlideDateTimePicker.Builder(SupportFragmentManager)
					.SetListener(slideDateTimeListenerImplementationtobj)
					.SetInitialDate(new Date())
					//.setMinDate(minDate)
					//.setMaxDate(maxDate)
					//.setIs24HourTime(true)
					//.setTheme(SlideDateTimePicker.HOLO_DARK)
					//.setIndicatorColor(Color.parseColor("#990000"))
					.Build()
					.Show();
			};
		}
		public class SlideDateTimeListenerImplementation : SlideDateTimeListener
		{
			SimpleDateFormat mFormatter = new SimpleDateFormat("MMMM dd yyyy hh:mm aa");
			MainActivity activity;
			public SlideDateTimeListenerImplementation(MainActivity activity)
			{
				this.activity = activity;
			}
			#region implemented abstract members of SlideDateTimeListener

			public override void onDateTimeSet (Date date)
			{
				Toast.MakeText(activity,mFormatter.Format(date), ToastLength.Short).Show();
			}

			#endregion
			public override void onDateTimeCancel ()
			{
				Toast.MakeText(activity,"Canceled", ToastLength.Short).Show();
			}
		}	
	}
}


