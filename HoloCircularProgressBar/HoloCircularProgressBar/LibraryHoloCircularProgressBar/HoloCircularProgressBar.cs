using System;
using Android.Views;
using Android.Graphics;
using Android.Content;
using Android.Util;
using Android.Content.Res;
using Android.OS;

namespace LibraryHoloCircularProgressBar
{
	public class HoloCircularProgressBar : View
	{
		/**
     * The rectangle enclosing the circle.
     */
		RectF _circleBounds = new RectF();

		/**
     * the rect for the thumb square
     */
		RectF _squareRect = new RectF();

		/**
     * the paint for the background.
     */
		Paint _backgroundColorPaint = new Paint();

		/**
     * The stroke width used to paint the circle.
     */
		int _circleStrokeWidth = 10;

		/**
     * The gravity of the view. Where should the Circle be drawn within the given bounds
     *
     * {@link #computeInsets(int, int)}
     */
		GravityFlags _gravity = GravityFlags.Center;

		/**
     * The Horizontal inset calcualted in {@link #computeInsets(int, int)} depends on {@link
     * #mGravity}.
     */
		int _horizontalInset = 0;

		/**
     * true if not all properties are set. then the view isn't drawn and there are no errors in the
     * LayoutEditor
     */
		bool _isInitializing = true;

		/**
     * flag if the marker should be visible
     */
		bool _isMarkerEnabled = false;

		/**
     * indicates if the thumb is visible
     */
		bool _isThumbEnabled = true;

		/**
     * The Marker color paint.
     */
		Paint _markerColorPaint;

		/**
     * The Marker progress.
     */
		float _markerProgress = 0.0f;

		/**
     * the overdraw is true if the progress is over 1.0.
     */
		bool _overrdraw = false;

		/**
     * The current progress.
     */
		float _progress = 0.3f;

		/**
     * The color of the progress background.
     */
		int _progressBackgroundColor;

		/**
     * the color of the progress.
     */
		int _progressColor;

		/**
     * paint for the progress.
     */
		Paint _progressColorPaint;

		/**
     * Radius of the circle
     *
     * <p> Note: (Re)calculated in {@link #onMeasure(int, int)}. </p>
     */
		float _radius;

		/**
     * The Thumb color paint.
     */
		Paint _thumbColorPaint = new Paint();

		/**
     * The Thumb pos x.
     *
     * Care. the position is not the position of the rotated thumb. The position is only calculated
     * in {@link #onMeasure(int, int)}
     */
		float _thumbPosX;

		/**
     * The Thumb pos y.
     *
     * Care. the position is not the position of the rotated thumb. The position is only calculated
     * in {@link #onMeasure(int, int)}
     */
		float _thumbPosY;

		/**
     * The pointer width (in pixels).
     */
		int _thumbRadius = 20;

		/**
     * The Translation offset x which gives us the ability to use our own coordinates system.
     */
		float _translationOffsetX;

		/**
     * The Translation offset y which gives us the ability to use our own coordinates system.
     */
		float _translationOffsetY;

		/**
     * The Vertical inset calcualted in {@link #computeInsets(int, int)} depends on {@link
     * #mGravity}..
     */
		int _verticalInset = 0;

		/**
     * Instantiates a new holo circular progress bar.
     *
     * @param context the context
     */
		public HoloCircularProgressBar(Context context) :this(context, null){
		}
		/**
     * Instantiates a new holo circular progress bar.
     *
     * @param context the context
     * @param attrs   the attrs
     */
		public HoloCircularProgressBar(Context context, IAttributeSet Attrs):this(context, Attrs, Resource.Attribute.circularProgressBarStyle) {
		}

		/**
     * Instantiates a new holo circular progress bar.
     *
     * @param context  the context
     * @param attrs    the attrs
     * @param defStyle the def style
     */
		public HoloCircularProgressBar(Context context, IAttributeSet Attrs,int DefStyle):base(context, Attrs, DefStyle) {

			// load the styled attributes and set their properties
			TypedArray attributes = context.ObtainStyledAttributes(Attrs, Resource.Styleable.HoloCircularProgressBar,DefStyle, 0);
			if (attributes != null) {
				try {
					SetProgressColor(attributes.GetColor(Resource.Styleable.HoloCircularProgressBar_progress_color, Color.Cyan));
					SetProgressBackgroundColor(attributes.GetColor(Resource.Styleable.HoloCircularProgressBar_progress_background_color,Color.Green));
					SetProgress(attributes.GetFloat(Resource.Styleable.HoloCircularProgressBar_progress, 0.0f));
					SetMarkerProgress(attributes.GetFloat(Resource.Styleable.HoloCircularProgressBar_marker_progress,0.0f));
					SetWheelSize((int) attributes.GetDimension(Resource.Styleable.HoloCircularProgressBar_stroke_width, 10));
					SetThumbEnabled(attributes.GetBoolean(Resource.Styleable.HoloCircularProgressBar_thumb_visible, true));
					SetMarkerEnabled(attributes.GetBoolean(Resource.Styleable.HoloCircularProgressBar_marker_visible, true));

					_gravity = (GravityFlags)attributes.GetInt (Resource.Styleable.HoloCircularProgressBar_android_gravity,(int)GravityFlags.Center);
				} finally {
					// make sure recycle is always called.
					attributes.Recycle();
				}
			}

			_thumbRadius = _circleStrokeWidth * 2;

			UpdateBackgroundColor();

			UpdateMarkerColor();

			UpdateProgressColor();

			// the view has now all properties and can be drawn
			_isInitializing = false;

		}
		protected override void OnDraw (Canvas canvas)
		{
			// All of our positions are using our internal coordinate system.
			// Instead of translating
			// them we let Canvas do the work for us.
			canvas.Translate(_translationOffsetX, _translationOffsetY);

			float progressRotation = GetCurrentRotation();

			// draw the background
			if (!_overrdraw) {
				canvas.DrawArc(_circleBounds, 270, -(360 - progressRotation), false,
					_backgroundColorPaint);
			}

			// draw the progress or a full circle if overdraw is true
			canvas.DrawArc(_circleBounds, 270, _overrdraw ? 360 : progressRotation, false,
				_progressColorPaint);

			// draw the marker at the correct rotated position
			if (_isMarkerEnabled) {
				float markerRotation = GetMarkerRotation();

				canvas.Save();
				canvas.Rotate(markerRotation - 90);
				canvas.DrawLine((float) (_thumbPosX + _thumbRadius / 2 * 1.4), _thumbPosY,
					(float) (_thumbPosX - _thumbRadius / 2 * 1.4), _thumbPosY, _markerColorPaint);
				canvas.Restore();
			}

			if (IsThumbEnabled()) {
				// draw the thumb square at the correct rotated position
				canvas.Save();
				canvas.Rotate(progressRotation - 90);
				// rotate the square by 45 degrees
				canvas.Rotate(45, _thumbPosX, _thumbPosY);
				_squareRect.Left = _thumbPosX - _thumbRadius / 3;
				_squareRect.Right = _thumbPosX + _thumbRadius / 3;
				_squareRect.Top = _thumbPosY - _thumbRadius / 3;
				_squareRect.Bottom = _thumbPosY + _thumbRadius / 3;
				canvas.DrawRect(_squareRect, _thumbColorPaint);
				canvas.Restore();
			}
		}
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			int height = GetDefaultSize(SuggestedMinimumHeight + PaddingTop + PaddingBottom,heightMeasureSpec);
			int width = GetDefaultSize(SuggestedMinimumWidth + PaddingLeft + PaddingRight,widthMeasureSpec);

			int diameter;
			if (heightMeasureSpec == (int)MeasureSpecMode.Unspecified) {
				// ScrollView
				diameter = width;
				ComputeInsets(0, 0);
			} else if (widthMeasureSpec ==(int) MeasureSpecMode.Unspecified) {
				// HorizontalScrollView
				diameter = height;
				ComputeInsets(0, 0);
			} else {
				// Default
				diameter = Math.Min(width, height);
				ComputeInsets(width - diameter, height - diameter);
			}

			SetMeasuredDimension(diameter, diameter);

			float halfWidth = diameter * 0.5f;

			// width of the drawed circle (+ the drawedThumb)
			float drawedWith;
			if (IsThumbEnabled()) {
				drawedWith = _thumbRadius * (5f / 6f);
			} else if (IsMarkerEnabled()) {
				drawedWith = _circleStrokeWidth * 1.4f;
			} else {
				drawedWith = _circleStrokeWidth / 2f;
			}

			// -0.5f for pixel perfect fit inside the viewbounds
			_radius = halfWidth - drawedWith - 0.5f;

			_circleBounds.Set(-_radius, -_radius, _radius, _radius);

			_thumbPosX = (float) (_radius * Math.Cos(0));
			_thumbPosY = (float) (_radius * Math.Sin(0));

			_translationOffsetX = halfWidth + _horizontalInset;
			_translationOffsetY = halfWidth + _verticalInset;

		}
		protected override void OnRestoreInstanceState (Android.OS.IParcelable state)
		{
			if (state is Bundle) {
				Bundle bundle = (Bundle) state;
				SetProgress(bundle.GetFloat("progress"));
				SetMarkerProgress(bundle.GetFloat("marker_progress"));

				int progressColor = bundle.GetInt("progress_color");
				if (progressColor != _progressColor) {
					_progressColor = progressColor;
					UpdateProgressColor();
				}

				int progressBackgroundColor = bundle.GetInt("progress_background_color");
				if (progressBackgroundColor != _progressBackgroundColor) {
					_progressBackgroundColor = progressBackgroundColor;
					UpdateBackgroundColor();
				}

				_isThumbEnabled = bundle.GetBoolean("thumb_visible");

				_isMarkerEnabled = bundle.GetBoolean("marker_visible");

				base.OnRestoreInstanceState((IParcelable)bundle.GetParcelable("saved_state"));
				return;
			}
				
			base.OnRestoreInstanceState (state);
		}
		protected override IParcelable OnSaveInstanceState ()
		{
			Bundle bundle = new Bundle();
			bundle.PutParcelable("saved_state", base.OnSaveInstanceState());
			bundle.PutFloat("progress", _progress);
			bundle.PutFloat("marker_progress", _markerProgress);
			bundle.PutInt("progress_color", _progressColor);
			bundle.PutInt("progress_background_color", _progressBackgroundColor);
			bundle.PutBoolean("thumb_visible", _isThumbEnabled);
			bundle.PutBoolean("marker_visible", _isMarkerEnabled);
			return bundle;
		}
		public int GetCircleStrokeWidth() {
			return _circleStrokeWidth;
		}

		/**
     * similar to {@link #getProgress}
     */
		public float GetMarkerProgress() {
			return _markerProgress;
		}

		/**
     * gives the current progress of the ProgressBar. Value between 0..1 if you set the progress to
     * >1 you'll get progress % 1 as return value
     *
     * @return the progress
     */
		public float GetProgress() {
			return _progress;
		}

		/**
     * Gets the progress color.
     *
     * @return the progress color
     */
		public int GetProgressColor() {
			return _progressColor;
		}

		/**
     * @return true if the marker is visible
     */
		public bool IsMarkerEnabled() {
			return _isMarkerEnabled;
		}

		/**
     * @return true if the marker is visible
     */
		public bool IsThumbEnabled() {
			return _isThumbEnabled;
		}

		/**
     * Sets the marker enabled.
     *
     * @param enabled the new marker enabled
     */
		public void SetMarkerEnabled(bool enabled) {
			_isMarkerEnabled = enabled;
		}

		/**
     * Sets the marker progress.
     *
     * @param progress the new marker progress
     */
		public void SetMarkerProgress(float progress) {
			_isMarkerEnabled = true;
			_markerProgress = progress;
		}

		/**
     * Sets the progress.
     *
     * @param progress the new progress
     */
		public void SetProgress(float progress) {
			if (progress == _progress) {
				return;
			}

			if (progress == 1) {
				_overrdraw = false;
				_progress = 1;
			} else {

				if (progress >= 1) {
					_overrdraw = true;
				} else {
					_overrdraw = false;
				}

				_progress = progress % 1.0f;
			}

			if (!_isInitializing) {
				Invalidate();
			}
		}

		/**
     * Sets the progress background color.
     *
     * @param color the new progress background color
     */
		public void SetProgressBackgroundColor(int color) {
			_progressBackgroundColor = color;

			UpdateMarkerColor();
			UpdateBackgroundColor();
		}

		/**
     * Sets the progress color.
     *
     * @param color the new progress color
     */
		public void SetProgressColor(int color) {
			_progressColor = color;

			UpdateProgressColor();
		}

		/**
     * shows or hides the thumb of the progress bar
     *
     * @param enabled true to show the thumb
     */
		public void SetThumbEnabled(bool enabled) {
			_isThumbEnabled = enabled;
		}

		/**
     * Sets the wheel size.
     *
     * @param dimension the new wheel size
     */
		public void SetWheelSize(int dimension) {
			_circleStrokeWidth = dimension;

			// update the paints
			UpdateBackgroundColor();
			UpdateMarkerColor();
			UpdateProgressColor();
		}

		/**
     * Compute insets.
     *
     * <pre>
     *  ______________________
     * |_________dx/2_________|
     * |......| /'''''\|......|
     * |-dx/2-|| View ||-dx/2-|
     * |______| \_____/|______|
     * |________ dx/2_________|
     * </pre>
     *
     * @param dx the dx the horizontal unfilled space
     * @param dy the dy the horizontal unfilled space
     */
//		@SuppressLint("NewApi")
		void ComputeInsets(int Dx, int Dy) {
			int absoluteGravity = (int)_gravity;
			if (Build.VERSION.SdkInt >=BuildVersionCodes.JellyBean) {
				absoluteGravity = (int)Gravity.GetAbsoluteGravity(GravityFlags.Center,GravityFlags.Center);
			}

			switch (absoluteGravity & (int)GravityFlags.HorizontalGravityMask) {
			case (int)GravityFlags.Left:
				_horizontalInset = 0;
				break;
			case (int)GravityFlags.Right:
				_horizontalInset = Dx;
				break;
			case(int)GravityFlags.CenterHorizontal:
			default:
				_horizontalInset = Dx / 2;
				break;
			}
			switch (absoluteGravity & (int)GravityFlags.VerticalGravityMask) {
			case (int)GravityFlags.Top:
				_verticalInset = 0;
				break;
			case (int)GravityFlags.Bottom:
				_verticalInset = Dy;
				break;
			case(int)GravityFlags.CenterVertical:
			default:
				_verticalInset = Dy / 2;
				break;
			}
		}

		/**
     * Gets the current rotation.
     *
     * @return the current rotation
     */
		float GetCurrentRotation() {
			return 360 * _progress;
		}

		/**
     * Gets the marker rotation.
     *
     * @return the marker rotation
     */
		float GetMarkerRotation() {
			return 360 * _markerProgress;
		}

		/**
     * updates the paint of the background
     */
		void UpdateBackgroundColor() {
			_backgroundColorPaint = new Paint(PaintFlags.AntiAlias);
			_backgroundColorPaint.Color=Color.White;
			_backgroundColorPaint.SetStyle(Paint.Style.Stroke);
			_backgroundColorPaint.StrokeWidth=_circleStrokeWidth;

			Invalidate();
		}

		/**
     * updates the paint of the marker
     */
		void UpdateMarkerColor() {
			_markerColorPaint = new Paint(PaintFlags.AntiAlias);
			_markerColorPaint.Color=Color.Red;
			_markerColorPaint.SetStyle(Paint.Style.Stroke);
			_markerColorPaint.StrokeWidth=_circleStrokeWidth / 2;

			Invalidate();
		}

		/**
     * updates the paint of the progress and the thumb to give them a new visual style
     */
		void UpdateProgressColor() {
			_progressColorPaint = new Paint(PaintFlags.AntiAlias);
			_progressColorPaint.Color=Color.Brown;
			_progressColorPaint.SetStyle(Paint.Style.Stroke);
			_progressColorPaint.StrokeWidth=_circleStrokeWidth;

			_thumbColorPaint = new Paint(PaintFlags.AntiAlias);
			_thumbColorPaint.Color=Color.Crimson;
			_thumbColorPaint.SetStyle(Paint.Style.FillAndStroke);
			_thumbColorPaint.StrokeWidth=_circleStrokeWidth;

			Invalidate();
		}
	}
}

