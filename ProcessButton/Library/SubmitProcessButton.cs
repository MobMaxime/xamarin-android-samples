
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
using Android.Util;

namespace Library
{
	public class SubmitProcessButton: ProcessButton
	{
		public SubmitProcessButton(Context context):base(context) {}

		public SubmitProcessButton(Context context, IAttributeSet attrs):base(context, attrs){}

		public SubmitProcessButton(Context context, IAttributeSet attrs, int defStyle):base(context, attrs, defStyle){}

		#region implemented abstract members of ProcessButton

		public override void drawProgress (Android.Graphics.Canvas canvas)
		{
			float scale = (float) getProgress() / (float) getMaxProgress();
			float indicatorWidth = (float) MeasuredWidth * scale;

			getProgressDrawable().SetBounds(0, 0, (int) indicatorWidth, MeasuredHeight);
			getProgressDrawable().Draw(canvas);
		}

		#endregion
	}
}

