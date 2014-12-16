
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
using Android.Graphics.Drawables;
using Android.Util;
using Android.Content.Res;
using Java.Lang;
using Android.Graphics;

namespace Library
{
	public abstract class ProcessButton : FlatButton
	{
		public int mProgress;
		private int mMaxProgress;
		private int mMinProgress;

		private GradientDrawable mProgressDrawable;
		private GradientDrawable mCompleteDrawable;
		private GradientDrawable mErrorDrawable;

		private string mLoadingText;
		private string mCompleteText;
		private string mErrorText;

		public ProcessButton(Context context, IAttributeSet attrs, int defStyle):base(context, attrs, defStyle) {
			init(context, attrs);
		}

		public ProcessButton(Context context, IAttributeSet attrs) : base(context, attrs){
			init(context, attrs);
		}

		public ProcessButton(Context context) : base(context){
			init(context, null);
		}

		private void init(Context context, IAttributeSet attrs) {
			mMinProgress = 0;
			mMaxProgress = 100;

			mProgressDrawable = (GradientDrawable) getDrawable(Resource.Drawable.rect_progress).Mutate();
			mProgressDrawable.SetCornerRadius(getCornerRadius());

			mCompleteDrawable = (GradientDrawable) getDrawable(Resource.Drawable.rect_complete).Mutate();
			mCompleteDrawable.SetCornerRadius(getCornerRadius());

			mErrorDrawable = (GradientDrawable) getDrawable(Resource.Drawable.rect_error).Mutate();
			mErrorDrawable.SetCornerRadius(getCornerRadius());

			if (attrs != null) {
				initAttributes(context, attrs);
			}
		}

		private void initAttributes(Context context, IAttributeSet attributeSet) {
			TypedArray attr = getTypedArray(context, attributeSet, Resource.Styleable.ProcessButton);

			if (attr == null) {
				return;
			}

			try {
				mLoadingText = attr.GetString(Resource.Styleable.ProcessButton_pb_textProgress);
				mCompleteText = attr.GetString(Resource.Styleable.ProcessButton_pb_textComplete);
				mErrorText = attr.GetString(Resource.Styleable.ProcessButton_pb_textError);

				int purple = getColor(Resource.Color.purple_progress);
				int colorProgress = attr.GetColor(Resource.Styleable.ProcessButton_pb_colorProgress, purple);
				mProgressDrawable.SetColor(colorProgress);

				int green = getColor(Resource.Color.green_complete);
				int colorComplete = attr.GetColor(Resource.Styleable.ProcessButton_pb_colorComplete, green);
				mCompleteDrawable.SetColor(colorComplete);

				int red = getColor(Resource.Color.red_error);
				int colorError = attr.GetColor(Resource.Styleable.ProcessButton_pb_colorError, red);
				mErrorDrawable.SetColor(colorError);

			} finally {
				attr.Recycle();
			}
		}

		public void setProgress(int progress) {
			mProgress = progress;

			if (mProgress == mMinProgress) {
				onNormalState();
			} else if (mProgress == mMaxProgress) {
				onCompleteState();
			} else if (mProgress < mMinProgress){
				onErrorState();
			} else {
				onProgress();
			}

			Invalidate();
		}

		protected void onErrorState() {
			if(getErrorText() != null) {
				Text = getErrorText ().ToString ();
			}
			setBackgroundCompat(getErrorDrawable());
		}

		protected void onProgress() {
			if(getLoadingText() != null) {
				Text = getLoadingText ().ToString ();
			}
			setBackgroundCompat(getNormalDrawable());
		}

		protected void onCompleteState() {
			if(getCompleteText() != null) {
				Text = getCompleteText ().ToString ();
			}
			setBackgroundCompat(getCompleteDrawable());
		}

		protected void onNormalState() {
			if(getNormalText() != null) {
				Text = getNormalText ().ToString ();
			}
			setBackgroundCompat(getNormalDrawable());
		}
		protected override void OnDraw (Android.Graphics.Canvas canvas)
		{
			if(mProgress > mMinProgress && mProgress < mMaxProgress) {
				drawProgress(canvas);
			}
			base.OnDraw (canvas);
		}
		public abstract void drawProgress(Canvas canvas);

		public int getProgress() {
			return mProgress;
		}

		public int getMaxProgress() {
			return mMaxProgress;
		}

		public int getMinProgress() {
			return mMinProgress;
		}

		public GradientDrawable getProgressDrawable() {
			return mProgressDrawable;
		}

		public GradientDrawable getCompleteDrawable() {
			return mCompleteDrawable;
		}

		public string getLoadingText() {
			return mLoadingText;
		}

		public string getCompleteText() {
			return mCompleteText;
		}

		public void setProgressDrawable(GradientDrawable progressDrawable) {
			mProgressDrawable = progressDrawable;
		}

		public void setCompleteDrawable(GradientDrawable completeDrawable) {
			mCompleteDrawable = completeDrawable;
		}
			
		public void setLoadingText(string loadingText) {
			mLoadingText = loadingText;
		}

		public void setCompleteText(string completeText) {
			mCompleteText = completeText;
		}

		public GradientDrawable getErrorDrawable() {
			return mErrorDrawable;
		}

		public void setErrorDrawable(GradientDrawable errorDrawable) {
			mErrorDrawable = errorDrawable;
		}
			
		public string getErrorText() {
			return mErrorText;
		}

		public void setErrorText(string errorText) {
			mErrorText = errorText;
		}
		public override IParcelable OnSaveInstanceState ()
		{
			IParcelable superState = base.OnSaveInstanceState();
			SavedState savedState = new SavedState(superState);
			savedState.mProgress = mProgress;

			return base.OnSaveInstanceState ();
		}
		public override void OnRestoreInstanceState (IParcelable state)
		{ 
			if (state is SavedState) {
				SavedState savedState = (SavedState) state;
				mProgress = savedState.mProgress;
				base.OnRestoreInstanceState(savedState.SuperState);
				setProgress(mProgress);
			} else {
				base.OnRestoreInstanceState(state);
			}
		}
		/**
     * A {@link android.os.Parcelable} representing the {@link com.dd.processbutton.ProcessButton}'s
     * state.
     */
		public class SavedState : BaseSavedState {

			public int mProgress;

			public SavedState(IParcelable parcel) :base(parcel){
			}

			private SavedState(Parcel ins):base(ins) {
				mProgress = ins.ReadInt();
			}
			public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
			{
				base.WriteToParcel (dest, flags);
				dest.WriteInt(mProgress);
			}
		}
	}
}

