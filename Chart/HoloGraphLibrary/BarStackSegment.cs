
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
	public class BarStackSegment
	{
		public int Value;
		public Color Color;
		public BarStackSegment(int val, Color color){
			Value = val;
			Color = color;
		}
	}
}

