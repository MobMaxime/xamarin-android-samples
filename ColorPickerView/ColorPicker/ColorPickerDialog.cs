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

namespace ColorPicker
{
	public class ColorPickerDialog : AlertDialog,ColorPickerView.OnColorChangedListener
	{
		private ColorPickerView mColorPicker;

		private ColorPanelView mOldColor;
		private ColorPanelView mNewColor;
		 
		private ColorPickerView.OnColorChangedListener mListener;

		public ColorPickerDialog(Context context, int initialColor) : base(context,initialColor) {
			mListener = null;
			init(initialColor);
		}

		public ColorPickerDialog(Context context, int initialColor, ColorPickerView.OnColorChangedListener listener) : base(context) {
			mListener = listener;
			init(initialColor);
		}


		private void init(int color) {
			// To fight color branding.
			Window.SetFormat(Android.Graphics.Format.Rgb888);
			setUp(color);
		}

		private void setUp(int color) {
			Boolean isLandscapeLayout = false;

			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService (Context.LayoutInflaterService);

			View layout = inflater.Inflate(Resource.Layout.dialog_color_picker, null);

			SetContentView(layout);

			SetTitle("Pick a Color");
			// setIcon(android.R.drawable.ic_dialog_info);

			LinearLayout landscapeLayout = (LinearLayout) layout.FindViewById(Resource.Id.dialog_color_picker_extra_layout_landscape);

			if(landscapeLayout != null) {
				isLandscapeLayout = true;
			}

			mColorPicker = (ColorPickerView) layout.FindViewById(Resource.Id.color_picker_view);
			mOldColor = (ColorPanelView) layout.FindViewById(Resource.Id.color_panel_old);
			mNewColor = (ColorPanelView) layout.FindViewById(Resource.Id.color_panel_new);

			if(!isLandscapeLayout) {
				((LinearLayout) mOldColor.Parent).SetPadding(
					(int)Math.Round(mColorPicker.getDrawingOffset()), 
					0, 
					(int)Math.Round(mColorPicker.getDrawingOffset()), 
					0);

			}
			else {
				landscapeLayout.SetPadding(0, 0,(int) Math.Round(mColorPicker.getDrawingOffset()), 0);
				string temp = null;
				SetTitle(temp);
			}

			mColorPicker.setOnColorChangedListener(this);

			mOldColor.setColor(color);
			mColorPicker.setColor(color, true);

		}
		 
		//TODO : change as per native lib for override
		 
		public void onColorChanged(int color) {
			mNewColor.setColor(color);

			if (mListener != null) {
				mListener.onColorChanged(color);

			}

		}

		public void setAlphaSliderVisible(Boolean visible) {
			mColorPicker.setAlphaSliderVisible(visible);
		}

		public int getColor() {
			return mColorPicker.getColor();
		}
	}
}

