using System;
using Android.Widget;
using Android.Graphics;
using Android.Views;
using Android.Content;
using Android.Util;
using Android.Graphics.Drawables;
using Android.App;
using System.Collections.Generic;
using Android.Views.Animations;

namespace PopOver
{
	public class PopoverView : RelativeLayout, Android.Views.View.IOnTouchListener
	{


		public interface PopoverViewDelegate
		{
			/**
			 * Called when the popover is going to show
			 * @param view The whole popover view
			 */
			void popoverViewWillShow (PopoverView view);

			/**
			 * Called when the popover did show
			 * @param view The whole popover view
			 */
			void popoverViewDidShow (PopoverView view);

			/**
			 * Called when the popover is going to be dismissed
			 * @param view The whole popover view
			 */
			void popoverViewWillDismiss (PopoverView view);

			/**
			 * Called when the popover was dismissed
			 * @param view The whole popover view
			 */
			void popoverViewDidDismiss (PopoverView view);
		}

		//********************************************************************
		// STATIC MEMBERS
		//********************************************************************
		/**
			 * Popover arrow points up. Integer to use with bit operators to tell the popover where the arrow should appear and from where the popover should appear
			 */
		public static int PopoverArrowDirectionUp = 0x00000001;
		/**
			 * Popover arrow points down. Integer to use with bit operators to tell the popover where the arrow should appear and from where the popover should appear
			 */
		public static int PopoverArrowDirectionDown = 0x00000002;
		/**
			 * Popover arrow points left. Integer to use with bit operators to tell the popover where the arrow should appear and from where the popover should appear
			 */
		public  static int PopoverArrowDirectionLeft = 0x00000004;
		/**
			 * Popover arrow points right. Integer to use with bit operators to tell the popover where the arrow should appear and from where the popover should appear
			 */
		public  static int PopoverArrowDirectionRight = 0x00000008;
		/**
			 * Popover arrow points any direction. Integer to use with bit operators to tell the popover where the arrow should appear and from where the popover should appear
			 */
		public  static int PopoverArrowDirectionAny = PopoverArrowDirectionUp | PopoverArrowDirectionDown | PopoverArrowDirectionLeft | PopoverArrowDirectionRight;
		/**
			 * The default popover background drawable for all the popovers
			 */
		public static int defaultPopoverBackgroundDrawable = Resource.Drawable.background_popover;
		/**
			 * The default popover arrow up drawable for all the popovers
			 */
		public static int defaultPopoverArrowUpDrawable = Resource.Drawable.icon_popover_arrow_up;
		/**
			 * The default popover arrow down drawable for all the popovers
			 */
		public static int defaultPopoverArrowDownDrawable = Resource.Drawable.icon_popover_arrow_down;
		/**
			 * The default popover arrow left drawable for all the popovers
			 */
		public static int defaultPopoverArrowLeftDrawable = Resource.Drawable.icon_popover_arrow_left;
		/**
			 * The default popover arrow down drawable for all the popovers
			 */
		public static int defaultPopoverArrowRightDrawable = Resource.Drawable.icon_popover_arrow_right;






		//********************************************************************
		// STATIC METHODS
		//********************************************************************
		/**
			 * Get the Rect frame for a view (relative to the Window of the application)
			 * @param v The view to get the rect from
			 * @return The rect of the view, relative to the application window
			 */
		public static Rect getFrameForView (View v)
		{
			int[] location = new int [2];
			v.GetLocationOnScreen (location);
			Rect viewRect = new Rect (location [0], location [1], location [0] + v.Width, location [1] + v.Height);
			return viewRect;
		}








		//********************************************************************
		// MEMBERS
		//********************************************************************
		/**
			 * The delegate of the view
			 */
		private PopoverViewDelegate del;
		/**
			 * The main popover containing the view we want to show
			 */
		private RelativeLayout popoverView;
		/**
			 * The view group storing this popover. We need this so, when we dismiss the popover, we remove it from the view group
			 */
		private ViewGroup superview;
		/**
			 * The content size for the view in the popover
			 */
		private Point contentSizeForViewInPopover = new Point (0, 0);
		/**
			 * The real content size we will use (it considers the padding)
			 */
		private Point realContentSize = new Point (0, 0);
		/**
			 * A hash containing
			 */
		private Dictionary<int, Rect> possibleRects;
		/**
			 * Whether the view is animating or not
			 */
		private bool isAnimating = false;
		/**
			 * The fade animation time in milliseconds
			 */
		private int fadeAnimationTime = 300;
		/**
			 * The layout Rect, is the same as the superview rect
			 */
		private Rect popoverLayoutRect;
		/**
			 * The popover background drawable
			 */
		private int popoverBackgroundDrawable;
		/**
			 * The popover arrow up drawable
			 */
		private int popoverArrowUpDrawable;
		/**
			 * The popover arrow down drawable
			 */
		private int popoverArrowDownDrawable;
		/**
			 * The popover arrow left drawable
			 */
		private int popoverArrowLeftDrawable;
		/**
			 * The popover arrow down drawable
			 */
		private int popoverArrowRightDrawable;











		//********************************************************************
		// CONSTRUCTORS
		//********************************************************************
		/**
	 * Constructor to create a popover with a popover view
	 * @param context The context where we should create the popover view
	 * @param layoutId The ID of the layout we want to put inside the popover
	 */
		public PopoverView (Context context, int layoutId) : base (context)
		{
			initPopoverView (Inflate (context, layoutId, null));
		}

		/**
	 * Constructor to create a popover with a popover view
	 * @param context The context where we should create the popover view
	 * @param attrs Attribute set to init the view
	 * @param layoutId The ID of the layout we want to put inside the popover
	 */
		public PopoverView (Context context, IAttributeSet attrs, int layoutId) : base (context, attrs)
		{
			initPopoverView (Inflate (context, layoutId, null));
		}

		/**
	 * Constructor to create a popover with a popover view
	 * @param context The context where we should create the popover view
	 * @param attrs Attribute set to init the view
	 * @param defStyle The default style for this view
	 * @param layoutId The ID of the layout we want to put inside the popover
	 */
		public PopoverView (Context context, IAttributeSet attrs, int defStyle, int layoutId) : base (context, attrs, defStyle)
		{
			initPopoverView (Inflate (context, layoutId, null));
		}

		/**
	 * Constructor to create a popover with a popover view
	 * @param context The context where we should create the popover view
	 * @param popoverView The inner view we want to show in a popover
	 */
		public PopoverView (Context context, View popoverView) : base (context)
		{
			initPopoverView (popoverView);
		}

		/**
	 * Constructor to create a popover with a popover view
	 * @param context The context where we should create the popover view
	 * @param attrs Attribute set to init the view
	 * @param popoverView The inner view we want to show in a popover
	 */
		public PopoverView (Context context, IAttributeSet attrs, View popoverView) : base (context, attrs)
		{
			initPopoverView (popoverView);
		}

		/**
	 * Constructor to create a popover with a popover view
	 * @param context The context where we should create the popover view
	 * @param attrs Attribute set to init the view
	 * @param defStyle The default style for this view
	 * @param popoverView The inner view we want to show in a popover
	 */
		public PopoverView (Context context, IAttributeSet attrs, int defStyle, View popoverView) : base (context, attrs, defStyle)
		{
			initPopoverView (popoverView);
		}

		/**
	 * Init the popover view
	 * @param viewToEnclose The view we wan to insert inside the popover
	 */
		private void initPopoverView (View viewToEnclose)
		{

			//Configure self
			SetBackgroundColor (Color.Transparent);
			//setOnClickListener(this);
			SetOnTouchListener (this);

			//Set initial drawables
			popoverBackgroundDrawable = PopoverView.defaultPopoverBackgroundDrawable;
			popoverArrowUpDrawable = PopoverView.defaultPopoverArrowUpDrawable;
			popoverArrowDownDrawable = PopoverView.defaultPopoverArrowDownDrawable;
			popoverArrowLeftDrawable = PopoverView.defaultPopoverArrowLeftDrawable;
			popoverArrowRightDrawable = PopoverView.defaultPopoverArrowRightDrawable;

			//Init the relative layout
			popoverView = new RelativeLayout (Application.Context);
			popoverView.SetBackgroundDrawable (Resources.GetDrawable (popoverBackgroundDrawable));
			popoverView.AddView (viewToEnclose, LayoutParams.MatchParent, LayoutParams.MatchParent);

		}











		//********************************************************************
		// PRIVATE METHODS
		//********************************************************************
		/**
	 * Add the popover to the view with a defined rect inside the popover
	 * @param insertRect The rect we want to insert the view
	 */
		private void addPopoverInRect (Rect insertRect)
		{
			//Set layout params
			LayoutParams insertParams = new LayoutParams (insertRect.Width (), insertRect.Height ());
			insertParams.LeftMargin = insertRect.Left;
			insertParams.TopMargin = insertRect.Top;
			//Add the view
			AddView (popoverView, insertParams);

		}


		private void addArrow (Rect originRect, int arrowDirection)
		{
			//Add arrow drawable
			ImageView arrowImageView = new ImageView (Application.Context);
			Drawable arrowDrawable = null;
			int xPos = 0;
			int arrowWidth = 0;
			int yPos = 0;
			int arrowHeight = 0;
			//Get correct drawable, and get Width, Height, Xpos and yPos depending on the selected arrow direction
			if (arrowDirection == PopoverView.PopoverArrowDirectionUp) {
				arrowDrawable = Resources.GetDrawable (popoverArrowUpDrawable);
				arrowWidth = arrowDrawable.IntrinsicWidth;
				arrowHeight = arrowDrawable.IntrinsicHeight;
				xPos = originRect.CenterX () - (arrowWidth / 2) - popoverLayoutRect.Left;
				yPos = originRect.Bottom - popoverLayoutRect.Top;
			} else if (arrowDirection == PopoverView.PopoverArrowDirectionDown) {
				arrowDrawable = Resources.GetDrawable (popoverArrowDownDrawable);
				arrowWidth = arrowDrawable.IntrinsicWidth;
				arrowHeight = arrowDrawable.IntrinsicHeight;
				xPos = originRect.CenterX () - (arrowWidth / 2) - popoverLayoutRect.Left;
				yPos = originRect.Top - arrowHeight - popoverLayoutRect.Top;
			} else if (arrowDirection == PopoverView.PopoverArrowDirectionLeft) {
				arrowDrawable = Resources.GetDrawable (popoverArrowLeftDrawable);
				arrowWidth = arrowDrawable.IntrinsicWidth;
				arrowHeight = arrowDrawable.IntrinsicHeight;
				xPos = originRect.Right - popoverLayoutRect.Left;
				yPos = originRect.CenterY () - (arrowHeight / 2) - popoverLayoutRect.Top;
			} else if (arrowDirection == PopoverView.PopoverArrowDirectionRight) {
				arrowDrawable = Resources.GetDrawable (popoverArrowRightDrawable);
				arrowWidth = arrowDrawable.IntrinsicWidth;
				arrowHeight = arrowDrawable.IntrinsicHeight;
				xPos = originRect.Left - arrowWidth - popoverLayoutRect.Left;
				yPos = originRect.CenterY () - (arrowHeight / 2) - popoverLayoutRect.Top;
			}
			//Set drawable
			arrowImageView.SetImageDrawable (arrowDrawable);
			//Init layout params
			LayoutParams arrowParams = new LayoutParams (arrowWidth, arrowHeight);
			arrowParams.LeftMargin = xPos;
			arrowParams.TopMargin = yPos;
			//add view :)
			AddView (arrowImageView, arrowParams);
		}


		/**
	 * Calculates the rect for showing the view with Arrow Up
	 * @param originRect The origin rect
	 * @return The calculated rect to show the view
	 */
		private Rect getRectForArrowUp (Rect originRect)
		{

			//Get available space		
			int xAvailable = popoverLayoutRect.Width ();
			if (xAvailable < 0)
				xAvailable = 0;
			int yAvailable = popoverLayoutRect.Height () - (originRect.Bottom - popoverLayoutRect.Top);
			if (yAvailable < 0)
				yAvailable = 0;

			//Get final width and height
			int finalX = xAvailable;
			if ((realContentSize.X > 0) && (realContentSize.X < finalX))
				finalX = realContentSize.X;
			int finalY = yAvailable;
			if ((realContentSize.Y > 0) && (realContentSize.Y < finalY))
				finalY = realContentSize.Y;

			//Get final origin X and Y
			int originX = (originRect.CenterX () - popoverLayoutRect.Left) - (finalX / 2);
			if (originX < 0)
				originX = 0;
			else if (originX + finalX > popoverLayoutRect.Width ())
				originX = popoverLayoutRect.Width () - finalX;
			int originY = (originRect.Bottom - popoverLayoutRect.Top);

			//Create rect
			Rect finalRect = new Rect (originX, originY, originX + finalX, originY + finalY);
			//And return
			return finalRect;

		}

		/**
	 * Calculates the rect for showing the view with Arrow Down
	 * @param originRect The origin rect
	 * @return The calculated rect to show the view
	 */
		private Rect getRectForArrowDown (Rect originRect)
		{

			//Get available space		
			int xAvailable = popoverLayoutRect.Width ();
			if (xAvailable < 0)
				xAvailable = 0;
			int yAvailable = (originRect.Top - popoverLayoutRect.Top);
			if (yAvailable < 0)
				yAvailable = 0;

			//Get final width and height
			int finalX = xAvailable;
			if ((realContentSize.X > 0) && (realContentSize.X < finalX))
				finalX = realContentSize.X;
			int finalY = yAvailable;
			if ((realContentSize.Y > 0) && (realContentSize.Y < finalY))
				finalY = realContentSize.Y;

			//Get final origin X and Y
			int originX = (originRect.CenterX () - popoverLayoutRect.Left) - (finalX / 2);
			if (originX < 0)
				originX = 0;
			else if (originX + finalX > popoverLayoutRect.Width ())
				originX = popoverLayoutRect.Width () - finalX;
			int originY = (originRect.Top - popoverLayoutRect.Top) - finalY;

			//Create rect
			Rect finalRect = new Rect (originX, originY, originX + finalX, originY + finalY);
			//And return
			return finalRect;

		}


		/**
	 * Calculates the rect for showing the view with Arrow Right
	 * @param originRect The origin rect
	 * @return The calculated rect to show the view
	 */
		private Rect getRectForArrowRight (Rect originRect)
		{
			//Get available space		
			int xAvailable = (originRect.Left - popoverLayoutRect.Left);
			if (xAvailable < 0)
				xAvailable = 0;
			int yAvailable = popoverLayoutRect.Height ();
			if (yAvailable < 0)
				yAvailable = 0;

			//Get final width and height
			int finalX = xAvailable;
			if ((realContentSize.X > 0) && (realContentSize.X < finalX))
				finalX = realContentSize.X;
			int finalY = yAvailable;
			if ((realContentSize.Y > 0) && (realContentSize.Y < finalY))
				finalY = realContentSize.Y;

			//Get final origin X and Y
			int originX = (originRect.Left - popoverLayoutRect.Left) - finalX;
			int originY = (originRect.CenterY () - popoverLayoutRect.Top) - (finalY / 2);
			if (originY < 0)
				originY = 0;
			else if (originY + finalY > popoverLayoutRect.Height ())
				originY = popoverLayoutRect.Height () - finalY;

			//Create rect
			Rect finalRect = new Rect (originX, originY, originX + finalX, originY + finalY);
			//And return
			return finalRect;
		}

		/**
	 * Calculates the rect for showing the view with Arrow Left
	 * @param originRect The origin rect
	 * @return The calculated rect to show the view
	 */
		private Rect getRectForArrowLeft (Rect originRect)
		{
			//Get available space		
			int xAvailable = popoverLayoutRect.Width () - (originRect.Right - popoverLayoutRect.Left);
			if (xAvailable < 0)
				xAvailable = 0;
			int yAvailable = popoverLayoutRect.Height ();
			if (yAvailable < 0)
				yAvailable = 0;

			//Get final width and height
			int finalX = xAvailable;
			if ((realContentSize.X > 0) && (realContentSize.X < finalX))
				finalX = realContentSize.X;
			int finalY = yAvailable;
			if ((realContentSize.Y > 0) && (realContentSize.Y < finalY))
				finalY = realContentSize.Y;

			//Get final origin X and Y
			int originX = (originRect.Right - popoverLayoutRect.Left);
			int originY = (originRect.CenterY () - popoverLayoutRect.Top) - (finalY / 2);
			if (originY < 0)
				originY = 0;
			else if (originY + finalY > popoverLayoutRect.Height ())
				originY = popoverLayoutRect.Height () - finalY;

			//Create rect
			Rect finalRect = new Rect (originX, originY, originX + finalX, originY + finalY);
			//And return
			return finalRect;
		}


		/**
	 * Add available rects for each selected arrow direction
	 * @param originRect The rect where the popover will appear from
	 * @param arrowDirections The bit mask for the possible arrow directions
	 */
		private void addAvailableRects (Rect originRect, int arrowDirections)
		{
			//Get popover rects for the available directions
			possibleRects = new Dictionary<int, Rect> ();
			if ((arrowDirections & PopoverView.PopoverArrowDirectionUp) != 0) {
//					possibleRects.put(PopoverView.PopoverArrowDirectionUp, getRectForArrowUp(originRect));
				possibleRects.Add (PopoverView.PopoverArrowDirectionUp, getRectForArrowUp (originRect));
			}
			if ((arrowDirections & PopoverView.PopoverArrowDirectionDown) != 0) {
				possibleRects.Add (PopoverView.PopoverArrowDirectionDown, getRectForArrowDown (originRect));
			}
			if ((arrowDirections & PopoverView.PopoverArrowDirectionRight) != 0) {
				possibleRects.Add (PopoverView.PopoverArrowDirectionRight, getRectForArrowRight (originRect));
			}
			if ((arrowDirections & PopoverView.PopoverArrowDirectionLeft) != 0) {
				possibleRects.Add (PopoverView.PopoverArrowDirectionLeft, getRectForArrowLeft (originRect));
			}

		}

		/**
	 * Get the best available rect (bigger area)
	 * @return The Integer key to get the Rect from posibleRects (PopoverArrowDirectionUp,PopoverArrowDirectionDown,PopoverArrowDirectionRight or PopoverArrowDirectionLeft)
	 */
		private int getBestRect ()
		{
			//Get the best one (bigger area)
			int best = 0;
//				for (Integer arrowDir : possibleRects.keySet()) {

			foreach (int arrowDir in possibleRects.Keys) {
				if (best == 0) {
					best = arrowDir;	
				} else {

					Rect bestRect = null;// = possibleRects.get(best);
					possibleRects.TryGetValue (best, out bestRect);
						
					Rect checkRect = null;// = possibleRects.get(arrowDir);
					possibleRects.TryGetValue (arrowDir, out checkRect);

					if ((bestRect.Width () * bestRect.Height ()) < (checkRect.Width () * checkRect.Height ()))
						best = arrowDir;
				}
			}
			return best;
		}











		//********************************************************************
		// GETTERS AND SETTERS
		//********************************************************************
		/**
	 * Gets the current fade animation time
	 * @return The fade animation time, in milliseconds
	 */
		public int getFadeAnimationTime ()
		{
			return fadeAnimationTime;
		}

		/**
	 * Sets the fade animation time
	 * @param fadeAnimationTime The time in milliseconds
	 */
		public void setFadeAnimationTime (int fadeAnimationTime)
		{
			this.fadeAnimationTime = fadeAnimationTime;
		}

		/**
	 * Get the content size for view in popover
	 * @return The point with the content size
	 */
		public Point getContentSizeForViewInPopover ()
		{
			return contentSizeForViewInPopover;
		}

		/**
	 * Sets the content size for the view in a popover, if point is (0,0) the popover will full the screen
	 * @param contentSizeForViewInPopover
	 */
		public void setContentSizeForViewInPopover (Point contentSizeForViewInPopover)
		{
			this.contentSizeForViewInPopover = contentSizeForViewInPopover;
			//Save the real content size
			realContentSize = new Point (contentSizeForViewInPopover);
			realContentSize.X += popoverView.PaddingLeft + popoverView.PaddingRight;
			realContentSize.Y += popoverView.PaddingTop + popoverView.PaddingBottom;

		}

		//			/**
		//			 * Gets the current delegate
		//			 * @return The current delegate
		//			 */
		//			public PopoverViewDelegate getDelegate() {
		//				return del;
		//			}
		//
		//			/**
		//			 * Sets the popover delegate
		//			 * @param delegate The new popover delegate
		//			 */
		//			public void setDelegate(PopoverViewDelegate del) {
		//				this.del = del;
		//			}
		public PopoverViewDelegate Del {
			get{ return del; }
			set{ del = value; }
		}
		public ViewGroup Superview{
			get{ return superview;}
			set{ superview = value;}
		}

		/**
	 * @return Current background drawable
	 */
		public int getPopoverBackgroundDrawable ()
		{
			return popoverBackgroundDrawable;
		}

		/**
	 * @param popoverBackgroundDrawable The new background drawable
	 */
		public void setPopoverBackgroundDrawable (int popoverBackgroundDrawable)
		{
			this.popoverBackgroundDrawable = popoverBackgroundDrawable;
		}

		/**
	 * @return Current arrow up drawable
	 */
		public int getPopoverArrowUpDrawable ()
		{
			return popoverArrowUpDrawable;
		}

		/**
	 * @param popoverArrowUpDrawable The new arrow up drawable
	 */
		public void setPopoverArrowUpDrawable (int popoverArrowUpDrawable)
		{
			this.popoverArrowUpDrawable = popoverArrowUpDrawable;
		}

		/**
	 * @return Current arrow down drawable
	 */
		public int getPopoverArrowDownDrawable ()
		{
			return popoverArrowDownDrawable;
		}

		/**
	 * @param popoverArrowDownDrawable The new arrow down drawable
	 */
		public void setPopoverArrowDownDrawable (int popoverArrowDownDrawable)
		{
			this.popoverArrowDownDrawable = popoverArrowDownDrawable;
		}

		/**
	 * @return Current arrow left drawable
	 */
		public int getPopoverArrowLeftDrawable ()
		{
			return popoverArrowLeftDrawable;
		}

		/**
	 * @param popoverArrowLeftDrawable The new arrow left drawable
	 */
		public void setPopoverArrowLeftDrawable (int popoverArrowLeftDrawable)
		{
			this.popoverArrowLeftDrawable = popoverArrowLeftDrawable;
		}

		/**
	 * @return Current arrow right drawable
	 */
		public int getPopoverArrowRightDrawable ()
		{
			return popoverArrowRightDrawable;
		}

		/**
	 * @param popoverArrowRightDrawable The new arrow right drawable
	 */
		public void setPopoverArrowRightDrawable (int popoverArrowRightDrawable)
		{
			this.popoverArrowRightDrawable = popoverArrowRightDrawable;
		}












		//********************************************************************
		// PUBLIC METHODS
		//********************************************************************
		/**
	 * This method shows a popover in a ViewGroup, from an origin rect (relative to the Application Window)
	 * @param group The group we want to insert the popup. Normally a Relative Layout so it can stand on top of everything
	 * @param originRect The rect we want the popup to appear from (relative to the Application Window!)
	 * @param arrowDirections The mask of bits to tell in which directions we want the popover to be shown
	 * @param animated Whether is animated, or not
	 */
		public void showPopoverFromRectInViewGroup (ViewGroup group, Rect originRect, int arrowDirections, bool animated)
		{

			//First, tell delegate we will show
			if (del != null)
				del.popoverViewWillShow (this);

			//Save superview
			Superview = group;

			//First, add the view to the view group. The popover will cover the whole area
			LayoutParams insertParams = new  LayoutParams (LayoutParams.MatchParent, LayoutParams.MatchParent);
			group.AddView (this, insertParams);

			//Now, save rect for the layout (is the same as the superview)
			popoverLayoutRect = PopoverView.getFrameForView (Superview);

			//Add available rects
			addAvailableRects (originRect, arrowDirections);
			//Get best rect
			int best = getBestRect ();

			//Add popover
//				Rect bestRect = possibleRects.get(best);
			Rect bestRect = null; 
			possibleRects.TryGetValue (best, out bestRect);
			addPopoverInRect (bestRect);
			//Add arrow image
			addArrow (originRect, best);


			//If we don't want animation, just tell the delegate
			if (!animated) {
				//Tell delegate we did show
				if (del != null)
					del.popoverViewDidShow (this);
			}
				//If we want animation, animate it!
				else {
				//Continue only if we are not animating
				if (!isAnimating) {

					//Create alpha animation, with its listener
					AlphaAnimation animation = new AlphaAnimation (0.0f, 1.0f);
					animation.Duration = fadeAnimationTime;
					animation.AnimationStart += (object sender, Animation.AnimationStartEventArgs e) => {
					};
					animation.AnimationRepeat += (object sender, Animation.AnimationRepeatEventArgs e) => {
					};
					animation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => {
						//End animation
						isAnimating = false;
						//Tell delegate we did show
						if (del != null)
							del.popoverViewDidShow (this);
					};
								
					//Start animation
					isAnimating = true;
					StartAnimation (animation);

				}
			}

		}

		/**
	 * Dismiss the current shown popover
	 * @param animated Whether it should be dismissed animated or not
	 */
		public void dissmissPopover (bool animated)
		{

			//Tell delegate we will dismiss
			if (del != null)
				del.popoverViewWillDismiss (this);

			//If we don't want animation
			if (!animated) {
				//Just remove views
				popoverView.RemoveAllViews ();
				RemoveAllViews ();
				Superview.RemoveView (this);
				//Tell delegate we did dismiss
				if (del != null)
					del.popoverViewDidDismiss (this);
			} else {
				//Continue only if there is not an animation in progress
				if (!isAnimating) {
					//Create alpha animation, with its listener
					AlphaAnimation animation = new AlphaAnimation (1.0f, 0.0f);
					animation.Duration = fadeAnimationTime;
					animation.AnimationStart += (object sender, Animation.AnimationStartEventArgs e) => {
					};
					animation.AnimationRepeat += (object sender, Animation.AnimationRepeatEventArgs e) => {

					};
					animation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => {
						//Remove the view
						popoverView.RemoveAllViews ();
						RemoveAllViews ();
						this.Superview.RemoveView (this);
						//End animation
						isAnimating = false;
						//Tell delegate we did dismiss
						if (del != null)
							del.popoverViewDidDismiss (this);
					};

					//Start animation
					isAnimating = true;
					StartAnimation (animation);
				}

			}

		}





		//********************************************************************
		// ON TOUCH LISTENER
		//********************************************************************
			
		public bool OnTouch (View v, MotionEvent e)
		{
			//If we touched over the background popover view (this)
			if ((!isAnimating) && (v == this)) {
				dissmissPopover (true);
			}
			return true;
		}


	}
}

