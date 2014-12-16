
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
	public class PieGraph : View 
	{
		private List<PieSlice> slices = new List<PieSlice>();
		private Paint paint = new Paint();
		private Path path = new Path();

		private int indexSelected = -1;
		private int thickness = 50;
		private OnSliceClickedListener listener;


		public PieGraph(Context context):base(context) {
		}
		public PieGraph(Context context, IAttributeSet attrs):base(context,attrs) {
		}
		protected override void OnDraw(Canvas canvas)  {
			canvas.DrawColor(Color.Transparent);
			paint.Reset();
			paint.AntiAlias = true;
			float midX, midY, radius, innerRadius;
			path.Reset();

			float currentAngle = 270;
			float currentSweep;
			float totalValue = 0;
			float padding = 2;

			midX = Width / 2;
			midY = Height/2;
			if (midX < midY){
				radius = midX;
			} else {
				radius = midY;
			}
			radius -= padding;
			innerRadius = radius - thickness;

			foreach (PieSlice slice in slices) {
				totalValue += slice.getValue();
			}

			int count = 0;
			foreach (PieSlice slice in slices) {
				Path p = new Path();
				paint.Color = slice.getColor ();
				currentSweep = (slice.getValue()/totalValue)*(360);
				p.ArcTo(new RectF(midX-radius, midY-radius, midX+radius, midY+radius), currentAngle+padding, currentSweep - padding);
				p.ArcTo(new RectF(midX-innerRadius, midY-innerRadius, midX+innerRadius, midY+innerRadius), (currentAngle+padding) + (currentSweep - padding), -(currentSweep-padding));
				p.Close ();

				slice.setPath(p);
				slice.setRegion(new Region((int)(midX-radius), (int)(midY-radius), (int)(midX+radius), (int)(midY+radius)));
				canvas.DrawPath(p, paint);

				if (indexSelected == count && listener != null){
					path.Reset();
					paint.Color = slice.getColor ();
					paint.Color = Color.ParseColor ("#33B5E5");
					paint.Alpha=100;

					if (slices.Count > 1) {
						path.ArcTo(new RectF(midX-radius-(padding*2), midY-radius-(padding*2), midX+radius+(padding*2), midY+radius+(padding*2)), currentAngle, currentSweep+padding);
						path.ArcTo(new RectF(midX-innerRadius+(padding*2), midY-innerRadius+(padding*2), midX+innerRadius-(padding*2), midY+innerRadius-(padding*2)), currentAngle + currentSweep + padding, -(currentSweep + padding));
						path.Close();
					} else {
						path.AddCircle(midX, midY, radius+padding, Android.Graphics.Path.Direction.Cw);
					}

					canvas.DrawPath(path, paint);
					paint.Alpha=255;
				}

				currentAngle = currentAngle+currentSweep;

				count++;
			}
		}
		public override bool OnTouchEvent (MotionEvent e)
		{
			Point point = new Point();
			point.X = (int)e.GetX ();
			point.Y = (int)e.GetY ();

			int count = 0;
			foreach (PieSlice slice in slices) {
				Region r = new Region();
				r.SetPath(slice.getPath(), slice.getRegion());
				if (r.Contains(point.X, point.Y) && e.Action == MotionEventActions.Down) {
					indexSelected = count;
				} else if (e.Action == MotionEventActions.Up){
					if (r.Contains(point.X, point.Y) && listener != null) {
						if (indexSelected > -1){
							listener.onClick(indexSelected);
						}
						indexSelected = -1;
					}

				}
				count++;
			}

			if (e.Action == MotionEventActions.Down || e.Action == MotionEventActions.Up){
				Invalidate();
			}

			return base.OnTouchEvent (e);
		}

		public List<PieSlice> getSlices() {
			return slices;
		}
		public void setSlices(List<PieSlice> slices) {
			this.slices = slices;
			Invalidate();
		}
		public PieSlice getSlice(int index) {
			return slices.ElementAt(index);
		}
		public void addSlice(PieSlice slice) {
			this.slices.Add(slice);
			Invalidate();
		}

		public int getThickness() {
			return thickness;
		}
		public void setThickness(int thickness) {
			this.thickness = thickness;
			Invalidate();
		}

		public void removeSlices(){
			for (int i = slices.Count-1; i >= 0; i--){
				slices.RemoveAt (i);
			}
			Invalidate();
		}

		public void setOnSliceClickedListener(OnSliceClickedListener listener) {
			this.listener = listener;
		}
		public interface OnSliceClickedListener {
			void onClick(int index);
		}
	}
}

