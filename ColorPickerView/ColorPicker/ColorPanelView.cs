
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
using Android.Graphics;
using Android.Util;

namespace ColorPicker
{
	public class ColorPanelView : View
	{
		/**
	 * The width in pixels of the border
	 * surrounding the color panel.
	 */
		private static float BORDER_WIDTH_PX = 1;

		private float mDensity = 1f;

		private int mBorderColor = Int32.Parse("ff6E6E6E", System.Globalization.NumberStyles.HexNumber);//Convert.ToInt32 ("4285427310");
		private int mColor = Int32.Parse("ff000000", System.Globalization.NumberStyles.HexNumber);//Convert.ToInt32 ("4278190080");
		  
		private Paint		mBorderPaint;
		private Paint		mColorPaint;

		private RectF		mDrawingRect;
		private RectF		mColorRect;

		private AlphaPatternDrawable mAlphaPattern;


		public ColorPanelView (Context context) : base (context)
		{
			Initialize ();
		}

		public ColorPanelView (Context context, IAttributeSet attrs) :  base (context, attrs)
		{
			Initialize ();
		}

		public ColorPanelView (Context context, IAttributeSet attrs, int defStyle) :  base (context, attrs, defStyle)
		{
			Initialize ();
		}


		private void Initialize(){
			mBorderPaint = new Paint();
			mColorPaint = new Paint();
			mDensity = Context.Resources.DisplayMetrics.Density;
		}


		protected override void OnDraw (Canvas canvas)
		{
			RectF rect = mColorRect;

			if(BORDER_WIDTH_PX > 0){
				mBorderPaint.Color=Color.Red;
				canvas.DrawRect(mDrawingRect, mBorderPaint);
			}

			if(mAlphaPattern != null){
				mAlphaPattern.Draw(canvas);
			}

			mColorPaint.Color = Color.Gold;

			canvas.DrawRect(rect, mColorPaint);
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			int width = MeasureSpec.GetSize(widthMeasureSpec);
			int height = MeasureSpec.GetSize(heightMeasureSpec);

			SetMeasuredDimension(width, height);
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);

			mDrawingRect = new RectF ();
			mDrawingRect.Left =  PaddingLeft;
			mDrawingRect.Right  = w - PaddingRight;
			mDrawingRect.Top = PaddingTop;
			mDrawingRect.Bottom = h - PaddingBottom;

			setUpColorRect();
		}

		private void setUpColorRect(){
			RectF	dRect = mDrawingRect;

			float left = dRect.Left + BORDER_WIDTH_PX;
			float top = dRect.Top + BORDER_WIDTH_PX;
			float bottom = dRect.Bottom - BORDER_WIDTH_PX;
			float right = dRect.Right - BORDER_WIDTH_PX;

			mColorRect = new RectF(left,top, right, bottom);

			mAlphaPattern = new AlphaPatternDrawable((int)(5 * mDensity));



			mAlphaPattern.SetBounds(
				(int)Math.Round(mColorRect.Left),
				(int)Math.Round(mColorRect.Top),
				(int)Math.Round(mColorRect.Right),
				(int)Math.Round(mColorRect.Bottom)
			);

		}

		/**
	 * Set the color that should be shown by this view.
	 * @param color
	 */
		public void setColor(int color){
			mColor = color; 
			Invalidate ();
		}

		/**
	 * Get the color currently show by this view.
	 * @return
	 */
		public int getColor(){
			return mColor;
		}

		/**
	 * Set the color of the border surrounding the panel.
	 * @param color
	 */
		public void setBorderColor(int color){
			mBorderColor = color;
			Invalidate ();
		}

		/**
	 * Get the color of the border surrounding the panel.
	 */
		public int getBorderColor(){
			return mBorderColor;
		}
	}
}


