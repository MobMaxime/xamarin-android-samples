using System;
using Android.Widget;
using Android.Graphics;
using Android.Content;
using Android.Util;
using Android.Views;

namespace LibrarySlideDateTimePicker
{
	public class SlidingTabStrip : LinearLayout
	{
		int _bottomBorderThickness;
		Paint _bottomBorderPaint;

		int _selectedIndicatorThickness;
		Paint _selectedIndicatorPaint;

		int _defaultBottomBorderColor;

		Paint _dividerPaint;
		float _dividerHeight;

		int _selectedPosition;
		float _selectionOffset;

		SlidingTabLayout.ITabColorizer _customTabColorizer;
		SimpleTabColorizer _defaultTabColorizer;

		public SlidingTabStrip(Context context) :this(context, null){
		}

		public SlidingTabStrip(Context context, IAttributeSet Attrs):base(context, Attrs) {
			SetWillNotDraw(false);

			float density = Resources.DisplayMetrics.Density;

			TypedValue outValue = new TypedValue();
			context.Theme.ResolveAttribute(Android.Resource.Attribute.ColorForeground, outValue, true);
			int themeForegroundColor =  outValue.Data;

			_defaultBottomBorderColor = SetColorAlpha(themeForegroundColor,0x26);

			_defaultTabColorizer = new SimpleTabColorizer();
			_defaultTabColorizer.SetIndicatorColors(0xFF33B5);
			_defaultTabColorizer.SetDividerColors(SetColorAlpha(themeForegroundColor,0x20));

			_bottomBorderThickness = (int) (2 * density);
			_bottomBorderPaint = new Paint();
			_bottomBorderPaint.Color = Color.White;

			_selectedIndicatorThickness = (int) (6 * density);
			_selectedIndicatorPaint = new Paint();

			_dividerHeight = 0.5f;
			_dividerPaint = new Paint();
			_dividerPaint.StrokeWidth=((int) (1 * density));
		}
		public void SetCustomTabColorizer(SlidingTabLayout.ITabColorizer CustomTabColorizer) {
			_customTabColorizer = CustomTabColorizer;
			Invalidate();
		}

		public void SetSelectedIndicatorColors(int Colors) {
			// Make sure that the custom colorizer is removed
			_customTabColorizer = null;
			_defaultTabColorizer.SetIndicatorColors(Colors);
			Invalidate();
		}

		public void SetDividerColors(int Colors) {
			// Make sure that the custom colorizer is removed
			_customTabColorizer = null;
			_defaultTabColorizer.SetDividerColors(Colors);
			Invalidate();
		}

		public void OnViewPagerPageChanged(int Position, float PositionOffset) {
			_selectedPosition = Position;
			_selectionOffset = PositionOffset;
			Invalidate();
		}
		protected override void OnDraw (Android.Graphics.Canvas canvas)
		{
			int height = Height;
			int childCount = ChildCount;
			int dividerHeightPx = (int) (Math.Min(Math.Max(0f, _dividerHeight), 1f) * height);
			SlidingTabLayout.ITabColorizer tabColorizer =_customTabColorizer != null ? _customTabColorizer: _defaultTabColorizer;

			// Thick colored underline below the current selection
			if (childCount > 0) {
				View selectedTitle = GetChildAt(_selectedPosition);
				int left = selectedTitle.Left;
				int right = selectedTitle.Right;
				int color = tabColorizer.GetIndicatorColor(_selectedPosition);

				if (_selectionOffset > 0f && _selectedPosition < (ChildCount - 1)) {
					int nextColor = tabColorizer.GetIndicatorColor(_selectedPosition + 1);
					if (color != nextColor) {
						color = BlendColors(nextColor, color, _selectionOffset);
					}

					// Draw the selection partway between the tabs
					View nextTitle = GetChildAt(_selectedPosition + 1);
					left = (int) (_selectionOffset * nextTitle.Left +(1.0f - _selectionOffset) * left);
					right = (int) (_selectionOffset * nextTitle.Right +(1.0f - _selectionOffset) * right);
				}
					
				_selectedIndicatorPaint.Color = Color.LightSkyBlue;


				canvas.DrawRect(left, height - _selectedIndicatorThickness, right,
					height, _selectedIndicatorPaint);
			}

			// Thin underline along the entire bottom edge
			canvas.DrawRect(0, height - _bottomBorderThickness, Width, height, _bottomBorderPaint);

			// Vertical separators between the titles
			int separatorTop = (height - dividerHeightPx) / 2;
			for (int i = 0; i < childCount - 1; i++) {
				View child = GetChildAt(i);
				_dividerPaint.Color=Color.Gray;
				canvas.DrawLine(child.Right, separatorTop, child.Right,
					separatorTop + dividerHeightPx, _dividerPaint);
			}
		}
		/**
     * Set the alpha value of the {@code color} to be the given {@code alpha} value.
     */
		private static int SetColorAlpha(int color, byte Alpha) {
			return Color.Argb(Alpha, Color.Red, Color.Green, Color.Blue);

		}

		/**
     * Blend {@code color1} and {@code color2} using the given ratio.
     *
     * @param ratio of which to blend. 1.0 will return {@code color1}, 0.5 will give an even blend,
     *              0.0 will return {@code color2}.
     */
		private static int BlendColors(int Color1, int Color2, float Ratio) {
			float inverseRation = 1f - Ratio;
			float red = (Color.Red * Ratio) + (Color.Red * inverseRation);
			float green = (Color.Green * Ratio) + (Color.Green * inverseRation);
			float blue = (Color.Blue * Ratio) + (Color.Blue * inverseRation);
			return Color.Rgb((int) red, (int) green, (int) blue);
		}
		class SimpleTabColorizer : SlidingTabLayout.ITabColorizer
		{
			int indicatorColors;
			int dividerColors;

			#region TabColorizer implementation

			int SlidingTabLayout.ITabColorizer.GetIndicatorColor (int Position)
			{
				return indicatorColors;
			}

			int SlidingTabLayout.ITabColorizer.GetDividerColor (int Position)
			{
				return dividerColors;
			}

			#endregion

			public void SetIndicatorColors(int colors) {
				indicatorColors = colors;
			}

			public void SetDividerColors(int colors) {
				dividerColors = colors;
			}
		}
	}
}

