using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Java.Lang;
using Java.Lang.Reflect;

namespace LibrarySlideDateTimePicker
{
	public class CustomTimePicker : TimePicker
	{
		public CustomTimePicker(Context context, IAttributeSet Attrs):base(context, Attrs)
		{
			Class _idClass = null;
			Class _numberPickerClass = null;
			Field _selectionDividerField = null;
			Field _hourField = null;
			Field _minuteField = null;
			Field _amPmField = null;
			NumberPicker _hourNumberPicker = null;
			NumberPicker _minuteNumberPicker = null;
			NumberPicker _amPmNumberPicker = null;

			try
			{
				// Create an instance of the id class
				_idClass = Class.ForName("com.android.internal.R$id");

				// Get the fields that store the resource IDs for the hour, minute and amPm NumberPickers
				_hourField = _idClass.GetField("hour");
				_minuteField = _idClass.GetField("minute");
				_amPmField = _idClass.GetField("amPm");

				// Use the resource IDs to get references to the hour, minute and amPm NumberPickers
				_hourNumberPicker = (NumberPicker) FindViewById(_hourField.GetInt(null));
				_minuteNumberPicker = (NumberPicker) FindViewById(_minuteField.GetInt(null));
				_amPmNumberPicker = (NumberPicker) FindViewById(_amPmField.GetInt(null));

				_numberPickerClass = Class.ForName("android.widget.NumberPicker");

				// Set the value of the mSelectionDivider field in the hour, minute and amPm NumberPickers
				// to refer to our custom drawables
				_selectionDividerField = _numberPickerClass.GetDeclaredField("mSelectionDivider");
				_selectionDividerField.Accessible=true;
				_selectionDividerField.Set(_hourNumberPicker, Resources.GetDrawable(Resource.Drawable.selection_divider));
				_selectionDividerField.Set(_minuteNumberPicker, Resources.GetDrawable(Resource.Drawable.selection_divider));
				_selectionDividerField.Set(_amPmNumberPicker, Resources.GetDrawable(Resource.Drawable.selection_divider));
			}
			catch (ClassNotFoundException e)
			{
				Log.Error("CustomTimePicker", "ClassNotFoundException in CustomTimePicker", e);
			}
			catch (NoSuchFieldException e)
			{
				Log.Error("CustomTimePicker", "NoSuchFieldException in CustomTimePicker", e);
			}
			catch (IllegalAccessException e)
			{
				Log.Error("CustomTimePicker", "IllegalAccessException in CustomTimePicker", e);
			}
			catch (IllegalArgumentException e)
			{
				Log.Error("CustomTimePicker", "IllegalArgumentException in CustomTimePicker", e);
			}
		}
	}
}

