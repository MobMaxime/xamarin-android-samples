
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
	[Activity (Label = "LineGraphActivity")]			
	public class LineGraphActivity : Activity,HoloGraphLibrary.LineGraph.OnPointClickedListener
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.LineGraph);
			Line l = new Line();
			LinePoint p = new LinePoint();
			p.setX(0);
			p.setY(5);
			l.addPoint(p);
			p = new LinePoint();
			p.setX(8);
			p.setY(8);
			l.addPoint(p);
			p = new LinePoint();
			p.setX(10);
			p.setY(4);
			l.addPoint(p);
			l.setColor(Color.ParseColor("#FFBB33"));
			
			LineGraph li = FindViewById<LineGraph> (Resource.Id.linegraph);//(LineGraph)v.findViewById(R.id.linegraph);
			li.addLine(l);
			li.setRangeY(0, 10);
			li.setLineToFill(0);
			li.setOnPointClickedListener(this);
		}
		#region OnPointClickedListener implementation
		public void onClick (int lineIndex, int pointIndex)
		{
			//throw new NotImplementedException ();
		}
		#endregion
	}
}

