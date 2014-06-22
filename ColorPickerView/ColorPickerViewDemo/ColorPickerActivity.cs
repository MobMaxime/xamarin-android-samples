
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
using ColorPicker;
using Android.Preferences;

namespace ColorPickerViewDemo
{
	[Activity (Label = "ColorPickerActivity",MainLauncher = true)]			
	public class ColorPickerActivity : Activity,ColorPicker.ColorPickerView.OnColorChangedListener
	{
		private ColorPickerView			mColorPickerView;
		private ColorPanelView			mOldColorPanelView;
		private ColorPanelView			mNewColorPanelView;

		private Button					mOkButton;
		private Button					mCancelButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Window.SetFormat (Android.Graphics.Format.Rgba8888);

			SetContentView(Resource.Layout.Main);

			init();
		}

		private void init() {

			var prefs = PreferenceManager.GetDefaultSharedPreferences (this);
			int initialColor = prefs.GetInt("color_3", Convert.ToInt32("4278190080"));

			mColorPickerView = (ColorPickerView) FindViewById(Resource.Id.color_picker_view);
			mOldColorPanelView = (ColorPanelView) FindViewById(Resource.Id.color_panel_old);
			mNewColorPanelView = (ColorPanelView) FindViewById(Resource.Id.color_panel_new);

			mOkButton = (Button) FindViewById(Resource.Id.okButton);
			mCancelButton = (Button) FindViewById(Resource.Id.cancelButton);


			((LinearLayout) mOldColorPanelView.Parent).SetPadding(
				(int)Math.Round(mColorPickerView.getDrawingOffset()), 0, 
				(int)Math.Round(mColorPickerView.getDrawingOffset()), 0);


			mColorPickerView.setOnColorChangedListener(this);
			mColorPickerView.setColor(initialColor, true);
			mOldColorPanelView.setColor(initialColor);

			mOkButton.Click += (sender, e) => {
				var edit = PreferenceManager.GetDefaultSharedPreferences(this).Edit();
				edit.PutInt("color_3", mColorPickerView.getColor());
				edit.Commit();

				Finish();	
			};
			mCancelButton.Click += (sender, e) => {
				Finish();
			};

		}

		public void onColorChanged(int newColor) {
			mNewColorPanelView.setColor(mColorPickerView.getColor());		
		}
		 
	}
}

