
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
using Android.Util;
using Android.Content.Res;
using Android.Graphics;
using Android.Views.Animations;
using Android.Support.V4.View;

namespace Library
{
	public class ActionProcessButton : ProcessButton
	{
		private ProgressBar mProgressBar;

		private Mode mMode;

		private int mColor1;
		private int mColor2;
		private int mColor3;
		private int mColor4;

		public enum Mode {
			PROGRESS, ENDLESS
		};

		public ActionProcessButton(Context context):base(context) {
			init(context);
		}

		public ActionProcessButton(Context context, IAttributeSet attrs) :base(context,attrs){
			init(context);
		}

		public ActionProcessButton(Context context, IAttributeSet attrs, int defStyle):base(context, attrs, defStyle) {
			init(context);
		}

		private void init(Context context) {

			Resources res = context.Resources;

			mMode = Mode.ENDLESS;
		
			mColor1 = res.GetColor(Resource.Color.holo_blue_bright);
			mColor2 = res.GetColor(Resource.Color.holo_green_light);
			mColor3 = res.GetColor(Resource.Color.holo_orange_light);
			mColor4 = res.GetColor(Resource.Color.holo_red_light);
		}

		public void setMode(Mode mode) {
			mMode = mode;
		}

		public void setColorScheme(int color1, int color2, int color3, int color4) {
			mColor1 = color1;
			mColor2 = color2;
			mColor3 = color3;
			mColor4 = color4;
		}

		#region implemented abstract members of ProcessButton


		public override void drawProgress (Android.Graphics.Canvas canvas)
		{
			if(Background != getNormalDrawable()) {
				Background = getNormalDrawable ();
				//setBackgroundDrawable(getNormalDrawable());
			}

			switch (mMode) {
			case Mode.ENDLESS:
				drawEndlessProgress(canvas);
				break;
			case Mode.PROGRESS:
				drawLineProgress(canvas);
				break;
			}
		}


		#endregion
		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);
			if (mProgressBar != null) {
				setupProgressBarBounds();
			}
		}
		private void drawLineProgress(Canvas canvas) {
			float scale = (float) getProgress() / (float) getMaxProgress();
			float indicatorWidth = (float) MeasuredWidth * scale;

			double indicatorHeightPercent = 0.05; // 5%
			int bottom = (int) (MeasuredHeight - MeasuredHeight * indicatorHeightPercent);
			getProgressDrawable().SetBounds(0, bottom, (int) indicatorWidth, MeasuredHeight);
			getProgressDrawable().Draw(canvas);
		}

		private void drawEndlessProgress(Canvas canvas) {
			if (mProgressBar == null) {
				mProgressBar = new ProgressBar(this);
				setupProgressBarBounds ();
				mProgressBar.setColorScheme(mColor1, mColor2, mColor3, mColor4);
				mProgressBar.start();
			}

			if (getProgress() > 0) {
				mProgressBar.draw(canvas);
			}
		}

		private void setupProgressBarBounds() {
			double indicatorHeight = getDimension(Resource.Dimension.layer_padding);
			int bottom = (int) (MeasuredHeight - indicatorHeight);
			mProgressBar.setBounds(0, bottom, MeasuredWidth, MeasuredHeight);
		}

		public class ProgressBar {

			// Default progress animation colors are grays.

			// The duration of the animation cycle.
			private static int ANIMATION_DURATION_MS = 2000;

			// The duration of the animation to clear the bar.
			private static int FINISH_ANIMATION_DURATION_MS = 1000;

			// Interpolator for varying the speed of the animation.

			public static AccelerateDecelerateInterpolator INTERPOLATOR = new AccelerateDecelerateInterpolator();

			private static Paint mPaint = new Paint();
			private static RectF mClipRect = new RectF();
			private static float mTriggerPercentage;
			private static long mStartTime;
			private static long mFinishTime;
			private static Boolean mRunning;

			// Colors used when rendering the animation,
			private static  int mColor1;
			private static int mColor2;
			private static int mColor3;
			private static int mColor4;
			private static View mParent;

			private static Rect mBounds = new Rect();

			public ProgressBar(View parent) {
				mParent = parent;
			}

			/**
         * Set the four colors used in the progress animation. The first color will
         * also be the color of the bar that grows in response to a user swipe
         * gesture.
         *
         * @param color1 Integer representation of a color.
         * @param color2 Integer representation of a color.
         * @param color3 Integer representation of a color.
         * @param color4 Integer representation of a color.
         */
			public void setColorScheme(int color1, int color2, int color3, int color4) {
				mColor1 = color1;
				mColor2 = color2;
				mColor3 = color3;
				mColor4 = color4;
			}

			/**
         * Start showing the progress animation.
         */
			public  void start() {
				if (!mRunning) {
					mTriggerPercentage = 0;
					mStartTime = AnimationUtils.CurrentAnimationTimeMillis();
					mRunning = true;
					mParent.PostInvalidate();
				}
			}

			public void draw(Canvas canvas) {
				int width = mBounds.Width ();
				int height = mBounds.Height();
				int cx = width / 2;
				int cy = height / 2;
				Boolean drawTriggerWhileFinishing = false;
				int restoreCount = canvas.Save();
				canvas.ClipRect(mBounds);

				if (mRunning || (mFinishTime > 0)) {
					long now = AnimationUtils.CurrentAnimationTimeMillis();
					long elapsed = (now - mStartTime) % ANIMATION_DURATION_MS;
					long iterations = (now - mStartTime) / ANIMATION_DURATION_MS;
					float rawProgress = (elapsed / (ANIMATION_DURATION_MS / 100f));

					// If we're not running anymore, that means we're running through
					// the finish animation.
					if (!mRunning) {
						// If the finish animation is done, don't draw anything, and
						// don't repost.
						if ((now - mFinishTime) >= FINISH_ANIMATION_DURATION_MS) {
							mFinishTime = 0;
							return;
						}

						// Otherwise, use a 0 opacity alpha layer to clear the animation
						// from the inside out. This layer will prevent the circles from
						// drawing within its bounds.
						long finishElapsed = (now - mFinishTime) % FINISH_ANIMATION_DURATION_MS;
						float finishProgress = (finishElapsed / (FINISH_ANIMATION_DURATION_MS / 100f));
						float pct = (finishProgress / 100f);
						// Radius of the circle is half of the screen.
						float clearRadius = width / 2;//* INTERPOLATOR.getInterpolation(pct);
						mClipRect.Set(cx - clearRadius, 0, cx + clearRadius, height);
						canvas.SaveLayerAlpha(mClipRect, 0, 0);
						// Only draw the trigger if there is a space in the center of
						// this refreshing view that needs to be filled in by the
						// trigger. If the progress view is just still animating, let it
						// continue animating.
						drawTriggerWhileFinishing = true;
					}

					// First fill in with the last color that would have finished drawing.
					if (iterations == 0) {
						canvas.DrawColor (Android.Graphics.Color.Blue);
					} else {
						if (rawProgress >= 0 && rawProgress < 25) {
							canvas.DrawColor(Android.Graphics.Color.AliceBlue);
						} else if (rawProgress >= 25 && rawProgress < 50) {
							canvas.DrawColor (Android.Graphics.Color.Blue);
						} else if (rawProgress >= 50 && rawProgress < 75) {
							canvas.DrawColor (Android.Graphics.Color.BlueViolet);
						} else {
							canvas.DrawColor (Android.Graphics.Color.CadetBlue);
						}
					}

					// Then draw up to 4 overlapping concentric circles of varying radii, based on how far
					// along we are in the cycle.
					// progress 0-50 draw mColor2
					// progress 25-75 draw mColor3
					// progress 50-100 draw mColor4
					// progress 75 (wrap to 25) draw mColor1
					if ((rawProgress >= 0 && rawProgress <= 25)) {
						float pct = (((rawProgress + 25) * 2) / 100f);
						drawCircle(canvas, cx, cy, mColor1, pct);
					}
					if (rawProgress >= 0 && rawProgress <= 50) {
						float pct = ((rawProgress * 2) / 100f);
						drawCircle(canvas, cx, cy, mColor2, pct);
					}
					if (rawProgress >= 25 && rawProgress <= 75) {
						float pct = (((rawProgress - 25) * 2) / 100f);
						drawCircle(canvas, cx, cy, mColor3, pct);
					}
					if (rawProgress >= 50 && rawProgress <= 100) {
						float pct = (((rawProgress - 50) * 2) / 100f);
						drawCircle(canvas, cx, cy, mColor4, pct);
					}
					if ((rawProgress >= 75 && rawProgress <= 100)) {
						float pct = (((rawProgress - 75) * 2) / 100f);
						drawCircle(canvas, cx, cy, mColor1, pct);
					}
					if (mTriggerPercentage > 0 && drawTriggerWhileFinishing) {
						// There is some portion of trigger to draw. Restore the canvas,
						// then draw the trigger. Otherwise, the trigger does not appear
						// until after the bar has finished animating and appears to
						// just jump in at a larger width than expected.
						canvas.RestoreToCount(restoreCount);
						restoreCount = canvas.Save();
						canvas.ClipRect(mBounds);
						drawTrigger(canvas, cx, cy);
					}
					// Keep running until we finish out the last cycle.
					ViewCompat.PostInvalidateOnAnimation(mParent);
				} else {
					// Otherwise if we're in the middle of a trigger, draw that.
					if (mTriggerPercentage > 0 && mTriggerPercentage <= 1.0) {
						drawTrigger(canvas, cx, cy);
					}
				}
				canvas.RestoreToCount(restoreCount);
			}

			private void drawTrigger(Canvas canvas, int cx, int cy) {
				canvas.DrawColor (Android.Graphics.Color.Blue);
				//mPaint.setColor(mColor1);
				canvas.DrawCircle(cx, cy, cx * mTriggerPercentage, mPaint);
			}

			/**
         * Draws a circle centered in the view.
         *
         * @param canvas the canvas to draw on
         * @param cx the center x coordinate
         * @param cy the center y coordinate
         * @param color the color to draw
         * @param pct the percentage of the view that the circle should cover
         */
			private void drawCircle(Canvas canvas, float cx, float cy, int color, float pct) {
				mPaint.Color = Android.Graphics.Color.Brown;
				//mPaint.SetColor(color);
				canvas.Save();
				canvas.Translate (cx, cy);
				float radiusScale = INTERPOLATOR.GetInterpolation (pct);
				canvas.Scale(radiusScale, radiusScale);
				canvas.DrawCircle(0, 0, cx, mPaint);
				canvas.Restore();
			}

			/**
         * Set the drawing bounds of this SwipeProgressBar.
         */
			public void setBounds(int left, int top, int right, int bottom) {
				mBounds.Left = left;
				mBounds.Top = top;
				mBounds.Right = right;
				mBounds.Bottom = bottom;
			}
		}

	}
}

