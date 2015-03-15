using System;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Content;

namespace LibraryGoogleProgressBar
{
	public class FoldingCirclesDrawable : Drawable,Drawable.ICallback
	{
		private static float _maxLevel = 10000;
		private static float _circleCount=Enum.GetValues(typeof(ProgressStates)).Length;
		private static float _maxLevePerCircle = _maxLevel / _circleCount;
		private static int _alphaOpaque = 255;
		private static int _alphaAboveDefault = 235;

		private Paint _firstHalfPaint;
		private Paint _secondHalfPaint;
		private Paint _abovePaint;
		private RectF _oval = new RectF();
		private int _diameter;
		private Path _path;
		private int _half;
		private ProgressStates _currentState;
		private int _controlPointMinimum;
		private int _controlPointMaximum;
		private int _axisValue;
		private int _alpha = _alphaOpaque;
		private ColorFilter _colorFilter;
		private static int _color1;
		private static int _color2;
		private static int _color3;
		private static int _color4;
		private int _firstColor, _secondColor;
		private bool _goesBackward;

		private enum ProgressStates {
			FOLDING_DOWN,
			FOLDING_LEFT,
			FOLDING_UP,
			FOLDING_RIGHT
		}
		public FoldingCirclesDrawable(int[] colors) {
			InitCirclesProgress(colors);
		}

		private void InitCirclesProgress(int[] colors) {
			InitColors(colors);
			_path = new Path();

			Paint basePaint = new Paint();
			basePaint.AntiAlias=true;

			_firstHalfPaint = new Paint(basePaint);
			_secondHalfPaint = new Paint(basePaint);
			_abovePaint = new Paint(basePaint);

			// init alpha and color filter
			SetAlpha(_alpha);
			SetColorFilter(_colorFilter);
		}

		private void InitColors(int[] colors) {
			_color1=colors[0];
			_color2=colors[1];
			_color3=colors[2];
			_color4=colors[3];
		}
		protected override void OnBoundsChange (Rect bounds)
		{
			base.OnBoundsChange (bounds);
			MeasureCircleProgress(bounds.Width(), bounds.Height());
		}
		protected override bool OnLevelChange (int level)
		{
			// level goes from 0 to 10000 but the number of colors divides 10000
			// so we need to do that hack that maps level 10000 to level 0
			int animationLevel = level == _maxLevel ? 0 : level;

			// state
			int stateForLevel = (int) (animationLevel / _maxLevePerCircle);
			System.Array a= Enum.GetValues (typeof(ProgressStates));
			_currentState =(ProgressStates)a.GetValue(stateForLevel);

			// colors
			ResetColor(_currentState);
			int levelForCircle = (int) (animationLevel % _maxLevePerCircle);

			bool halfPassed;
			if (!_goesBackward) {
				halfPassed = levelForCircle != (int) (animationLevel % (_maxLevePerCircle / 2));
			} else {
				halfPassed = levelForCircle == (int) (animationLevel % (_maxLevePerCircle / 2));
				levelForCircle = (int) (_maxLevePerCircle - levelForCircle);
			}
	
			if (!halfPassed) {
				_abovePaint.Color=_secondHalfPaint.Color;
			} else {
				_abovePaint.Color=_firstHalfPaint.Color;
			}

			// invalidate alpha (Paint#setAlpha is a shortcut for setColor(alpha part)
			// so alpha is affected by setColor())
			SetAlpha(_alpha);

			// axis
			_axisValue = (int) (_controlPointMinimum + (_controlPointMaximum - _controlPointMinimum) * (levelForCircle / _maxLevePerCircle));

			return true;
		}
		private void ResetColor(ProgressStates currentState) {
			switch (currentState){
			case ProgressStates.FOLDING_DOWN:
				_firstColor = _color1;
				_secondColor = _color2;
				_firstHalfPaint.Color = Color.ParseColor ("#0099CC");
				_secondHalfPaint.Color=Color.ParseColor ("#C93437");
				_goesBackward=false;
				break;
			case ProgressStates.FOLDING_LEFT:
				_firstColor= _color1;
				_secondColor=_color3;
				_firstHalfPaint.Color=Color.ParseColor ("#0099CC");
				_secondHalfPaint.Color=Color.ParseColor("#F7D23E");
				_goesBackward=true;
				break;
			case ProgressStates.FOLDING_UP:
				_firstColor= _color3;
				_secondColor=_color4;
				_firstHalfPaint.Color=Color.ParseColor("#F7D23E");
				_secondHalfPaint.Color=Color.ParseColor("#34A350");
				_goesBackward=true;
				break;
			case ProgressStates.FOLDING_RIGHT:
				_firstColor=_color2;
				_secondColor=_color4;
				_firstHalfPaint.Color=Color.ParseColor ("#C93437");
				_secondHalfPaint.Color=Color.ParseColor("#34A350");
				_goesBackward=false;
				break;
			}
		}
		public override void Draw (Canvas canvas)
		{
			if (_currentState != null) {
				MakeCirclesProgress(canvas);
			}
		}
		private void MeasureCircleProgress(int width, int height) {
			_diameter = Math.Min(width, height);
			_half = _diameter / 2;
			_oval.Set(0, 0, _diameter, _diameter);
			_controlPointMinimum = -_diameter / 6;
			_controlPointMaximum = _diameter + _diameter / 6;
		}

		private void MakeCirclesProgress(Canvas canvas) {

			switch (_currentState) {
			case ProgressStates.FOLDING_DOWN:
			case ProgressStates.FOLDING_UP:
				DrawYMotion(canvas);
				break;
			case ProgressStates.FOLDING_RIGHT:
			case ProgressStates.FOLDING_LEFT:
				DrawXMotion(canvas);
				break;
			}

			canvas.DrawPath(_path, _abovePaint);
		}

		private void DrawXMotion(Canvas canvas) {
			canvas.DrawArc(_oval, 90, 180, true, _firstHalfPaint);
			canvas.DrawArc(_oval, -270, -180, true, _secondHalfPaint);
			_path.Reset();
			_path.MoveTo(_half, 0);
			_path.CubicTo(_axisValue, 0, _axisValue, _diameter, _half, _diameter);
		}

		private void DrawYMotion(Canvas canvas) {
			canvas.DrawArc(_oval, 0, -180, true, _firstHalfPaint);
			canvas.DrawArc(_oval, -180, -180, true, _secondHalfPaint);
			_path.Reset();
			_path.MoveTo(0, _half);
			_path.CubicTo(0, _axisValue, _diameter, _axisValue, _diameter, _half);
		}
		public override void SetAlpha (int alpha)
		{
			this._alpha = alpha;
			_firstHalfPaint.Alpha=alpha;
			_secondHalfPaint.Alpha=alpha;
			int targetAboveAlpha = (_alphaAboveDefault * alpha) / _alphaOpaque;
			_abovePaint.Alpha=targetAboveAlpha;
		}
		public override void SetColorFilter (ColorFilter cf)
		{
			this._colorFilter = cf;
			_firstHalfPaint.SetColorFilter(cf);
			_secondHalfPaint.SetColorFilter(cf);
			_abovePaint.SetColorFilter(cf);
		}
		public override int Opacity {
			get {
				return (int)Format.Translucent;
			}
		}
		void ICallback.InvalidateDrawable (Drawable who)
		{
			ICallback callback = Callback;
			if (callback != null) {
				callback.InvalidateDrawable(this);
			}
		}

		void ICallback.ScheduleDrawable (Drawable who, Java.Lang.IRunnable what, long when)
		{
			ICallback callback = Callback;
			if (callback != null) {
				callback.ScheduleDrawable(this, what, when);
			}
		}

		void ICallback.UnscheduleDrawable (Drawable who, Java.Lang.IRunnable what)
		{
			ICallback callback = Callback;
			if (callback != null) {
				callback.UnscheduleDrawable(this, what);
			}
		}
		public class Builder {
			private int[] mColors;

			public Builder(Context context){
				InitDefaults(context);
			}

			private void InitDefaults(Context context) {
				//Default values
				mColors = context.Resources.GetIntArray(Resource.Array.google_colors);
			}

			public Builder Colors(int[] colors) {
				if (colors == null || colors.Length == 0) {
					throw new Java.Lang.IllegalArgumentException("Your color array must contains at least 4 values");
				}

				mColors = colors;
				return this;
			}

			public Drawable Build() {
				return new FoldingCirclesDrawable(mColors);
			}
		}

	}
}

