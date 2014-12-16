
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
using Xamarin.NineOldAndroids.Views;

namespace CarouselViewProjectmaster
{			
	public class MyFragment : Android.Support.V4.App.Fragment {

		public MyFragment(){

		}

		public static Android.Support.V4.App.Fragment newInstance(MainActivity context, int pos, float scale,bool IsBlured)
		{

			Bundle b = new Bundle();
			b.PutInt("pos", pos);
			b.PutFloat("scale", scale);
			b.PutBoolean("IsBlured", IsBlured);	
			MyFragment myf = new MyFragment ();
			return Android.Support.V4.App.Fragment.Instantiate (context,myf.Class.Name, b);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if (container == null) {
				return null;
			}

			LinearLayout l = (LinearLayout)inflater.Inflate(Resource.Layout.mf, container, false);

			int pos = this.Arguments.GetInt("pos");

			TextView tv = (TextView) l.FindViewById(Resource.Id.viewID);
			tv.Text = "Position = " + pos;

			LinearLayout root = (LinearLayout) l.FindViewById(Resource.Id.root);
			float scale = this.Arguments.GetFloat("scale");
			bool isBlured=this.Arguments.GetBoolean("IsBlured");
			if(isBlured)
			{
				ViewHelper.SetAlpha(root,MyPagerAdapter.getMinAlpha());
				ViewHelper.SetRotationY(root, MyPagerAdapter.getMinDegree());
			}
			return l;
		}

	}
}
