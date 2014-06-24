
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
using Android.Preferences;
using ColorPicker;

namespace ColorPickerViewDemo
{
	[Activity (Label = "MainActivity",MainLauncher = true)]			
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main, menu);

			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{


			switch(item.ItemId) {
			case Resource.Id.menu_color_picker_dialog:
				onClickColorPickerDialog(item);
				return true; 
			}


			return base.OnOptionsItemSelected (item);
		}
		  
		public void onClickColorPickerDialog(IMenuItem item) {
			//The color picker menu item as been clicked. Show 
			//a dialog using the custom ColorPickerDialog class.

			var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
			int initialValue = prefs.GetInt("color_2", Int32.Parse("FF000000", System.Globalization.NumberStyles.HexNumber));

			Console.WriteLine("mColorPicker", "initial value:" + initialValue);

			ColorPickerDialog colorDialog = new ColorPickerDialog(this, initialValue);

			colorDialog.setAlphaSliderVisible(true);
			colorDialog.SetTitle("Pick a Color!");

//			colorDialog.setButton(DialogInterface.BUTTON_POSITIVE, getString(android.R.string.ok), new DialogInterface.OnClickListener() {
//
//				@Override
//				public void onClick(DialogInterface dialog, int which) {
//					Toast.makeText(MainActivity.this, "Selected Color: " + colorToHexString(colorDialog.getColor()), Toast.LENGTH_LONG).show();
//
//					//Save the value in our preferences.
//					SharedPreferences.Editor editor = prefs.edit();
//					editor.putInt("color_2", colorDialog.getColor());
//					editor.commit();
//				}
//			});
//
//			colorDialog.setButton(DialogInterface.BUTTON_NEGATIVE, getString(android.R.string.cancel), new DialogInterface.OnClickListener() {
//
//				@Override
//				public void onClick(DialogInterface dialog, int which) {
//					//Nothing to do here.
//				}
//			});

			colorDialog.Show();
		}


		private String colorToHexString(int color) {
			return String.Format("#%06X", 0xFFFFFFFF & color);
		}
	}
}

