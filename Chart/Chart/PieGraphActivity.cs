
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
	[Activity (Label = "PieGraphActivity")]			
	public class PieGraphActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.PieGraph);

			PieGraph pg = FindViewById<PieGraph> (Resource.Id.piegraph);//(PieGraph)v.findViewById(R.id.piegraph);
			PieSlice slice = new PieSlice();
			slice.setColor(Color.ParseColor("#99CC00"));
			slice.setValue(2);
			pg.addSlice(slice);
			slice = new PieSlice();
			slice.setColor(Color.ParseColor("#FFBB33"));
			slice.setValue(3);
			pg.addSlice(slice);
			slice = new PieSlice();
			slice.setColor(Color.ParseColor("#AA66CC"));
			slice.setValue(8);
			pg.addSlice(slice);	
		}
	}
}

