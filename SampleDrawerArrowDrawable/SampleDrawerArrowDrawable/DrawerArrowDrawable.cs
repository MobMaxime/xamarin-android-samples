
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
using Android.Content.Res;
using Android.Graphics.Drawables;

namespace DrawerArrowDrawable
{
	public class DrawerArrowDrawable : Drawable
	{
		/**
   * Joins two {@link Path}s as if they were one where the first 50% of the path is {@code
   * PathFirst} and the second 50% of the path is {@code pathSecond}.
   */
		public class JoinedPath {

			private PathMeasure measureFirst;
			private PathMeasure measureSecond;
			private float lengthFirst;
			private float lengthSecond;

			public JoinedPath(Path pathFirst, Path pathSecond) {
				measureFirst = new PathMeasure(pathFirst, false);
				measureSecond = new PathMeasure(pathSecond, false);
				lengthFirst = measureFirst.Length;
				lengthSecond = measureSecond.Length;
			}
			/**
		     * Returns a point on this curve at the given {@code parameter}.
		     * For {@code parameter} values less than .5f, the first path will drive the point.
		     * For {@code parameter} values greater than .5f, the second path will drive the point.
		     * For {@code parameter} equal to .5f, the point will be the point where the two
		     * internal paths connect.
     		*/
			public void getPointOnLine(float parameter, float[] coords) {
				if (parameter <= .5f) {
					parameter *= 2;
					measureFirst.GetPosTan(lengthFirst * parameter, coords, null);
				} else {
					parameter -= .5f;
					parameter *= 2;
					measureSecond.GetPosTan(lengthSecond * parameter, coords, null);
				}
			}
		}
		/** Draws a line between two {@link JoinedPath}s at distance {@code parameter} along each path. */
		public class BridgingLine {

			private JoinedPath pathA;
			private JoinedPath pathB;

			public BridgingLine(JoinedPath pathA, JoinedPath pathB) {
				this.pathA = pathA;
				this.pathB = pathB;
			}
			public void draw(Canvas canvas) {
			
			pathA.getPointOnLine(parameter, coordsA);
			pathB.getPointOnLine(parameter, coordsB);
			if (rounded) insetPointsForRoundCaps();
			canvas.DrawLine(coordsA[0], coordsA[1], coordsB[0], coordsB[1], linePaint);
			}
			private void insetPointsForRoundCaps() {
				vX = coordsB[0] - coordsA[0];
				vY = coordsB[1] - coordsA[1];

				magnitude = (float) Math.Sqrt((vX * vX + vY * vY));
				paramA = (magnitude - halfStrokeWidthPixel) / magnitude;
				paramB = halfStrokeWidthPixel / magnitude;

				coordsA[0] = coordsB[0] - (vX * paramA);
				coordsA[1] = coordsB[1] - (vY * paramA);
				coordsB[0] = coordsB[0] - (vX * paramB);
				coordsB[1] = coordsB[1] - (vY * paramB);
			}
		}

		/** Paths were generated at a 3px/dp density; this is the scale factor for different densities. */
		private static float PATH_GEN_DENSITY = 3;

		/** Paths were generated with at this size for {@link DrawerArrowDrawable#PATH_GEN_DENSITY}. */
		private static float DIMEN_DP = 23.5f;

		/**
   * Paths were generated targeting this stroke width to form the arrowhead properly, modification
   * may cause the arrow to not for nicely.
   */
		private static float STROKE_WIDTH_DP = 2;

		private BridgingLine topLine;
		private BridgingLine middleLine;
		private BridgingLine bottomLine;

		private Rect bounds;
		private static float halfStrokeWidthPixel;
		public static Paint linePaint;
		public static Boolean rounded;

		private Boolean flip;
		public static float parameter;

		// Helper fields during drawing calculations.
		private static float vX, vY, magnitude, paramA, paramB;
		private static float []coordsA = { 0f, 0f };
		private static float []coordsB = { 0f, 0f };

		public DrawerArrowDrawable(Resources resources) :this(resources, false){
		}
		public DrawerArrowDrawable(Resources resources, Boolean rounded1) {
			rounded = rounded1;
			float density = resources.DisplayMetrics.Density;
			float strokeWidthPixel = STROKE_WIDTH_DP * density;
			halfStrokeWidthPixel = strokeWidthPixel / 2;

			linePaint = new Paint(Android.Graphics.PaintFlags.SubpixelText | Android.Graphics.PaintFlags.AntiAlias);
			linePaint.StrokeCap=rounded ? Android.Graphics.Paint.Cap.Round : Android.Graphics.Paint.Cap.Butt;
			linePaint.Color=Android.Graphics.Color.Black;
			linePaint.SetStyle (Android.Graphics.Paint.Style.Stroke);
			linePaint.StrokeWidth=strokeWidthPixel;

			int dimen = (int) (DIMEN_DP * density);
			bounds = new Rect(0, 0, dimen, dimen);

			Path first, second;
			JoinedPath joinedA, joinedB;

			// Top
			first = new Path();
			first.MoveTo(5.042f, 20f);
			first.RCubicTo(8.125f, -16.317f, 39.753f, -27.851f, 55.49f, -2.765f);
			second = new Path();
			second.MoveTo(60.531f, 17.235f);
			second.RCubicTo(11.301f, 18.015f, -3.699f, 46.083f, -23.725f, 43.456f);
			scalePath(first, density);
			scalePath(second, density);
			joinedA = new JoinedPath(first, second);

			first = new Path();
			first.MoveTo(64.959f, 20f);
			first.RCubicTo(4.457f, 16.75f, 1.512f, 37.982f, -22.557f, 42.699f);
			second = new Path();
			second.MoveTo(42.402f, 62.699f);
			second.CubicTo(18.333f, 67.418f, 8.807f, 45.646f, 8.807f, 32.823f);
			scalePath(first, density);
			scalePath(second, density);
			joinedB = new JoinedPath(first, second);
			topLine = new BridgingLine(joinedA, joinedB);

			// Middle
			first = new Path();
			first.MoveTo(5.042f, 35f);
			first.CubicTo(5.042f, 20.333f, 18.625f, 6.791f, 35f, 6.791f);
			second = new Path();
			second.MoveTo(35f, 6.791f);
			second.RCubicTo(16.083f, 0f, 26.853f, 16.702f, 26.853f, 28.209f);
			scalePath(first, density);
			scalePath(second, density);
			joinedA = new JoinedPath(first, second);

			first = new Path();
			first.MoveTo(64.959f, 35f);
			first.RCubicTo(0f, 10.926f, -8.709f, 26.416f, -29.958f, 26.416f);
			second = new Path();
			second.MoveTo(35f, 61.416f);
			second.RCubicTo(-7.5f, 0f, -23.946f, -8.211f, -23.946f, -26.416f);
			scalePath(first, density);
			scalePath(second, density);
			joinedB = new JoinedPath(first, second);
			middleLine = new BridgingLine(joinedA, joinedB);

			// Bottom
			first = new Path();
			first.MoveTo(5.042f, 50f);
			first.CubicTo(2.5f, 43.312f, 0.013f, 26.546f, 9.475f, 17.346f);
			second = new Path();
			second.MoveTo(9.475f, 17.346f);
			second.RCubicTo(9.462f, -9.2f, 24.188f, -10.353f, 27.326f, -8.245f);
			scalePath(first, density);
			scalePath(second, density);
			joinedA = new JoinedPath(first, second);

			first = new Path();
			first.MoveTo(64.959f, 50f);
			first.RCubicTo(-7.021f, 10.08f, -20.584f, 19.699f, -37.361f, 12.74f);
			second = new Path();
			second.MoveTo(27.598f, 62.699f);
			second.RCubicTo(-15.723f, -6.521f, -18.8f, -23.543f, -18.8f, -25.642f);
			scalePath(first, density);
			scalePath(second, density);
			joinedB = new JoinedPath(first, second);
			bottomLine = new BridgingLine(joinedA, joinedB);
		}
		public void setStrokeColor(int color) {
			//linePaint.Color = color;
			linePaint.Color = Android.Graphics.Color.LightGray;
			InvalidateSelf();
		}
		private static void scalePath(Path path, float density) {
			if (density == PATH_GEN_DENSITY) return;
			Matrix scaleMatrix = new Matrix();
			scaleMatrix.SetScale(density / PATH_GEN_DENSITY, density / PATH_GEN_DENSITY, 0, 0);
			path.Transform(scaleMatrix);
		}
		/**
   * Sets the rotation of this drawable based on {@code parameter} between 0 and 1. Usually driven
   * via {@link DrawerListener#onDrawerSlide(View, float)}'s {@code slideOffset} parameter.
   */
		public void setParameter(float parameter1) {
			if (parameter > 1 || parameter < 0) {
				throw new Java.Lang.IllegalArgumentException("Value must be between 1 and zero inclusive!");
			}
			parameter = parameter1;
			InvalidateSelf();
		}

		/**
   * When false, rotates from 3 o'clock to 9 o'clock between a drawer icon and a back arrow.
   * When true, rotates from 9 o'clock to 3 o'clock between a back arrow and a drawer icon.
   */
		public void setFlip(Boolean flip) {
			this.flip = flip;
			InvalidateSelf();
		}

		public override int IntrinsicHeight {
			get {
//				return base.IntrinsicHeight;
				return bounds.Height ();
			}
		}
		public override int IntrinsicWidth {
			get {
				//return base.IntrinsicWidth;
				return bounds.Width ();
			}
		}
		#region implemented abstract members of Drawable


		public override void Draw (Canvas canvas)
		{
			if (flip) {
				canvas.Save();
				canvas.Scale(1f, -1f, IntrinsicWidth / 2, IntrinsicHeight / 2);
			}
			topLine.draw(canvas);
			middleLine.draw(canvas);
			bottomLine.draw(canvas);

			if (flip) canvas.Restore();
			//			throw new NotImplementedException ();
		}


		public override void SetAlpha (int alpha)
		{
			linePaint.Alpha=alpha;
			InvalidateSelf();
			//throw new NotImplementedException ();
		}


		public override void SetColorFilter (ColorFilter cf)
		{
			linePaint.SetColorFilter (cf);
			InvalidateSelf();
			//throw new NotImplementedException ();
		}


		public override int Opacity {
			get {
				return (int)Android.Graphics.Format.Translucent;
				//throw new NotImplementedException ();
			}
		}


		#endregion

	}
}

