using System;
using Android.Widget;
using Android.Content;
using Java.Util;
using Android.Views;

namespace Calendar_Android
{
	public class CalendarAdapter : BaseAdapter
	{
		int FIRST_DAY_OF_WEEK = 0; // Sunday = 0, Monday = 1


		private Context mContext;

		private Calendar month;
		private Calendar selectedDate;
		private ArrayList items;
		public String[] days;

		public CalendarAdapter(Context c, Calendar monthCalendar) {

			month = monthCalendar;
			selectedDate = (Calendar)monthCalendar.Clone();
			mContext = c;
			month.Set(Calendar.DayOfMonth, 1);
			this.items = new ArrayList();
			refreshDays();
		}

		public void setItems(ArrayList items) {
			for(int i = 0;i != items.Size();i++){
				//if(items.li == 1) {
					items.Set(i, "0" + items.Get(i));
				//}
			}
			this.items = items;
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return null;
		}
		public override long GetItemId (int position)
		{
			return 0;
		}
		public override int Count {
			get {
				return days.Length;
			}
		}
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView;
			TextView dayView;
			if (view == null) // no view to re-use, create new
				view = LayoutInflater.From(mContext).Inflate(Resource.Layout.calendar_item, null);

			dayView = (TextView)view.FindViewById(Resource.Id.date);


			// disable empty days from the beginning
			if(days[position].Equals("")) {
				dayView.Clickable = false;
				dayView.Focusable = false;
			}else {
				// mark current day as focused
				if(month.Get(Calendar.Year)== selectedDate.Get(Calendar.Year) && month.Get(Calendar.Month)== selectedDate.Get(Calendar.Month) && days[position].Equals(""+selectedDate.Get(Calendar.DayOfMonth))) {
					view.SetBackgroundColor (Android.Graphics.Color.Blue);
				}
				else {
					//v.setBackgroundResource(R.drawable.list_item_background);
				}
			}

			dayView.Text = days [position];
			String date = days[position];

			if(date.Length == 1) {
				date = "0"+ date;
			}
			String monthStr = ""+(month.Get(Calendar.Month)+1);
			if(monthStr.Length == 1) {
				monthStr = "0"+monthStr;
			}

			ImageView iw = (ImageView)view.FindViewById(Resource.Id.date_icon);
			if(date.Length>0 && items!=null && items.Contains(date)) {        	
				iw.Visibility = ViewStates.Visible;
			}
			else {
				iw.Visibility = ViewStates.Invisible;
			}

			return view;
		}

		public void refreshDays()
		{
			items.Clear ();

			int lastDay = month.GetActualMaximum (Calendar.DayOfMonth);
			int firstDay = month.Get (Calendar.DayOfWeek);

			// figure size of the array
			if(firstDay==1){
				days = new string[lastDay+(FIRST_DAY_OF_WEEK*6)];
			}
			else {
				days = new String[lastDay+firstDay-(FIRST_DAY_OF_WEEK+1)];
			}
			int j=FIRST_DAY_OF_WEEK;

			// populate empty days before first real day
			if(firstDay>1) {
				for(j=0;j<firstDay-FIRST_DAY_OF_WEEK;j++) {
					days[j] = "";
				}
			}
			else {
				for(j=0;j<FIRST_DAY_OF_WEEK*6;j++) {
					days[j] = "";
				}
				j=FIRST_DAY_OF_WEEK*6+1; // sunday => 1, monday => 7
			}

			// populate days
			int dayNumber = 1;
			for(int i=j-1;i<days.Length;i++) {
				days[i] = ""+dayNumber;
				dayNumber++;
			}
		}
	}
}

