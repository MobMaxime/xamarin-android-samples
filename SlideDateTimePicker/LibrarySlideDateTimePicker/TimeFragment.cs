using Android.OS;
using Fragment=Android.Support.V4.App.Fragment;
using Android.Widget;
using Java.Lang;
using Android.Content;
using Android.Views;
using Android.Text.Format;

namespace LibrarySlideDateTimePicker
{
	public class TimeFragment : Fragment,TimePicker.IOnTimeChangedListener,NumberPicker.IOnValueChangeListener
	{
		/**
     * Used to communicate back to the parent fragment as the user
     * is changing the time spinners so we can dynamically update
     * the tab text.
     */
		public interface ITimeChangedListener
		{
			void OnTimeChanged(int hour, int minute);
		}

		ITimeChangedListener _callback;
		TimePicker _timePicker;

		public TimeFragment()
		{
			// Required empty public constructor for fragment.
		}

		/**
     * Cast the reference to {@link SlideDateTimeDialogFragment} to a
     * {@link TimeChangedListener}.
     */
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			try
			{
				_callback = (ITimeChangedListener) TargetFragment;
			}
			catch (ClassCastException e)
			{
				throw new ClassCastException("Calling fragment must implement " +
					"TimeFragment.TimeChangedListener interface");
			}
		}
		/**
     * Return an instance of TimeFragment with its bundle filled with the
     * constructor arguments. The values in the bundle are retrieved in
     * {@link #onCreateView()} below to properly initialize the TimePicker.
     *
     * @param theme
     * @param hour
     * @param minute
     * @param isClientSpecified24HourTime
     * @param is24HourTime
     * @return
     */
		public static TimeFragment NewInstance(int Theme, int Hour, int Minute,bool IsClientSpecified24HourTime, bool Is24HourTime)
		{
			TimeFragment timeFragment = new TimeFragment();

			Bundle bundle = new Bundle();
			bundle.PutInt("theme", Theme);
			bundle.PutInt("hour", Hour);
			bundle.PutInt("minute", Minute);
			bundle.PutBoolean("isClientSpecified24HourTime", IsClientSpecified24HourTime);
			bundle.PutBoolean("is24HourTime", Is24HourTime);
			timeFragment.Arguments=bundle;

			return timeFragment;
		}

		/**
     * Create and return the user interface view for this fragment.
     */
		public override Android.Views.View OnCreateView (Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
		{
			int theme = Arguments.GetInt("theme");
			int initialHour = Arguments.GetInt("hour");
			int initialMinute = Arguments.GetInt("minute");
			bool isClientSpecified24HourTime = Arguments.GetBoolean("isClientSpecified24HourTime");
			bool is24HourTime = Arguments.GetBoolean("is24HourTime");

			// Unless we inflate using a cloned inflater with a Holo theme,
			// on Lollipop devices the TimePicker will be the new-style
			// radial TimePicker, which is not what we want. So we will
			// clone the inflater that we're given but with our specified
			// theme, then inflate the layout with this new inflater.

			Context contextThemeWrapper = new ContextThemeWrapper(
				Activity,theme == SlideDateTimePicker._holoDark ?Android.Resource.Style.ThemeHolo : Android.Resource.Style.ThemeHoloLight);

			LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

			View view = localInflater.Inflate(Resource.Layout.FragmentTimeLayout, container, false);

			_timePicker = (TimePicker) view.FindViewById(Resource.Id.timePicker);
			// block keyboard popping up on touch
			_timePicker.DescendantFocusability=DescendantFocusability.BlockDescendants;
			_timePicker.SetOnTimeChangedListener (this);

			// If the client specifies a 24-hour time format, set it on
			// the TimePicker.
			if (isClientSpecified24HourTime)
			{
				_timePicker.SetIs24HourView((Boolean)is24HourTime);
			}
			else
			{
				// If the client does not specify a 24-hour time format, use the
				// device default.
				_timePicker.SetIs24HourView((Boolean)DateFormat.Is24HourFormat(TargetFragment.Activity));
			}

			_timePicker.CurrentHour=(Integer)initialHour;
			_timePicker.CurrentMinute=(Integer)initialMinute;

			// Fix for the bug where a TimePicker's onTimeChanged() is not called when
			// the user toggles the AM/PM button. Only applies to 4.0.0 and 4.0.3.
			if ((int)Build.VERSION.SdkInt >= 14 && (int)Build.VERSION.SdkInt <= 15)
			{
				FixTimePickerBug18982();
			}

			return view;
		}
		/**
     * Workaround for bug in Android TimePicker where the onTimeChanged() callback
     * is not invoked when the user toggles between AM/PM. But we need to be able
     * to detect this in order to dynamically update the tab title properly when
     * the user toggles between AM/PM.
     *
     * Registered as Issue 18982:
     *
     * https://code.google.com/p/android/issues/detail?id=18982
     */
		private void FixTimePickerBug18982()
		{
			View amPmView = ((ViewGroup) _timePicker.GetChildAt(0)).GetChildAt(3);

			if (amPmView is NumberPicker)
			{
				((NumberPicker)amPmView).SetOnValueChangedListener (this);
			}
		}

		void NumberPicker.IOnValueChangeListener.OnValueChange (NumberPicker picker, int oldVal, int newVal)
		{
			if (picker.Value == 1)  // PM
			{
				if ((int)_timePicker.CurrentHour < 12)
					_timePicker.CurrentHour=(Integer)((int)_timePicker.CurrentHour + 12);
			}
			else  // AM
			{
				if ((int)_timePicker.CurrentHour >= 12)
					_timePicker.CurrentHour=(Integer)((int)_timePicker.CurrentHour - 12);
			}

			_callback.OnTimeChanged((int)_timePicker.CurrentHour,(int)_timePicker.CurrentMinute);
		}

		void TimePicker.IOnTimeChangedListener.OnTimeChanged (TimePicker view, int hourOfDay, int minute)
		{
			_callback.OnTimeChanged(hourOfDay, minute);
		}
	}
}

