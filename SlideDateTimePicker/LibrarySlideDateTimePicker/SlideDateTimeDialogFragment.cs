using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Util;
using Android.Text.Format;
using Android.Support.V4.App;
using Java.Lang;
using Java.Text;

namespace LibrarySlideDateTimePicker
{
	/**
 * <p>The {@code DialogFragment} that contains the {@link SlidingTabLayout}
 * and {@link CustomViewPager}.</p>
 *
 * <p>The {@code CustomViewPager} contains the {@link DateFragment} and {@link TimeFragment}.</p>
 *
 * <p>This {@code DialogFragment} is managed by {@link SlideDateTimePicker}.</p>
 *
 * @author jjobes
 *
 */
	public class SlideDateTimeDialogFragment : Android.Support.V4.App.DialogFragment,DateFragment.DateChangedListener,TimeFragment.ITimeChangedListener
	{
		public static string _tagSlideDateTimeDialogFragment = "tagSlideDateTimeDialogFragment";

		static SlideDateTimeListener _listener;

		Context _context;
		CustomViewPager _viewPager;
		ViewPagerAdapter _viewPagerAdapter;
		SlidingTabLayout _slidingTabLayout;
		View _buttonHorizontalDivider;
		View _buttonVerticalDivider;
		Button _btnOk;
		Button _btnCancel;
		Date _initialDate;
		static int _theme;
		int _indicatorColor;
		static Date _minDate;
		static Date _maxDate;
		static bool _isClientSpecified24HourTime;
		static bool _is24HourTime;
		static Calendar _calendar;
		FormatStyleFlags _dateFlags =FormatStyleFlags.ShowWeekday | FormatStyleFlags.ShowDate | FormatStyleFlags.AbbrevAll;

		public SlideDateTimeDialogFragment()
		{
			// Required empty public constructor
		}

		/**
     * <p>Return a new instance of {@code SlideDateTimeDialogFragment} with its bundle
     * filled with the incoming arguments.</p>
     *
     * <p>Called by {@link SlideDateTimePicker#show()}.</p>
     *
     * @param listener
     * @param initialDate
     * @param minDate
     * @param maxDate
     * @param isClientSpecified24HourTime
     * @param is24HourTime
     * @param theme
     * @param indicatorColor
     * @return
     */
		public static SlideDateTimeDialogFragment NewInstance(SlideDateTimeListener Listener,Date InitialDate, Date MinDate, Date MaxDate, bool IsClientSpecified24HourTime,bool Is24HourTime, int Theme, int IndicatorColor)
		{
			_listener = Listener;

			// Create a new instance of SlideDateTimeDialogFragment
			SlideDateTimeDialogFragment dialogFragment = new SlideDateTimeDialogFragment();

			// Store the arguments and attach the bundle to the fragment
			Bundle bundle = new Bundle();
			bundle.PutSerializable("initialDate", InitialDate);
			bundle.PutSerializable("minDate", MinDate);
			bundle.PutSerializable("maxDate", MaxDate);
			bundle.PutBoolean("isClientSpecified24HourTime", IsClientSpecified24HourTime);
			bundle.PutBoolean("is24HourTime", Is24HourTime);
			bundle.PutInt("theme", Theme);
			bundle.PutInt("indicatorColor", IndicatorColor);
			dialogFragment.Arguments=bundle;

			// Return the fragment with its bundle
			return dialogFragment;
		}
		public override void OnAttach (Activity activity)
		{
			base.OnAttach (activity);
			_context = activity;
		}
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RetainInstance = true;

			UnpackBundle();
			_calendar = Calendar.Instance;
			_calendar.Time=_initialDate;

			switch (_theme)
			{
			case SlideDateTimePicker._holoDark:
				SetStyle((int)DialogFragmentStyle.NoTitle, Android.Resource.Style.ThemeHoloDialogNoActionBar);
				break;
			case SlideDateTimePicker._holoLight:
				SetStyle((int)DialogFragmentStyle.NoTitle, Android.Resource.Style.ThemeHoloDialogNoActionBar);
				break;
			default:  // if no theme was specified, default to holo light
				SetStyle ((int)DialogFragmentStyle.NoTitle, Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
				break;
			}
		}
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.SlideDateTimePickerLayout, container);

			SetupViews(view);
			CustomizeViews();
			InitViewPager();
			InitTabs();
			InitButtons();

			return view;
		}
		public override void OnDestroyView ()
		{
			// Workaround for a bug in the compatibility library where calling
			// setRetainInstance(true) does not retain the instance across
			// orientation changes.
			if (Dialog != null && RetainInstance)
			{
				Dialog.SetDismissMessage(null);
			}

			base.OnDestroyView ();
		}
		private void UnpackBundle()
		{
			Bundle args = Arguments;

			_initialDate = (Date) args.GetSerializable("initialDate");
			_minDate = (Date) args.GetSerializable("minDate");
			_maxDate = (Date) args.GetSerializable("maxDate");
			_isClientSpecified24HourTime = args.GetBoolean("isClientSpecified24HourTime");
			_is24HourTime = args.GetBoolean("is24HourTime");
			_theme = args.GetInt("theme");
			_indicatorColor = args.GetInt("indicatorColor");
		}

		private void SetupViews(View v)
		{
			_viewPager = v.FindViewById<CustomViewPager>(Resource.Id.viewPager);
			_slidingTabLayout = v.FindViewById<SlidingTabLayout>(Resource.Id.slidingTabLayout);
			_buttonHorizontalDivider = v.FindViewById(Resource.Id.buttonHorizontalDivider);
			_buttonVerticalDivider = v.FindViewById(Resource.Id.buttonVerticalDivider);
			_btnOk = v.FindViewById<Button>(Resource.Id.btnOk);
			_btnCancel = v.FindViewById<Button>(Resource.Id.btnCancel);
		}

		private void CustomizeViews()
		{
			int lineColor = _theme == SlideDateTimePicker._holoDark ?
				Resources.GetColor(Resource.Color.gray_holo_dark) :
				Resources.GetColor(Resource.Color.gray_holo_light);

			// Set the colors of the horizontal and vertical lines for the
			// bottom buttons depending on the theme.
			switch (_theme)
			{
			case SlideDateTimePicker._holoLight:
			case SlideDateTimePicker._holoDark:
				_buttonHorizontalDivider.SetBackgroundColor (Android.Graphics.Color.Blue);
				_buttonVerticalDivider.SetBackgroundColor(Android.Graphics.Color.Blue);
				break;

			default:  // if no theme was specified, default to holo light
				_buttonHorizontalDivider.SetBackgroundColor (Resources.GetColor (Resource.Color.gray_holo_light));
				_buttonVerticalDivider.SetBackgroundColor (Resources.GetColor (Resource.Color.gray_holo_light));
				break;
			}

			// Set the color of the selected tab underline if one was specified.
			if (_indicatorColor != 0)
				_slidingTabLayout.SetSelectedIndicatorColors(_indicatorColor);
		}

		void InitViewPager()
		{
			_viewPagerAdapter = new ViewPagerAdapter(this,ChildFragmentManager);
			_viewPager.Adapter=_viewPagerAdapter;

			// Setting this custom layout for each tab ensures that the tabs will
			// fill all available horizontal space.
			_slidingTabLayout.SetCustomTabView(Resource.Layout.CustomTabLayout, Resource.Id.txtTab);
			_slidingTabLayout.SetViewPager(_viewPager);
		}

		private void InitTabs()
		{
			// Set intial date on date tab
			UpdateDateTab();

			// Set initial time on time tab
			UpdateTimeTab();
		}
		private void InitButtons()
		{
			_btnOk.Click += (object sender, EventArgs e) => {
				if (_listener == null)
				{
					throw new NullPointerException("Listener no longer exists for mOkButton");
				}

				_listener.onDateTimeSet(new Date(_calendar.TimeInMillis));

				Dismiss();
			};
			_btnCancel.Click += (object sender, EventArgs e) => {
				if (_listener == null)
				{
					throw new NullPointerException(
						"Listener no longer exists for mCancelButton");
				}

				_listener.onDateTimeCancel();

				Dismiss();
			};
		}
		/**
     * <p>The callback used by the DatePicker to update {@code mCalendar} as
     * the user changes the date. Each time this is called, we also update
     * the text on the date tab to reflect the date the user has currenly
     * selected.</p>
     *
     * <p>Implements the {@link DateFragment.DateChangedListener}
     * interface.</p>
     */
		#region DateChangedListener implementation

		void DateFragment.DateChangedListener.onDateChanged (int year, int month, int day)
		{
			_calendar.Set(year, month, day);

			UpdateDateTab();
		}

		#endregion

		/**
		* <p>The callback used by the TimePicker to update {@code mCalendar} as
		* the user changes the time. Each time this is called, we also update
		* the text on the time tab to reflect the time the user has currenly
		* selected.</p>
		*
		* <p>Implements the {@link TimeFragment.TimeChangedListener}
		* interface.</p>
		*/
		#region TimeChangedListener implementation

		void TimeFragment.ITimeChangedListener.OnTimeChanged (int hour, int minute)
		{
			_calendar.Set(CalendarField.HourOfDay, hour);
			_calendar.Set(CalendarField.Minute, minute);

			UpdateTimeTab();
		}

		#endregion
		void UpdateDateTab()
		{
			_slidingTabLayout.SetTabText(0, DateUtils.FormatDateTime(_context, _calendar.TimeInMillis, _dateFlags));
		}

//		@SuppressLint("SimpleDateFormat")
		private void UpdateTimeTab()
		{
			if (_isClientSpecified24HourTime)
			{
				SimpleDateFormat formatter;

				if (_is24HourTime)
				{
					formatter = new SimpleDateFormat("HH:mm");
					_slidingTabLayout.SetTabText(1, formatter.Format(_calendar.Time));
				}
				else
				{
					formatter = new SimpleDateFormat("h:mm aa");
					_slidingTabLayout.SetTabText (1, formatter.Format (_calendar.Time));
				}
			}
			else  // display time using the device's default 12/24 hour format preference
			{
				_slidingTabLayout.SetTabText(1, Android.Text.Format.DateFormat.GetTimeFormat(_context).Format(_calendar.TimeInMillis));
			}
		}

		/**
     * <p>Called when the user clicks outside the dialog or presses the <b>Back</b>
     * button.</p>
     *
     * <p><b>Note:</b> Actual <b>Cancel</b> button clicks are handled by {@code mCancelButton}'s
     * event handler.</p>
     */
		public override void OnCancel (IDialogInterface dialog)
		{
			base.OnCancel (dialog);
			if (_listener == null)
			{
				throw new NullPointerException(
					"Listener no longer exists in onCancel()");
			}

			_listener.onDateTimeCancel();
		}
		class ViewPagerAdapter : FragmentPagerAdapter
		{
			SlideDateTimeDialogFragment slideDateTimeDialogFragment;
			public ViewPagerAdapter(SlideDateTimeDialogFragment slideDateTimeDialogFragment,Android.Support.V4.App.FragmentManager fm):base(fm)
			{
				this.slideDateTimeDialogFragment=slideDateTimeDialogFragment;
			}
			public override Android.Support.V4.App.Fragment GetItem (int position)
			{
				switch (position)
				{
				case 0:
					DateFragment dateFragment = DateFragment.newInstance(
						_theme,
						_calendar.Get(CalendarField.Year),
						_calendar.Get(CalendarField.Month),
						_calendar.Get(CalendarField.DayOfMonth),
						_minDate,
						_maxDate);
					dateFragment.SetTargetFragment(slideDateTimeDialogFragment, 100);
					return dateFragment;
				case 1:
					TimeFragment timeFragment = TimeFragment.NewInstance(
						_theme,
						_calendar.Get(CalendarField.HourOfDay),
						_calendar.Get(CalendarField.Minute),
						_isClientSpecified24HourTime,
						_is24HourTime);
					timeFragment.SetTargetFragment(slideDateTimeDialogFragment, 200);
					return timeFragment;
				default:
					return null;
				}
			}
			public override int Count {
				get {
					return 2;
				}
			}
		}
	}
}

