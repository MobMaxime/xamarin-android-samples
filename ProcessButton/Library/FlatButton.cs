
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

namespace Library
{
	public class FlatButton:Button
	{
		private StateListDrawable mNormalDrawable;
		private string mNormalText;
		private float cornerRadius;

		public FlatButton(Context context, IAttributeSet attrs, int defStyle):base(context, attrs, defStyle) {
			init(context, attrs);
		}

		public FlatButton(Context context, IAttributeSet attrs):base(context, attrs) {
			init(context, attrs);
		}

		public FlatButton(Context context):base(context) {
			init(context, null);
		}

		private void init(Context context, IAttributeSet attrs) {
			mNormalDrawable = new StateListDrawable();
			if (attrs != null) {
				initAttributes(context, attrs);
			}
			mNormalText = Text.ToString ();
			setBackgroundCompat(mNormalDrawable);
		}
		private void initAttributes(Context context, IAttributeSet attributeSet) {
			TypedArray attr = getTypedArray(context, attributeSet,Resource.Styleable.FlatButton);
			if(attr == null) {
				return;
			}

			try {
				float defValue = Resources.GetDimension(Resource.Dimension.corner_radius);
				cornerRadius = attr.GetDimension(Resource.Styleable.FlatButton_pb_cornerRadius, defValue);

				mNormalDrawable.AddState(new int[]{Android.Resource.Attribute.StatePressed},
					createPressedDrawable(attr));
				mNormalDrawable.AddState(new int[] { }, createNormalDrawable(attr));

			} finally {
				attr.Recycle();
			}
		}
		private LayerDrawable createNormalDrawable(TypedArray attr) {
			LayerDrawable drawableNormal =
				(LayerDrawable) getDrawable(Resource.Drawable.rect_normal).Mutate();
				
			GradientDrawable drawableTop = (GradientDrawable)drawableNormal.GetDrawable (0).Mutate ();
			drawableTop.SetCornerRadius(getCornerRadius());

			int blueDark = getColor(Resource.Color.blue_pressed);

			int colorPressed = attr.GetColor(Resource.Styleable.FlatButton_pb_colorPressed, blueDark);
			drawableTop.SetColor(colorPressed);

			GradientDrawable drawableBottom =
				(GradientDrawable) drawableNormal.GetDrawable(1).Mutate();
			drawableBottom.SetCornerRadius(getCornerRadius());

			int blueNormal = getColor(Resource.Color.blue_normal);
			int colorNormal = attr.GetColor(Resource.Styleable.FlatButton_pb_colorNormal, blueNormal);
			drawableBottom.SetColor(colorNormal);
			return drawableNormal;
		}

		private Drawable createPressedDrawable(TypedArray attr) {
			GradientDrawable drawablePressed =
				(GradientDrawable) getDrawable(Resource.Drawable.rect_pressed).Mutate();
			drawablePressed.SetCornerRadius(getCornerRadius());

			int blueDark = getColor(Resource.Color.blue_pressed);
			int colorPressed = attr.GetColor(Resource.Styleable.FlatButton_pb_colorPressed, blueDark);
			drawablePressed.SetColor(colorPressed);

			return drawablePressed;
		}

		protected Drawable getDrawable(int id) {
			return Resources.GetDrawable (id);
		}

		protected float getDimension(int id) {
			return Resources.GetDimension(id);
		}

		protected int getColor(int id) {
			return Resources.GetColor(id);
		}

		protected TypedArray getTypedArray(Context context, IAttributeSet attributeSet, int[] attr) {
			return context.ObtainStyledAttributes(attributeSet, attr, 0, 0);
		}

		public float getCornerRadius() {
			return cornerRadius;
		}

		public StateListDrawable getNormalDrawable() {
			return mNormalDrawable;
		}

//		public ICharSequence getNormalText() {
//			return mNormalText;
//		}
		public string getNormalText() {
			return mNormalText;
		}

		/**
     * Set the View's background. Masks the API changes made in Jelly Bean.
     *
     * @param drawable
     */
		public void setBackgroundCompat(Drawable drawable) {
			int pL = PaddingLeft;
			int pT = PaddingTop;
			int pR = PaddingRight;
			int pB = PaddingBottom;
		
			if (Build.VERSION.SdkInt >= Build.VERSION_CODES.JellyBean) {
				Background = drawable;
			} else {
				Background=drawable;
			}
			SetPadding(pL, pT, pR, pB);
		}
	}
}

