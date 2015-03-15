using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Fragment=Android.Support.V4.App.Fragment;
using Java.Lang;
using Java.Util;

namespace LibrarySlideDateTimePicker
{
	public class DateFragment : Fragment,DatePicker.IOnDateChangedListener
	{
		/**
     * Used to communicate back to the parent fragment as the user
     * is changing the date spinners so we can dynamically update
     * the tab text.
     */
		public interface DateChangedListener
		{
			void onDateChanged(int Year, int Month, int Day);
		}

		DateChangedListener _callback;
		CustomDatePicker _datePicker;

		public DateFragment()
		{
			// Required empty public constructor for fragment.
		}

		/**
     * Cast the reference to {@link SlideDateTimeDialogFragment}
     * to a {@link DateChangedListener}.
     */
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			try
			{
				_callback = (DateChangedListener) TargetFragment;
			}
			catch (ClassCastException e)
			{
				throw new ClassCastException("Calling fragment must implement " +
					"DateFragment.DateChangedListener interface");
			}
		}
		/**
     * Return an instance of DateFragment with its bundle filled with the
     * constructor arguments. The values in the bundle are retrieved in
     * {@link #onCreateView()} below to properly initialize the DatePicker.
     *
     * @param theme
     * @param year
     * @param month
     * @param day
     * @param minDate
     * @param maxDate
     * @return an instance of DateFragment
     */
		public static DateFragment newInstance(int Theme, int Year, int Month,int Day, Date MinDate, Date MaxDate)
		{
			DateFragment dateFragment = new DateFragment();

			Bundle b = new Bundle();
			b.PutInt("theme", Theme);
			b.PutInt("year", Year);
			b.PutInt("month", Month);
			b.PutInt("day", Day);
			b.PutSerializable("minDate", MinDate);
			b.PutSerializable("maxDate", MaxDate);
			dateFragment.Arguments=b;

			return dateFragment;
		}

		/**
     * Create and return the user interface view for this fragment.
     */
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			int theme = Arguments.GetInt("theme");
			int initialYear = Arguments.GetInt("year");
			int initialMonth = Arguments.GetInt("month");
			int initialDay = Arguments.GetInt("day");
			Date minDate = (Date) Arguments.GetSerializable("minDate");
			Date maxDate = (Date) Arguments.GetSerializable("maxDate");

			// Unless we inflate using a cloned inflater with a Holo theme,
			// on Lollipop devices the DatePicker will be the new-style
			// DatePicker, which is not what we want. So we will
			// clone the inflater that we're given but with our specified
			// theme, then inflate the layout with this new inflater.

			Context contextThemeWrapper = new ContextThemeWrapper(Activity,theme == SlideDateTimePicker._holoDark ?Android.Resource.Style.ThemeHolo : Android.Resource.Style.ThemeHoloLight);

			LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

			View view = localInflater.Inflate(Resource.Layout.FragmentDateLayout, container, false);

			_datePicker = view.FindViewById<CustomDatePicker>(Resource.Id.datePicker);
			// block keyboard popping up on touch
			_datePicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
			_datePicker.Init (initialYear, initialMonth, initialDay, this);

			if (minDate != null)
				_datePicker.MinDate=minDate.Time;

			if (maxDate != null)
				_datePicker.MaxDate=maxDate.Time;

			return view;
		}

		void DatePicker.IOnDateChangedListener.OnDateChanged (DatePicker view, int year, int monthOfYear, int dayOfMonth)
		{
			_callback.onDateChanged(year, monthOfYear, dayOfMonth);
		}
	}
}

