
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

namespace HoloGraphLibrary
{
	public class LineGraph : View
	{
		private List<Line> lines = new List<Line>();
		private Paint paint = new Paint();
		private Paint txtPaint = new Paint();
		private float minY = 0, minX = 0;
		private float maxY = 0, maxX = 0;
		private bool isMaxYUserSet = false;
		private int lineToFill = -1;
		private int indexSelected = -1;
		private OnPointClickedListener listener;
		private Bitmap fullImage;
		private bool shouldUpdate = false;
		private bool showMinAndMax = false;
		private bool showHorizontalGrid1 = false;
		private string gridColor = "0xffffffff";
		public LineGraph(Context context):this(context,null){
		}
		public LineGraph(Context context, IAttributeSet attrs) : base(context, attrs){
			txtPaint.Color = Color.White;
			txtPaint.TextSize = 20;
			txtPaint.AntiAlias = true;
		}
		public void setGridColor(string color)
		{
			gridColor = color;
		}
		public void showHorizontalGrid(bool show)
		{
			showHorizontalGrid1 = show;
		}
		public void showMinAndMaxValues(bool show)
		{
			showMinAndMax = show;
		}
		public void setTextColor(Color color)
		{
			txtPaint.Color = color;
		}
		public void setTextSize(float s)
		{
			txtPaint.TextSize = s;
		}
		public void setMinY(float minY){
			this.minY = minY;
		}

		public void update()
		{
			shouldUpdate = true;
			Invalidate();
		}
		public void removeAllLines(){
			while (lines.Count > 0){
				lines.RemoveAt (0);
			}
			shouldUpdate = true;
			Invalidate();
		}

		public void addLine(Line line) {
			lines.Add(line);
			shouldUpdate = true;
			Invalidate();
		}
		public List<Line> getLines() {
			return lines;
		}
		public void setLineToFill(int indexOfLine) {
			this.lineToFill = indexOfLine;
			shouldUpdate = true;
			Invalidate();
		}
		public int getLineToFill(){
			return lineToFill;
		}
		public void setLines(List<Line> lines) {
			this.lines = lines;
		}
		public Line getLine(int index) {
			return lines.ElementAt (index);
		}
		public int getSize(){
			return lines.Count;
		}

		public void setRangeY(float min, float max) {
			minY = min;
			maxY = max;
			isMaxYUserSet = true;
		}
		public float getMaxY(){
			if (isMaxYUserSet){
				return maxY;
			} else {
				maxY = lines.ElementAt(0).getPoint(0).getY();
				foreach (Line line in lines) {
					foreach (LinePoint point in line.getPoints()) {
						if (point.getY() > maxY){
							maxY = point.getY();
						}
					}
				}
				return maxY;
			}

		}
		public float getMinY(){
			if (isMaxYUserSet){
				return minY;
			} else {
				float min = lines.ElementAt(0).getPoint(0).getY();
				foreach (Line line in lines) {
					foreach (LinePoint point in line.getPoints()) {
						if (point.getY() < min) min = point.getY();
					}
				}
				minY = min;
				return minY;
			}
		}
		public float getMaxX(){
			float max = lines.ElementAt(0).getPoint(0).getX();
			foreach (Line line in lines) {
				foreach (LinePoint point  in line.getPoints()) {
					if (point.getX() > max) max = point.getX();
				}
			}
			maxX = max;
			return maxX;

		}
		public float getMinX(){
			float max = lines.ElementAt(0).getPoint(0).getX();
			foreach (Line line in lines) {
				foreach (LinePoint point in line.getPoints()) {
					if (point.getX() < max) max = point.getX();
				}
			}
			maxX = max;
			return maxX;
		}

		protected override void OnDraw(Canvas ca)  {
			if (fullImage == null || shouldUpdate) {
				fullImage = Bitmap.CreateBitmap (Width, Height, Bitmap.Config.Argb8888);
				Canvas canvas = new Canvas(fullImage);
				String max = (int)maxY+"";// used to display max
				String min = (int)minY+"";// used to display min
				paint.Reset ();
				Path path = new Path();

				float bottomPadding = 1, topPadding = 0;
				float sidePadding = 10;
				if (this.showMinAndMax)
					sidePadding = txtPaint.MeasureText(max);

				float usableHeight = Height - bottomPadding - topPadding;
				float usableWidth = Width - sidePadding*2;
				float lineSpace = usableHeight/10;

				int lineCount = 0;
				foreach (Line line in lines) {
					int count = 0;
					float lastXPixels = 0, newYPixels;
					float lastYPixels = 0, newXPixels;
					float maxYd = getMaxY ();
					float minYd = getMinY();
					float maxXd = getMaxX();
					float minXd = getMinX();

					if (lineCount == lineToFill){
						paint.Color = Color.White;
						paint.Alpha = 30;	
						paint.StrokeWidth = 2;
						for (int i = 10; i-Width < Height; i = i+20){
							canvas.DrawLine(i, Height-bottomPadding, 0, Height-bottomPadding-i, paint);
						}

						paint.Reset();

						paint.SetXfermode (new PorterDuffXfermode (Android.Graphics.PorterDuff.Mode.Clear));
						foreach (LinePoint p  in line.getPoints()) {
							float yPercent = (p.getY()-minY)/(maxYd - minYd);
							float xPercent = (p.getX()-minX)/(maxXd - minXd);
							if (count == 0){
								lastXPixels = sidePadding + (xPercent*usableWidth);
								lastYPixels = Height - bottomPadding - (usableHeight*yPercent);
								path.MoveTo(lastXPixels, lastYPixels);
							} else {
								newXPixels = sidePadding + (xPercent*usableWidth);
								newYPixels = Height - bottomPadding - (usableHeight*yPercent);
								path.LineTo(newXPixels, newYPixels);
								Path pa = new Path();
								pa.MoveTo(lastXPixels, lastYPixels);
								pa.LineTo(newXPixels, newYPixels);
								pa.LineTo(newXPixels, 0);
								pa.LineTo(lastXPixels, 0);
								pa.Close();
								canvas.DrawPath(pa, paint);
								lastXPixels = newXPixels;
								lastYPixels = newYPixels;
							}
							count++;
						}

						path.Reset();

						path.MoveTo(0, Height-bottomPadding);
						path.LineTo(sidePadding, Height-bottomPadding);
						path.LineTo(sidePadding, 0);
						path.LineTo(0, 0);
						path.Close();
						canvas.DrawPath(path, paint);

						path.Reset();

						path.MoveTo(Width, Height-bottomPadding);
						path.LineTo(Width-sidePadding, Height-bottomPadding);
						path.LineTo(Width-sidePadding, 0);
						path.LineTo(Width, 0);
						path.Close();

						canvas.DrawPath(path, paint);

					}

					lineCount++;
				}

				paint.Reset();

				paint.Color = Color.White;
				//paint.setColor(this.gridColor);
				paint.Alpha = 50;
				paint.AntiAlias = true;
				canvas.DrawLine(sidePadding, Height - bottomPadding, Width, Height-bottomPadding, paint);
				if(this.showHorizontalGrid1)
					for(int i=1;i<=10;i++)
					{
						canvas.DrawLine(sidePadding, Height - bottomPadding-(i*lineSpace), Width, Height-bottomPadding-(i*lineSpace), paint);
					}
				paint.Alpha = 255;

				foreach (Line line in lines) {
					int count = 0;
					float lastXPixels = 0, newYPixels;
					float lastYPixels = 0, newXPixels;
					float maxYd = getMaxY();
					float minYd = getMinY();
					float maxXd = getMaxX();
					float minXd = getMinX();

					paint.Color = Color.Yellow;
					//paint.setColor(line.getColor());
					paint.StrokeWidth = 6;

					foreach (LinePoint p in line.getPoints()) {
						float yPercent = (p.getY()-minY)/(maxYd - minYd);
						float xPercent = (p.getX()-minX)/(maxXd - minXd);
						if (count == 0){
							lastXPixels = sidePadding + (xPercent*usableWidth);
							lastYPixels = Height - bottomPadding - (usableHeight*yPercent);
						} else {
							newXPixels = sidePadding + (xPercent*usableWidth);
							newYPixels = Height - bottomPadding - (usableHeight*yPercent);
							canvas.DrawLine(lastXPixels, lastYPixels, newXPixels, newYPixels, paint);
							lastXPixels = newXPixels;
							lastYPixels = newYPixels;
						}
						count++;
					}
				}


				int pointCount = 0;

				foreach (Line line  in lines) {
					float maxYd = getMaxY();
					float minYd = getMinY();
					float maxXd = getMaxX();
					float minXd = getMinX();

					paint.Color = Color.Yellow;
					//paint.setColor(line.getColor());
					paint.StrokeWidth = 6;
					paint.StrokeCap = Paint.Cap.Round;

					if (line.isShowingPoints()){
						foreach (LinePoint p in line.getPoints()) {
							float yPercent = (p.getY()-minYd)/(maxYd - minYd);
							float xPercent = (p.getX()-minXd)/(maxXd - minXd);
							float xPixels = sidePadding + (xPercent*usableWidth);
							float yPixels = Height - bottomPadding - (usableHeight*yPercent);

							paint.Color = Color.Gray;
							canvas.DrawCircle(xPixels, yPixels, 10, paint);
							paint.Color = Color.White;
							canvas.DrawCircle(xPixels, yPixels, 5, paint);

							Path path2 = new Path();
							path2.AddCircle(xPixels, yPixels, 30, Android.Graphics.Path.Direction.Cw);
							p.setPath(path2);
							p.setRegion(new Region((int)(xPixels-30), (int)(yPixels-30), (int)(xPixels+30), (int)(yPixels+30)));
							if (indexSelected == pointCount && listener != null){
								paint.Color=Color.ParseColor("#33B5E5");
								paint.Alpha = 100;
								canvas.DrawPath(p.getPath(), paint);
								paint.Alpha = 255;
							}

							pointCount++;
						}
					}
				}

				shouldUpdate = false;
				if (this.showMinAndMax) {
					ca.DrawText(max, 0, txtPaint.TextSize, txtPaint);
					ca.DrawText(min,0,this.Height,txtPaint);
				}
			}
			ca.DrawBitmap(fullImage, 0, 0, null);
		}
		public override bool OnTouchEvent (MotionEvent e)
		{
			Point point = new Point();
			point.X = (int)e.GetX ();
			point.Y = (int) e.GetY();

			int count = 0;
			int lineCount = 0;
			int pointCount;

			Region r = new Region();
			foreach (Line line in lines) {
				pointCount = 0;
				foreach (LinePoint p in line.getPoints()) {

					if (p.getPath() != null && p.getRegion() != null){
						r.SetPath(p.getPath(), p.getRegion());
						if (r.Contains(point.X, point.Y) && e.Action == MotionEventActions.Down) {
							indexSelected = count;
						} else if (e.Action == MotionEventActions.Up){
							if (r.Contains(point.X, point.Y) && listener != null) {
								listener.onClick(lineCount, pointCount);
							}
							indexSelected = -1;
						}
					}

					pointCount++;
					count++;
				}
				lineCount++;

			}

			if (e.Action == MotionEventActions.Down || e.Action ==MotionEventActions.Up){
				shouldUpdate = true;
				Invalidate();
			}

			return base.OnTouchEvent (e);
		}
		public void setOnPointClickedListener(OnPointClickedListener listener) {
			this.listener = listener;
		}

		public interface OnPointClickedListener {
			//abstract void onClick(int lineIndex, int pointIndex);
			void onClick(int lineIndex, int pointIndex);
		}
	}
}

