using System;
using Android.Widget;
using Android.Graphics;
using Android.Content;
using Android.Util;
using Android.Views;

namespace LibraryFlabbyListView
{
	public class FlabbyLayout : FrameLayout
	{
		private static float _maxCurvature = 100;
		private Path _path;
		private Paint _paint;
		private Rect _rect;
		private float _deltaY = 0;
		private float _curvature;
		private int _width;
		private int _height;
		private int _oneFifthWidth;
		private int _fourFifthWith;
		private bool _isUserTouching = false;
		private float _fingerX = 0;
		private bool _isSelectedView = false;


		public float Curvature
		{
			get{
				return this._curvature;
			}
			set {
				this._curvature = value;
				Invalidate ();
			}
		}

		public float DeltaY
		{
			get{
				return this._deltaY;
			}
			set {
				this._deltaY = value;
				Invalidate ();
			}
		}
		public int OneFifthWidth
		{
			get{
				return this._oneFifthWidth;
			}
			set {
				this._oneFifthWidth = value;
				Invalidate ();
			}
		}
		public int FourFifthWith
		{
			get{
				return this._fourFifthWith;
			}
			set {
				this._fourFifthWith = value;
				Invalidate ();
			}
		}

		public FlabbyLayout(Context context):base(context) {
			Init(context);
		}

		public FlabbyLayout(Context context, IAttributeSet attrs) :base(context, attrs){
			Init(context);
		}

		public FlabbyLayout(Context context, IAttributeSet attrs, int defStyleAttr) :base(context, attrs, defStyleAttr){
			Init(context);
		}

		private void Init(Context context) {
			SetWillNotDraw(false);
			_path = new Path();
			_paint = new Paint(PaintFlags.AntiAlias);
			_paint.SetStyle(Paint.Style.Fill);
		}
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			_width = Width;
			_height =Height;
			OneFifthWidth = _width / 5;
			FourFifthWith = OneFifthWidth * 4;
		}
		protected override void OnDraw (Canvas canvas)
		{
			_rect = canvas.ClipBounds;
			_rect.Inset(0, -_height / 2);
			canvas.ClipRect(_rect, Region.Op.Replace);

			if (!_isUserTouching) {
				if (DeltaY > -_maxCurvature && DeltaY < _maxCurvature) Curvature = DeltaY * 2;
				TopCellPath(OneFifthWidth, FourFifthWith, Curvature);
				BottomCellPath(FourFifthWith, OneFifthWidth, _height + Curvature);
			} else {
				float Curvatured = _isSelectedView?-Curvature:Curvature;
				TopCellPath(_fingerX,_fingerX,Curvatured);
				Curvatured = _isSelectedView?_height-Curvatured:_height;
				BottomCellPath(_fingerX,_fingerX,Curvatured);
			}
			canvas.DrawPath(_path, _paint);
		}
		private Path TopCellPath(float x1, float x2, float curvature) {
			_path.Reset();
			_path.MoveTo(0, 0);
			_path.CubicTo(x1, curvature, x2, curvature, _width, 0);
			_path.LineTo(_width, _height);
			return _path;
		}

		private Path BottomCellPath(float x1, float x2, float curvature) {
			_path.CubicTo(x1,  curvature, x2, curvature, 0, _height);
			_path.LineTo(0, 0);
			return null;
		}
		public override bool OnTouchEvent (MotionEvent e)
		{
			switch (e.Action) {
			case MotionEventActions.Down:
				ActionDown(e);
				break;
			case MotionEventActions.Move:
				ActionMove(e);
				break;
			case MotionEventActions.Up:
				ActionUp();
				break;
			}
			return base.OnTouchEvent (e);
		}
		private void ActionDown(MotionEvent e) {
			Curvature = _maxCurvature;
			_fingerX = e.GetX();
			_isUserTouching = true;
		}

		private void ActionMove(MotionEvent e) {
			if (_fingerX != e.GetX()) {
				RequestLayout();
			}
			_fingerX = e.GetX();
		}

		private void ActionUp() {
			_isUserTouching = false;
			Curvature = 0;
			Invalidate();
		}

		public void UpdateControlPoints(float deltaY) {
			DeltaY = deltaY;
			Invalidate();
		}

		public void SetFlabbyColor(Color color) {
			_paint.Color = color;
		}

		public void SetAsSelected(bool isSelected) {
			_isSelectedView = isSelected;
		}
	}
}

