using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Java.Lang.Reflect;
using Java.Lang;

namespace LibrarySlideDateTimePicker
{
	public class CustomDatePicker : DatePicker
	{
		public CustomDatePicker(Context context, IAttributeSet Attrs):base(context, Attrs)
		{
			Class _idClass = null;
			Class _numberPickerClass = null;
			Field _selectionDividerField = null;
			Field _monthField = null;
			Field _dayField = null;
			Field _yearField = null;
			NumberPicker _monthNumberPicker = null;
			NumberPicker _dayNumberPicker = null;
			NumberPicker _yearNumberPicker = null;

			try
			{
				// Create an instance of the id class
				_idClass = Class.ForName("com.android.internal.R$id");

				// Get the fields that store the resource IDs for the month, day and year NumberPickers
				_monthField = _idClass.GetField("month");
				_dayField = _idClass.GetField("day");
				_yearField = _idClass.GetField("year");

				// Use the resource IDs to get references to the month, day and year NumberPickers
				_monthNumberPicker = (NumberPicker) FindViewById(_monthField.GetInt(null));
				_dayNumberPicker = (NumberPicker) FindViewById(_dayField.GetInt(null));
				_yearNumberPicker = (NumberPicker) FindViewById(_yearField.GetInt(null));

				_numberPickerClass = Class.ForName("android.widget.NumberPicker");

				// Set the value of the mSelectionDivider field in the month, day and year NumberPickers
				// to refer to our custom drawables
				_selectionDividerField = _numberPickerClass.GetDeclaredField("mSelectionDivider");
				_selectionDividerField.Accessible=true;
				_selectionDividerField.Set(_monthNumberPicker, Resources.GetDrawable(Resource.Drawable.selection_divider));
				_selectionDividerField.Set(_dayNumberPicker, Resources.GetDrawable(Resource.Drawable.selection_divider));
				_selectionDividerField.Set(_yearNumberPicker, Resources.GetDrawable(Resource.Drawable.selection_divider));
			}
			catch (ClassNotFoundException e)
			{
				Log.Error("CustomDatePicker", "ClassNotFoundException in CustomDatePicker", e);
			}
			catch (NoSuchFieldException e)
			{
				Log.Error("CustomDatePicker", "NoSuchFieldException in CustomDatePicker", e);
			}
			catch (IllegalAccessException e)
			{
				Log.Error("CustomDatePicker", "IllegalAccessException in CustomDatePicker", e);
			}
			catch (IllegalArgumentException e)
			{
				Log.Error("CustomDatePicker", "IllegalArgumentException in CustomDatePicker", e);
			}
		}
	}
}

