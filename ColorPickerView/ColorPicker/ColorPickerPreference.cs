
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
using Android.Graphics;
using Android.Util;
using Java.Lang;
using Android.Content.Res;

namespace ColorPicker
{
	public class ColorPickerPreference : DialogPreference,ColorPickerView.OnColorChangedListener
	{
		private ColorPickerView				mColorPickerView;
		private ColorPanelView				mOldColorView;
		private ColorPanelView				mNewColorView;

		private int							mColor;

		private System.Boolean						alphaChannelVisible = false;
		private string						alphaChannelText = null;
		private System.Boolean						showDialogTitle = false;
		private System.Boolean						showPreviewSelectedColorInList = true;
		private int							colorPickerSliderColor = -1;
		private int							colorPickerBorderColor = -1;


		public ColorPickerPreference(Context context, IAttributeSet attrs) : base(context, attrs){
			init(attrs);
		}

		public ColorPickerPreference(Context context, IAttributeSet attrs, int defStyle) :base(context, attrs, defStyle){
			init(attrs); 
		} 

		private void init(IAttributeSet attrs) {
			TypedArray a = Context.ObtainStyledAttributes (attrs, Resource.Styleable.ColorPickerPreference);

			showDialogTitle = a.GetBoolean(Resource.Styleable.ColorPickerPreference_showDialogTitle, false);
			showPreviewSelectedColorInList = a.GetBoolean(Resource.Styleable.ColorPickerPreference_showSelectedColorInList, true);

			a.Recycle();	
			a = Context.ObtainStyledAttributes(attrs, Resource.Styleable.ColorPickerView);

			alphaChannelVisible = a.GetBoolean(Resource.Styleable.ColorPickerView_alphaChannelVisible, false);
			alphaChannelText = a.GetString(Resource.Styleable.ColorPickerView_alphaChannelText);		
			colorPickerSliderColor = a.GetColor(Resource.Styleable.ColorPickerView_colorPickerSliderColor, -1);
			colorPickerBorderColor = a.GetColor(Resource.Styleable.ColorPickerView_colorPickerBorderColor, -1);

			a.Recycle();

			if(showPreviewSelectedColorInList) {

				WidgetLayoutResource = Resource.Layout.preference_preview_layout;
			}

			if(!showDialogTitle) {
				SetDialogTitle(Resource.String.dialog_title);
			}

			DialogLayoutResource = Resource.Layout.dialog_color_picker;
			   

			SetPositiveButtonText(Resource.String.dialog_ok);
			SetNegativeButtonText(Resource.String.dialog_cancle);		

			Persistent = true;
			 
		}

		protected override IParcelable OnSaveInstanceState ()
		{
			IParcelable superState = base.OnSaveInstanceState();

			// Create instance of custom BaseSavedState
			SavedState myState = new SavedState(superState);
			// Set the state's value with the class member that holds current setting value


			if(Dialog != null && mColorPickerView != null) {
				myState.currentColor = mColorPickerView.getColor();
			}
			else {
				myState.currentColor = 0;
			}

			return myState;
 		}
		 
		protected override void OnRestoreInstanceState (IParcelable state)
		{
			base.OnRestoreInstanceState (state);

			if (state == null || !(state.GetType().Equals(new SavedState(state)))) {
				// Didn't save state for us in onSaveInstanceState
				base.OnRestoreInstanceState (state);
				return;
			}

			SavedState myState = (SavedState) state;
			base.OnRestoreInstanceState(myState.SuperState);
//			showDialog(myState.dialogBundle);
			   

			// Set this Preference's widget to reflect the restored state
			if(Dialog != null && mColorPickerView != null) {
//				Log.d("mColorPicker", "Restoring color!");	    	
				mColorPickerView.setColor(myState.currentColor, true);
			}

		}

		protected override void OnBindView (View view)
		{
			base.OnBindView (view);

			ColorPanelView preview = (ColorPanelView) view.FindViewById(Resource.Id.preference_preview_color_panel);

			if(preview != null) {
				preview.setColor(mColor);
			}
		}


		protected override void OnBindDialogView (View view)
		{
			base.OnBindDialogView (view);

			System.Boolean isLandscapeLayout = false;

			mColorPickerView = (ColorPickerView)view.FindViewById(Resource.Id.color_picker_view);

			LinearLayout landscapeLayout = (LinearLayout) view.FindViewById(Resource.Id.dialog_color_picker_extra_layout_landscape);

			if(landscapeLayout != null) {
				isLandscapeLayout = true;
			}


			mColorPickerView = (ColorPickerView) view.FindViewById(Resource.Id.color_picker_view);
			mOldColorView = (ColorPanelView) view.FindViewById(Resource.Id.color_panel_old);
			mNewColorView = (ColorPanelView) view.FindViewById(Resource.Id.color_panel_new);

			if(!isLandscapeLayout) {
				((LinearLayout) mOldColorView.Parent).SetPadding(
					(int)System.Math.Round(mColorPickerView.getDrawingOffset()), 
					0, 
					(int)System.Math.Round(mColorPickerView.getDrawingOffset()), 
					0);

			}
			else {
				landscapeLayout.SetPadding(0, 0, (int)System.Math.Round(mColorPickerView.getDrawingOffset()), 0);
			}

			mColorPickerView.setAlphaSliderVisible(alphaChannelVisible);
			mColorPickerView.setAlphaSliderText(alphaChannelText);		
			mColorPickerView.setSliderTrackerColor(colorPickerSliderColor);

			if(colorPickerSliderColor != -1) {
				mColorPickerView.setSliderTrackerColor(colorPickerSliderColor);
			}

			if(colorPickerBorderColor != -1) {
				mColorPickerView.setBorderColor(colorPickerBorderColor);
			}


			mColorPickerView.setOnColorChangedListener(this);

			//Log.d("mColorPicker", "setting initial color!");
			mOldColorView.setColor(mColor);
			mColorPickerView.setColor(mColor, true);
		}
		 

		protected override void OnDialogClosed (bool positiveResult)
		{
			//base.OnDialogClosed (positiveResult);

			if(positiveResult) {
				mColor = mColorPickerView.getColor();
				PersistInt(mColor);

				NotifyChanged();

			}
		}
		 
		protected override void OnSetInitialValue (bool restorePersistedValue, Java.Lang.Object defaultValue)
		{
 
			if(restorePersistedValue) {
				mColor = GetPersistedInt (Int32.Parse("FF000000", System.Globalization.NumberStyles.HexNumber));// getPersistedInt(0xFF000000);
				Console.WriteLine("mColorPicker", "Load saved color: " + mColor);
			}
			else {
				mColor = (int)defaultValue;
				PersistInt(mColor);
			}
		}
	 
		protected override Java.Lang.Object OnGetDefaultValue (TypedArray a, int index)
		{
			return base.OnGetDefaultValue (a, index);
		}
	     
		public void onColorChanged(int newColor) {
			mNewColorView.setColor(newColor);
		}

		class SavedState : BaseSavedState {
			public int currentColor;

			public SavedState(IParcelable superState) : base(superState) {
				 
			}

			public SavedState(Parcel source) : base(source) {
				currentColor = source.ReadInt(); 
			}

			public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
			{
				base.WriteToParcel (dest, flags);
				dest.WriteBundle((Bundle)currentColor);
			}

			//			public static ParcelableCreator CREATOR = new ParcelableCreator(); 

		} 
	}
}

