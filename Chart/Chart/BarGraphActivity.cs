
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
using HoloGraphLibrary;
using Android.Graphics;

namespace Chart
{
	[Activity (Label = "BarGraphActivity")]			
	public class BarGraphActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.BarGraph);

			//assert v != null;
			List<Bar> points = new List<Bar>();

			for (int i = 1; i <= 20; i++) {
				Bar d = new Bar();
				d.setColor(Color.ParseColor("#99CC00"));
				d.setName("Test"+i);
				d.setValue(i*10);
				points.Add(d);
			}

//			Bar d = new Bar();
			//			d.setColor(Color.ParseColor("#99CC00"));
			//			d.setName("Test1");
			//			d.setValue(10);

//			Bar d2 = new Bar();
//			d2.setColor(Color.ParseColor("#FFBB33"));
//			d2.setName("Test2");
//			d2.setValue(20);
//
//			Bar d3 = new Bar();
//			d3.setColor(Color.BlueViolet);
//			d3.setName("Test3");
//			d3.setValue(30);
//
//			Bar d4 = new Bar();
//			d4.setColor(Color.LightSalmon);
//			d4.setName("Test4");
//			d4.setValue(40);
//
//			Bar d5 = new Bar();
//			d5.setColor(Color.Chocolate);
//			d5.setName("Test5");
//			d5.setValue(50);
//
//
//
////			Bar d3 = new Bar();
////			d3.setColor(Color.Red);
////			d3.setName("Test3");
////			d3.setValue (0);
////			d3.setStackedBar(true);
////			d3.AddStackValue(new BarStackSegment(2, Color.ParseColor("#FFBB33")));
////			d3.AddStackValue(new BarStackSegment(4, Color.Red));
//
//
//			points.Add(d);
//			points.Add(d2);
//			points.Add(d3);
//			points.Add(d4);
//			points.Add(d5);

			BarGraph g = FindViewById<BarGraph> (Resource.Id.bargraph);//v.findViewById(R.id.bargraph);
			//assert g != null;
			g.setUnit("€");
			g.appendUnit(true);
			g.setBars(points);
		}

	}
}

