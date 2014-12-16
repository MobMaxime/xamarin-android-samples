
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
	public class LinePoint
	{
		private float x = 0;
		private float y = 0;
		private Path path;
		private Region region;

		public LinePoint(float x, float y) {
			//super();
			this.x = x;
			this.y = y;
		}

		public LinePoint() { }

		public float getX() {
			return x;
		}

		public void setX(float x) {
			this.x = x;
		}

		public float getY() {
			return y;
		}

		public void setY(float y) {
			this.y = y;
		}

		public Region getRegion() {
			return region;
		}

		public void setRegion(Region region) {
			this.region = region;
		}

		public Path getPath() {
			return path;
		}

		public void setPath(Path path) {
			this.path = path;
		}
	}
}

