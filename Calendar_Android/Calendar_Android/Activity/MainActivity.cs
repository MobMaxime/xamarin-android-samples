using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Java.Util;
using Java.Lang;



namespace Calendar_Android
{
	[Activity (Label = "Calendar_Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;
		public Calendar month;
		public CalendarAdapter adapter;
		public ArrayList items;
		public Handler handler;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			month = Calendar.Instance;

			//Static date set
			System.String date = "2015-07-31";
			char[] splitchar = { '-' };
			System.String[] dateArr = date.Split(splitchar); // date format is yyyy-mm-dd



			//month.set(Integer.parseInt(dateArr[0]), Integer.parseInt(dateArr[1]), Integer.parseInt(dateArr[2]));
			month.Set(Int16.Parse(dateArr[0]), Int16.Parse(dateArr[1]),Int16.Parse(dateArr[2]));

			items = new ArrayList();
			adapter = new CalendarAdapter(this, month);

			date_date ();

			GridView gridview = (GridView)FindViewById (Resource.Id.gridview);
			gridview.SetAdapter(adapter);
			gridview.ItemClick += grid_click;

			TextView title  = (TextView) FindViewById(Resource.Id.title);
			title.Text = Android.Text.Format.DateFormat.Format("MMMM yyyy",month);

			TextView previous = (TextView) FindViewById (Resource.Id.previous);
			previous.Click += previous_click; 

			TextView next = (TextView) FindViewById (Resource.Id.next);
			next.Click += next_click; 

		}

		public void previous_click(object sender, EventArgs e){
			if(month.Get(Calendar.Month) == month.GetActualMinimum(Calendar.Month)) {				
				month.Set((month.Get(Calendar.Year)-1),month.GetActualMaximum(Calendar.Month),1);
			} else {
				month.Set(Calendar.Month,month.Get(Calendar.Month)-1);
			}
			refreshCalendar();
		}

		public void next_click(object sender, EventArgs e){
			if(month.Get(Calendar.Month)== month.GetActualMaximum(Calendar.Month)) {				
				month.Set((month.Get(Calendar.Year)+1),month.GetActualMinimum(Calendar.Month),1);
			} else {
				month.Set(Calendar.Month,month.Get(Calendar.Month)+1);
			}
			refreshCalendar();
		}

		public void grid_click(object sender, AdapterView.ItemClickEventArgs e){

			TextView date = (TextView)e.View.FindViewById(Resource.Id.date);
			System.String day = date.Text.ToString ();

			if(day.Length == 1) {
				day = "0"+day;
			}
			Toast.MakeText (this, "Date --->" + Android.Text.Format.DateFormat.Format("yyyy-MM", month)+"-"+day, ToastLength.Short).Show ();
		}

		public void refreshCalendar()
		{
			TextView title = (TextView)FindViewById (Resource.Id.title);

			adapter.refreshDays();
			adapter.NotifyDataSetChanged();
			date_date();
			title.Text = Android.Text.Format.DateFormat.Format("MMMM yyyy",month);
		}
		public void date_date(){
			items.Clear ();
			// format random values. You can implement a dedicated class to provide real values
			for(int i=0;i<31;i++) {
				System.Random r = new System.Random();

				int a = r.Next (10);
				if (a > 6) {
					items.Add (i.ToString ());
				} else {
					Console.WriteLine ("Ok");
				}

			}

			adapter.setItems(items);
			adapter.NotifyDataSetChanged ();
		}


	}
}


