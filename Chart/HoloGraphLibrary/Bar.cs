
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
	public class Bar
	{
		private Color color;
		private String name;
		private float value;
		private Path path;
		private Region region;
		private bool isStackedBar;
		private List<BarStackSegment> values = new List<BarStackSegment>();

		public Color getColor() {
			return color;
		}
		public void setColor(Color color) {
			this.color = color;
		}
		public String getName() {
			return name;
		}
		public void setName(String name) {
			this.name = name;
		}
		public float getValue() {
			return value;
		}
		public void setValue(float value) {
			this.value = value;
		}
		public Path getPath() {
			return path;
		}
		public void setPath(Path path) {
			this.path = path;
		}
		public Region getRegion() {
			return region;
		}
		public void setRegion(Region region) {
			this.region = region;
		}
		public void setStackedBar(bool stacked){
			isStackedBar = stacked;
		}
		public bool getStackedBar(){
			return isStackedBar;
		}
		public void AddStackValue(BarStackSegment val){
			values.Add(val);
		}
		public List<BarStackSegment> getStackedValues(){
			return values;
		}
	}
}

