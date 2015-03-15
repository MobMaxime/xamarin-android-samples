using Android.Support.V4.App;
using Java.Util;
using Java.Lang;
using Fragment=Android.Support.V4.App.Fragment;


namespace LibrarySlideDateTimePicker
{
	public class SlideDateTimePicker
	{
		public const int _holoDark = 1;
		public const int _holoLight = 2;

		FragmentManager _fragmentManager;
		SlideDateTimeListener _listener;
		Date _initialDate;
		Date _minDate;
		Date _maxDate;
		bool _isClientSpecified24HourTime;
		bool _is24HourTime;
		int _theme;
		int _indicatorColor;

		/**
     * Creates a new instance of {@code SlideDateTimePicker}.
     *
     * @param fm  The {@code FragmentManager} from the calling activity that is used
     *            internally to show the {@code DialogFragment}.
     */
		public SlideDateTimePicker(FragmentManager fm)
		{
			// See if there are any DialogFragments from the FragmentManager
			FragmentTransaction fragmentTransaction = fm.BeginTransaction();
			Fragment fragment = fm.FindFragmentByTag(SlideDateTimeDialogFragment._tagSlideDateTimeDialogFragment);

			// Remove if found
			if (fragment != null)
			{
				fragmentTransaction.Remove(fragment);
				fragmentTransaction.Commit();
			}

			_fragmentManager = fm;
		}

		/**
     * <p>Sets the listener that is used to inform the client when
     * the user selects a new date and time.</p>
     *
     * <p>This must be called before {@link #show()}.</p>
     *
     * @param listener
     */
		public void SetListener(SlideDateTimeListener Listener)
		{
			_listener = Listener;
		}

		/**
     * <p>Sets the initial date and time to display in the date
     * and time pickers.</p>
     *
     * <p>If this method is not called, the current date and time
     * will be displayed.</p>
     *
     * @param initialDate  the {@code Date} object used to determine the
     *                     initial date and time to display
     */
		public void SetInitialDate(Date InitialDate)
		{
			_initialDate = InitialDate;
		}

		/**
     * <p>Sets the minimum date that the DatePicker should show.</p>
     *
     * <p>This must be called before {@link #show()}.</p>
     *
     * @param minDate  the minimum selectable date for the DatePicker
     */
		public void SetMinDate(Date MinDate)
		{
			_minDate = MinDate;
		}

		/**
     * <p>Sets the maximum date that the DatePicker should show.</p>
     *
     * <p>This must be called before {@link #show()}.</p>
     *
     * @param maxDate  the maximum selectable date for the DatePicker
     */
		public void SetMaxDate(Date MaxDate)
		{
			_maxDate = MaxDate;
		}

		private void SetIsClientSpecified24HourTime(bool IsClientSpecified24HourTime)
		{
			_isClientSpecified24HourTime = IsClientSpecified24HourTime;
		}

		/**
     * <p>Sets whether the TimePicker displays its time in 12-hour
     * (AM/PM) or 24-hour format.</p>
     *
     * <p>If this method is not called, the device's default time
     * format is used.</p>
     *
     * <p>This also affects the time displayed in the tab.</p>
     *
     * <p>Must be called before {@link #show()}.</p>
     *
     * @param is24HourTime  <tt>true</tt> to force 24-hour time format,
     *                      <tt>false</tt> to force 12-hour (AM/PM) time
     *                      format.
     */
		public void SetIs24HourTime(bool Is24HourTime)
		{
			SetIsClientSpecified24HourTime(true);
			_is24HourTime = Is24HourTime;
		}

		/**
     * Sets the theme of the dialog. If no theme is specified, it
     * defaults to holo light.
     *
     * @param theme  {@code SlideDateTimePicker.HOLO_DARK} for a dark theme, or
     *               {@code SlideDateTimePicker.HOLO_LIGHT} for a light theme
     */
		public void SetTheme(int Theme)
		{
			_theme = Theme;
		}

		/**
     * Sets the color of the underline for the currently selected tab.
     *
     * @param indicatorColor  the color of the selected tab's underline
     */
		public void SetIndicatorColor(int IndicatorColor)
		{
			_indicatorColor = IndicatorColor;
		}

		/**
     * Shows the dialog to the user. Make sure to call
     * {@link #setListener()} before calling this.
     */
		public void Show()
		{
			if (_listener == null)
			{
				throw new NullPointerException(
					"Attempting to bind null listener to SlideDateTimePicker");
			}

			if (_initialDate == null)
			{
				SetInitialDate(new Date());
			}

			SlideDateTimeDialogFragment dialogFragment =SlideDateTimeDialogFragment.NewInstance(_listener,_initialDate,_minDate,_maxDate,_isClientSpecified24HourTime,_is24HourTime,_theme,_indicatorColor);

			dialogFragment.Show(_fragmentManager,SlideDateTimeDialogFragment._tagSlideDateTimeDialogFragment);
		}

		/*
     * The following implements the builder API to simplify
     * creation and display of the dialog.
     */
		public class Builder
		{
			// Required
			FragmentManager fm;
			SlideDateTimeListener listener;

			// Optional
			Date initialDate;
			Date minDate;
			Date maxDate;
			bool isClientSpecified24HourTime;
			bool is24HourTime;
			int theme;
			int indicatorColor;

			public Builder(FragmentManager fm)
			{
				this.fm = fm;
			}

			/**
         * @see SlideDateTimePicker#setListener(SlideDateTimeListener)
         */
			public Builder SetListener(SlideDateTimeListener Listener)
			{
				this.listener = Listener;
				return this;
			}

			/**
         * @see SlideDateTimePicker#setInitialDate(Date)
         */
			public Builder SetInitialDate(Date InitialDate)
			{
				this.initialDate = InitialDate;
				return this;
			}

			/**
         * @see SlideDateTimePicker#setMinDate(Date)
         */
			public Builder SetMinDate(Date MinDate)
			{
				this.minDate = MinDate;
				return this;
			}

			/**
         * @see SlideDateTimePicker#setMaxDate(Date)
         */
			public Builder SetMaxDate(Date MaxDate)
			{
				this.maxDate = MaxDate;
				return this;
			}

			/**
         * @see SlideDateTimePicker#setIs24HourTime(boolean)
         */
			public Builder SetIs24HourTime(bool Is24HourTime)
			{
				this.isClientSpecified24HourTime = true;
				this.is24HourTime = Is24HourTime;
				return this;
			}

			/**
         * @see SlideDateTimePicker#setTheme(int)
         */
			public Builder SetTheme(int Theme)
			{
				this.theme = Theme;
				return this;
			}

			/**
         * @see SlideDateTimePicker#setIndicatorColor(int)
         */
			public Builder SetIndicatorColor(int IndicatorColor)
			{
				this.indicatorColor = IndicatorColor;
				return this;
			}

			/**
         * <p>Build and return a {@code SlideDateTimePicker} object based on the previously
         * supplied parameters.</p>
         *
         * <p>You should call {@link #show()} immediately after this.</p>
         *
         * @return
         */
			public SlideDateTimePicker Build()
			{
				SlideDateTimePicker picker = new SlideDateTimePicker(fm);
				picker.SetListener(listener);
				picker.SetInitialDate(initialDate);
				picker.SetMinDate(minDate);
				picker.SetMaxDate(maxDate);
				picker.SetIsClientSpecified24HourTime(isClientSpecified24HourTime);
				picker.SetIs24HourTime(is24HourTime);
				picker.SetTheme(theme);
				picker.SetIndicatorColor(indicatorColor);

				return picker;
			}
		}
	}
}

