using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using HoloGraphLibrary;
using Android.Graphics;
using System.Collections.Generic;

namespace Chart
{
	[Activity (Label = "Chart", MainLauncher = true)]
	public class MainActivity : Activity
	{
		Button btnLineGraph,btnBarGraph,btnPieGraph;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			btnLineGraph = FindViewById<Button> (Resource.Id.btnlinegraph);
			btnBarGraph = FindViewById<Button> (Resource.Id.btnbargraph);
			btnPieGraph = FindViewById<Button> (Resource.Id.btnpiegraph);

			btnLineGraph.Click += (sender, e) => {
				StartActivity(typeof(LineGraphActivity));
			};
			btnBarGraph.Click += (sender, e) => {
				StartActivity(typeof(BarGraphActivity));
			};
			btnPieGraph.Click += (sender, e) => {
				StartActivity(typeof(PieGraphActivity));
			};
		}

	}
}


