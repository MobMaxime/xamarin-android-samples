
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

namespace HoloGraphLibrary
{
	public class Line
	{
		private List<LinePoint> points = new List<LinePoint>();
		private Color color;
		private bool showPoints = true;


		public int getColor() {
			return color;
		}

		public void setColor(Color color) {
			this.color = color;
		}

		public List<LinePoint> getPoints() {
			return points;
		}

		public void setPoints(List<LinePoint> points) {
			this.points = points;
		}

		public void addPoint(LinePoint point) {
			points.Add(point);
		}

		public LinePoint getPoint(int index) {
			return points.ElementAt (index);
		}

		public int getSize() {
			return points.Count;
		}

		public bool isShowingPoints() {
			return showPoints;
		}

		public void setShowingPoints(bool showPoints) {
			this.showPoints = showPoints;
		}
	}
}

