using Android.Support.V4.View;
using Android.Content;
using Android.Widget;
using Android.Util;
using Android.Views;
using Java.Lang;

namespace LibrarySlideDateTimePicker
{
	public class CustomViewPager : ViewPager
	{
		DatePicker _datePicker;
		TimePicker _timePicker;
		float _x1, _y1, _x2, _y2;
		float _touchSlop;

		public CustomViewPager(Context context):base(context)
		{
			init(context);
		}

		public CustomViewPager(Context context, IAttributeSet Attrs):base(context, Attrs)
		{
			init(context);
		}

		void init(Context context)
		{
			_touchSlop = ViewConfiguration.Get(context).ScaledPagingTouchSlop;
		}

		/**
     * Setting wrap_content on a ViewPager's layout_height in XML
     * doesn't seem to be recognized and the ViewPager will fill the
     * height of the screen regardless. We'll force the ViewPager to
     * have the same height as its immediate child.
     */
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			if (ChildCount > 0)
			{
				View childView = GetChildAt(0);

				if (childView != null)
				{
					childView.Measure(widthMeasureSpec, heightMeasureSpec);
					int height = childView.MeasuredHeight;
					SetMeasuredDimension(MeasuredWidth, height);
					LayoutParameters.Height = height;
				}
			}

			_datePicker = (DatePicker) FindViewById(Resource.Id.datePicker);
			_timePicker = (TimePicker) FindViewById(Resource.Id.timePicker);
		}
		/**
     * When the user swipes their finger horizontally, dispatch
     * those touch events to the ViewPager. When they swipe
     * vertically, dispatch those touch events to the date or
     * time picker (depending on which page we're currently on).
     *
     * @param event
     */
		public override bool DispatchTouchEvent (MotionEvent e)
		{
			switch (e.Action)
			{
			case MotionEventActions.Down:
				_x1 = e.GetX();
				_y1 = e.GetY();

				break;

			case MotionEventActions.Move:
				_x2 = e.GetX();
				_y2 = e.GetY();

				if (isScrollingHorizontal(_x1, _y1, _x2, _y2))
				{
					// When the user is scrolling the ViewPager horizontally,
					// block the pickers from scrolling vertically.
					return base.DispatchTouchEvent(e);
				}

				break;
			}

			// As long as the ViewPager isn't scrolling horizontally,
			// dispatch the event to the DatePicker or TimePicker,
			// depending on which page the ViewPager is currently on.

			switch (CurrentItem)
			{
			case 0:

				if (_datePicker != null)
					_datePicker.DispatchTouchEvent(e);

				break;

			case 1:

				if (_timePicker != null)
					_timePicker.DispatchTouchEvent(e);

				break;
			}

			// need this for the ViewPager to scroll horizontally at all
			return base.DispatchTouchEvent (e);
		}
		/**
     * Determine whether the distance between the user's ACTION_DOWN
     * event (x1, y1) and the current ACTION_MOVE event (x2, y2) should
     * be interpreted as a horizontal swipe.
     *
     * @param x1
     * @param y1
     * @param x2
     * @param y2
     * @return
     */
		bool isScrollingHorizontal(float X1, float Y1, float X2, float Y2)
		{
			float deltaX = X2 - X1;
			float deltaY = Y2 - X1;

			if (Math.Abs(deltaX) > _touchSlop &&
				Math.Abs(deltaX) > Math.Abs(deltaY))
			{

				return true;
			}

			return false;
		}
	}
}

