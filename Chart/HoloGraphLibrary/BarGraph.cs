
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
using Android.Graphics.Drawables;
using Java.Util;


namespace HoloGraphLibrary
{
	public class BarGraph : View
	{
		private List<Bar> points = new List<Bar>();
		private Paint p = new Paint();
		private Path path = new Path();
		private Rect r;
		private bool showBarText = true;
		private int indexSelected = -1;
		private OnBarClickedListener listener;
		private Bitmap fullImage;
		private bool shouldUpdate = false;
		private String unit = "$";
		private Boolean append = false;
		private Rect r2 = new Rect();
		private Rect r3 = new Rect();

		public BarGraph(Context context):base(context) {
		}

		public BarGraph(Context context, IAttributeSet attrs):base(context,attrs) {
		}

		public void setShowBarText(bool show) {
			showBarText = show;
		}
		public void setBars(List<Bar> points) {
			this.points = points;
			Invalidate();
		}

		public void setUnit(String unit) {
			this.unit = unit;
		}

		public String getUnit() {
			return this.unit;
		}

		public void appendUnit(Boolean doAppend) {
			this.append = doAppend;
		}

		public Boolean isAppended() {
			return this.append;
		}

		public List<Bar> getBars() {
			return this.points;
		}
		protected override void OnDraw(Canvas ca)  {

			if (fullImage == null || shouldUpdate) {

				fullImage = Bitmap.CreateBitmap (Width, Height, Bitmap.Config.Argb8888);
				Canvas canvas = new Canvas(fullImage);
				canvas.DrawColor(Color.Transparent);
				//NinePatchDrawable popup = (NinePatchDrawable)this.Resources.GetDrawable (Android.Resource.Drawable.AlertDarkFrame);

				float maxValue = 0;
				float padding = 7;
				int selectPadding = 4;
				float bottomPadding = 40;
				float usableHeight;
				if (showBarText) {
					this.p.TextSize = 40;
					this.p.GetTextBounds(unit, 0, 1, r3);
					usableHeight = Height - bottomPadding - Math.Abs(r3.Top - r3.Bottom) - 26;
				} else {
					usableHeight = Height - bottomPadding;
				}

				p.Color = Color.Black;
				p.StrokeWidth = 2;
				p.Alpha = 50;
				p.AntiAlias = true;

				canvas.DrawLine(0, Height - bottomPadding + 10, Width, Height - bottomPadding + 10, p);

				float barWidth = (Width - (padding * 2) * points.Count) / points.Count;

				foreach (Bar po in points) {
					maxValue += po.getValue();
				}

				r = new Rect();

				path.Reset();

				int count = 0;
				foreach (Bar po in points) {

					if(po.getStackedBar()){
						List<BarStackSegment> values = new List<BarStackSegment>(po.getStackedValues());
						int prevValue = 0;
						foreach (BarStackSegment value in values) {
							value.Value += prevValue;
							prevValue += value.Value;
						}
						//Collections.Reverse(values);

						foreach (BarStackSegment value in values) {
							r.Set((int) ((padding * 2) * count + padding + barWidth * count), (int) ((Height - bottomPadding - (usableHeight * (value.Value / maxValue)))), (int) ((padding * 2) * count + padding + barWidth * (count + 1)), (int) ((Height - bottomPadding)));
							path.AddRect(new RectF(r.Left - selectPadding, r.Top - selectPadding, r.Right + selectPadding, r.Bottom + selectPadding), Path.Direction.Cw);
							po.setPath(path);
							po.setRegion(new Region(r.Left - selectPadding, r.Top - selectPadding, r.Right + selectPadding, r.Bottom + selectPadding));
							this.p.Color = value.Color;
							this.p.Alpha = 255;
							canvas.DrawRect (r, this.p);
						}
					}else {
						r.Set((int) ((padding * 2) * count + padding + barWidth * count), (int) (Height - bottomPadding - (usableHeight * (po.getValue() / maxValue))), (int) ((padding * 2) * count + padding + barWidth * (count + 1)), (int) (Height - bottomPadding));
						path.AddRect(new RectF(r.Left - selectPadding, r.Top - selectPadding, r.Right + selectPadding, r.Bottom + selectPadding), Path.Direction.Cw);
						po.setPath(path);
						po.setRegion(new Region(r.Left - selectPadding, r.Top - selectPadding, r.Right + selectPadding, r.Bottom + selectPadding));
						this.p.Color = po.getColor ();
						this.p.Alpha=255;
						canvas.DrawRect(r, this.p);
					}

					this.p.TextSize = 20;
					canvas.DrawText(po.getName(), (int) (((r.Left + r.Right) / 2) - (this.p.MeasureText(po.getName()) / 2)), Height - 5, this.p);
					if (showBarText) {
						this.p.TextSize=40;
						this.p.Color=Color.White;
						this.p.GetTextBounds (unit + po.getValue (), 0, 1, r2);
//						if (popup != null)
//							popup.SetBounds((int) (((r.Left + r.Right) / 2) - (this.p.MeasureText(unit + po.getValue()) / 2)) - 14, r.Top + (r2.Top - r2.Bottom) - 26, (int) (((r.Left + r.Right) / 2) + (this.p.MeasureText(unit + po.getValue()) / 2)) + 14, r.Top);
//						popup.Draw(canvas);
						if (isAppended())
							canvas.DrawText(po.getValue() + unit, (int) (((r.Left + r.Right) / 2) - (this.p.MeasureText(unit + po.getValue()) / 2)), r.Top - 20, this.p);
						else
							canvas.DrawText(unit + po.getValue(), (int) (((r.Left + r.Right) / 2) - (this.p.MeasureText(unit + po.getValue()) / 2)), r.Top - 20, this.p);
					}
					if (indexSelected == count && listener != null) {
						this.p.Color = Color.ParseColor ("#33B5E5");
						this.p.Alpha = 100;
						canvas.DrawPath (po.getPath (), this.p);
						this.p.Alpha = 255;
					}
					count++;
				}
				shouldUpdate = false;
			}
			ca.DrawBitmap(fullImage, 0, 0, null);
		}
		public override bool OnTouchEvent (MotionEvent e)
		{
			Point point = new Point();
			point.X = (int)e.GetX ();
			point.Y = (int) e.GetY();

			int count = 0;
			foreach (Bar bar in points) {
				Region r = new Region();
				r.SetPath(bar.getPath(), bar.getRegion());
				if (r.Contains(point.X, point.Y) && e.Action == MotionEventActions.Down) {
					indexSelected = count;
				} else if (e.Action== MotionEventActions.Up) {
					if (r.Contains(point.X, point.Y) && listener != null) {
						listener.onClick(indexSelected);
					}
					indexSelected = -1;
				}
				count++;
			}

			if (e.Action == MotionEventActions.Down || e.Action == MotionEventActions.Up) {
				shouldUpdate = true;
				Invalidate();
			}


			return base.OnTouchEvent (e);
		}
		public void setOnBarClickedListener(OnBarClickedListener listener) {
			this.listener = listener;
		}

		public interface OnBarClickedListener {
			 void onClick(int index);
		}
	}
}

