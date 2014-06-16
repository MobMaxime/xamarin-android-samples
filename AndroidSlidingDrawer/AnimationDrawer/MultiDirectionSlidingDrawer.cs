
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

using Android.Util;
using Android.Content.Res;

namespace AnimationDrawer
{
	public class MultiDirectionSlidingDrawer : ViewGroup
	{
		public static  int				ORIENTATION_RTL		= 0;
		public static  int				ORIENTATION_BTT		= 1;
		public static  int				ORIENTATION_LTR		= 2;
		public static  int				ORIENTATION_TTB		= 3;

		private static  int			TAP_THRESHOLD					= 6;
		private static  float			MAXIMUM_TAP_VELOCITY			= 100.0f;
		private static  float			MAXIMUM_MINOR_VELOCITY		= 150.0f;
		private static  float			MAXIMUM_MAJOR_VELOCITY		= 200.0f;
		private static  float			MAXIMUM_ACCELERATION			= 2000.0f;
		private static  int			VELOCITY_UNITS					= 1000;
		private static  int			MSG_ANIMATE						= 1000;
		private static  int			ANIMATION_FRAME_DURATION	= 1000 / 60;

		private static  int			EXPANDED_FULL_OPEN			= -10001;
		private static  int			COLLAPSED_FULL_CLOSED		= -10002;

		private  int						mHandleId;
		private  int						mContentId;

		private View							mHandle;
		private View							mContent;

		private  Rect					mFrame							= new Rect();
		private  Rect					mInvalidate						= new Rect();
		private Boolean						mTracking;
		private Boolean						mLocked;

		private VelocityTracker				mVelocityTracker;

		private Boolean						mInvert;
		private Boolean						mVertical;
		private Boolean						mExpanded;
		private int								mBottomOffset;
		private int								mTopOffset;
		private int								mHandleHeight;
		private int								mHandleWidth;

		private AnimationDrawer.MultiDirectionSlidingDrawer.OnDrawerOpenListener		mOnDrawerOpenListener;
		private AnimationDrawer.MultiDirectionSlidingDrawer.OnDrawerCloseListener		mOnDrawerCloseListener;
		private AnimationDrawer.MultiDirectionSlidingDrawer.OnDrawerScrollListener	mOnDrawerScrollListener;

		private  Handler				mHandler;
		private float							mAnimatedAcceleration;
		private float							mAnimatedVelocity;
		private float							mAnimationPosition;
		private long							mAnimationLastTime;
		private long							mCurrentAnimationTime;
		private int								mTouchDelta;
		private Boolean						mAnimating;
		private Boolean						mAllowSingleTap;
		private Boolean						mAnimateOnClick;

		private  int						mTapThreshold;
		private  int						mMaximumTapVelocity;
		private int								mMaximumMinorVelocity;
		private int								mMaximumMajorVelocity;
		private int								mMaximumAcceleration;
		private  int						mVelocityUnits;


		public interface OnDrawerOpenListener {

			/**
		 * Invoked when the drawer becomes fully open.
		 */
			void onDrawerOpened();
		}

		/**
	 * Callback invoked when the drawer is closed.
	 */
		public interface OnDrawerCloseListener {

			/**
		 * Invoked when the drawer becomes fully closed.
		 */
			void onDrawerClosed();
		}

		/**
	 * Callback invoked when the drawer is scrolled.
	 */
		public interface OnDrawerScrollListener {

			/**
		 * Invoked when the user starts dragging/flinging the drawer's handle.
		 */
			void onScrollStarted();

			/**
		 * Invoked when the user stops dragging/flinging the drawer's handle.
		 */
			void onScrollEnded();
		}

		/**
	 * Creates a new SlidingDrawer from a specified set of attributes defined in
	 * XML.
	 * 
	 * @param context
	 *           The application's environment.
	 * @param attrs
	 *           The attributes defined in XML.
	 */
		public MultiDirectionSlidingDrawer( Context context, IAttributeSet attrs ) :base(context, attrs, 0 ) 
		{

			inititialize (context, attrs, 0);
		}

		/**
	 * Creates a new SlidingDrawer from a specified set of attributes defined in
	 * XML.
	 * 
	 * @param context
	 *           The application's environment.
	 * @param attrs
	 *           The attributes defined in XML.
	 * @param defStyle
	 *           The style to apply to this widget.
	 */
		public MultiDirectionSlidingDrawer( Context context, IAttributeSet attrs, int defStyle ): base( context, attrs, defStyle ) 
		{
			inititialize (context, attrs, defStyle);
		}
		  
		public void inititialize( Context context, IAttributeSet attrs, int defStyle )
		{
			mHandler = new SlidingHandler(this);
			TypedArray a = context.ObtainStyledAttributes( attrs, Resource.Styleable.MultiDirectionSlidingDrawer, defStyle, 0 );

			int orientation = a.GetInt( Resource.Styleable.MultiDirectionSlidingDrawer_direction, ORIENTATION_BTT );
			mVertical = ( orientation == ORIENTATION_BTT || orientation == ORIENTATION_TTB );
			mBottomOffset = (int)a.GetDimension( Resource.Styleable.MultiDirectionSlidingDrawer_bottomOffset, 0.0f );
			mTopOffset = (int)a.GetDimension(Resource.Styleable.MultiDirectionSlidingDrawer_topOffset, 0.0f );
			mAllowSingleTap = a.GetBoolean( Resource.Styleable.MultiDirectionSlidingDrawer_allowSingleTap, true );
			mAnimateOnClick = a.GetBoolean( Resource.Styleable.MultiDirectionSlidingDrawer_animateOnClick, true );
			mInvert = ( orientation == ORIENTATION_TTB || orientation == ORIENTATION_LTR );

			int handleId = a.GetResourceId( Resource.Styleable.MultiDirectionSlidingDrawer_handle, 0 );
			if ( handleId == 0 ) { 
				throw new Java.Lang.IllegalArgumentException( "The handle attribute is required and must refer "
					+ "to a valid child." ); 
			}

			int contentId = a.GetResourceId( Resource.Styleable.MultiDirectionSlidingDrawer_content, 0 );
			if ( contentId == 0 ) { throw new Java.Lang.IllegalArgumentException( "The content attribute is required and must refer "
				+ "to a valid child." ); }

			if ( handleId == contentId ) { throw new Java.Lang.IllegalArgumentException( "The content and handle attributes must refer "
				+ "to different children." ); }
			mHandleId = handleId;
			mContentId = contentId;

			float density = Resources.DisplayMetrics.Density;
			mTapThreshold = (int)( TAP_THRESHOLD * density + 0.5f );
			mMaximumTapVelocity = (int)( MAXIMUM_TAP_VELOCITY * density + 0.5f );
			mMaximumMinorVelocity = (int)( MAXIMUM_MINOR_VELOCITY * density + 0.5f );
			mMaximumMajorVelocity = (int)( MAXIMUM_MAJOR_VELOCITY * density + 0.5f );
			mMaximumAcceleration = (int)( MAXIMUM_ACCELERATION * density + 0.5f );
			mVelocityUnits = (int)( VELOCITY_UNITS * density + 0.5f );

			if( mInvert ) {
				mMaximumAcceleration = -mMaximumAcceleration;
				mMaximumMajorVelocity = -mMaximumMajorVelocity;
				mMaximumMinorVelocity = -mMaximumMinorVelocity;
			}

			a.Recycle();
			AlwaysDrawnWithCacheEnabled = false;
		}

		protected override void OnFinishInflate ()
		{
			mHandle = FindViewById( mHandleId );
			if ( mHandle == null ) { 
				throw new Java.Lang.IllegalArgumentException( "The handle attribute is must refer to an" + " existing child." ); 
			}

			mHandle.Click += (sender, e) => {
				if ( mLocked ) { return; }
	
					if ( mAnimateOnClick ) {
						animateToggle();
					} else {
						toggle();
					}
			};
			  

			mContent = FindViewById( mContentId );
			if ( mContent == null ) { throw new Java.Lang.IllegalArgumentException( "The content attribute is must refer to an"
				+ " existing child." ); }
			mContent.Visibility = ViewStates.Gone ;

		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			int widthSpecMode = (int)MeasureSpec.GetMode( widthMeasureSpec );
			int widthSpecSize = MeasureSpec.GetSize( widthMeasureSpec );

			int heightSpecMode = (int)MeasureSpec.GetMode( heightMeasureSpec );
			int heightSpecSize = MeasureSpec.GetSize( heightMeasureSpec );

			if ( widthSpecMode == (int)MeasureSpecMode.Unspecified || heightSpecMode == (int) MeasureSpecMode.Unspecified ) { throw new Java.Lang.RuntimeException(
				"SlidingDrawer cannot have UNSPECIFIED dimensions" ); }

			View handle = mHandle;
			MeasureChild( handle, widthMeasureSpec, heightMeasureSpec );

			if ( mVertical ) {
				int height = heightSpecSize - handle.MeasuredHeight - mTopOffset;
				mContent.Measure( MeasureSpec.MakeMeasureSpec( widthSpecSize, MeasureSpecMode.Exactly ), MeasureSpec.MakeMeasureSpec( height, MeasureSpecMode.Exactly ) );
			} else {
				int width = widthSpecSize - handle.MeasuredWidth - mTopOffset;
				mContent.Measure( MeasureSpec.MakeMeasureSpec( width, MeasureSpecMode.Exactly ), MeasureSpec.MakeMeasureSpec( heightSpecSize, MeasureSpecMode.Exactly ) );
			}
			SetMeasuredDimension( widthSpecSize, heightSpecSize );
		}


		protected override void DispatchDraw (Canvas canvas)
		{
			long drawingTime = DrawingTime;
			View handle = mHandle;
			System.Boolean isVertical = mVertical;

			DrawChild( canvas, handle, drawingTime );

			if ( mTracking || mAnimating ) {
				Bitmap cache = mContent.DrawingCache;
				if ( cache != null ) {
					if ( isVertical ) {
						if( mInvert ) {
							canvas.DrawBitmap( cache, 0, handle.Top - (Bottom - Top) + mHandleHeight, null );
						} else {
							canvas.DrawBitmap( cache, 0, handle.Bottom, null );
						}
					} else {
						canvas.DrawBitmap( cache, mInvert ? handle.Left - cache.Width : handle.Right, 0, null );
					}
				} else {
					canvas.Save ();
					if( mInvert ) {
						canvas.Translate( isVertical ? 0 : handle.Left - mTopOffset - mContent.MeasuredWidth, isVertical ? handle.Top - mTopOffset - mContent.MeasuredHeight : 0 );
					} else {
						canvas.Translate( isVertical ? 0 : handle.Left - mTopOffset, isVertical ? handle.Top - mTopOffset : 0 );
					}
					DrawChild( canvas, mContent, drawingTime );
					canvas.Restore();
				}
				Invalidate();
			} else if ( mExpanded ) {
				DrawChild( canvas, mContent, drawingTime );
			}
		}


		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			if ( mTracking ) { return; }

			int width = r - l;
			int height = b - t;

			View handle = mHandle;

			int handleWidth = handle.MeasuredWidth;
			int handleHeight = handle.MeasuredHeight;


			int handleLeft;
			int handleTop;

			View content = mContent;

			if ( mVertical ) {
				handleLeft = ( width - handleWidth ) / 2;
				if ( mInvert ) {
					handleTop = mExpanded ? height - mBottomOffset - handleHeight : mTopOffset;
					content.Layout( 0, mTopOffset, content.MeasuredWidth, mTopOffset + content.MeasuredHeight );
				} else {
					handleTop = mExpanded ? mTopOffset : height - handleHeight + mBottomOffset;
					content.Layout( 0, mTopOffset + handleHeight, content.MeasuredWidth, mTopOffset + handleHeight + content.MeasuredHeight );
				}
			} else {
				handleTop = ( height - handleHeight ) / 2;
				if( mInvert ) {
					handleLeft = mExpanded ? width - mBottomOffset - handleWidth : mTopOffset;
					content.Layout( mTopOffset, 0, mTopOffset + content.MeasuredWidth, content.MeasuredHeight );
				} else {
					handleLeft = mExpanded ? mTopOffset : width - handleWidth + mBottomOffset;
					content.Layout( mTopOffset + handleWidth, 0, mTopOffset + handleWidth + content.MeasuredWidth, content.MeasuredHeight );
				}
			}

			handle.Layout( handleLeft, handleTop, handleLeft + handleWidth, handleTop + handleHeight );
			mHandleHeight = handle.Height;
			mHandleWidth = handle.Width;
		}

		public override bool OnInterceptTouchEvent (MotionEvent ev)
		{
			if ( mLocked ) { return false; }

			int action = (int) ev.Action;

			float x = ev.GetX ();
			float y = ev.GetY();

			Rect frame = mFrame;
			View handle = mHandle;

			handle.GetHitRect( frame );
			if ( !mTracking && !frame.Contains( (int)x, (int)y ) ) { return false; }

			if ( action == (int) MotionEventActions.Down ) {
				mTracking = true;

				handle.Pressed = true;
				// Must be called before prepareTracking()
				prepareContent();

				// Must be called after prepareContent()
				if ( mOnDrawerScrollListener != null ) {
					mOnDrawerScrollListener.onScrollStarted();
				}

				if ( mVertical ) {
					int top = mHandle.Top;
					mTouchDelta = (int)y - top;
					prepareTracking( top );
				} else {
					int left = mHandle.Left;
					mTouchDelta = (int)x - left;
					prepareTracking( left );
				}
				mVelocityTracker.AddMovement( ev);
			}

			return true;
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			if ( mLocked ) { return true; }

			if ( mTracking ) {
				mVelocityTracker.AddMovement( e );
				MotionEventActions action = e.Action;
				switch ( action ) {
				case MotionEventActions.Move:
					moveHandle( (int)( mVertical ? e.GetY() : e.GetX() ) - mTouchDelta );
					break;
				case MotionEventActions.Up:
				case MotionEventActions.Cancel: {
						VelocityTracker velocityTracker = mVelocityTracker;
						velocityTracker.ComputeCurrentVelocity( mVelocityUnits );

						float yVelocity = velocityTracker.YVelocity;
						float xVelocity = velocityTracker.XVelocity;
						Boolean negative;

						Boolean vertical = mVertical;
						if ( vertical ) {
							negative = yVelocity < 0;
							if ( xVelocity < 0 ) {
								xVelocity = -xVelocity;
							}
							// fix by Maciej Ciemięga.
							if ( (!mInvert && xVelocity > mMaximumMinorVelocity) || (mInvert && xVelocity < mMaximumMinorVelocity) ) {
								xVelocity = mMaximumMinorVelocity;
							}
						} else {
							negative = xVelocity < 0;
							if ( yVelocity < 0 ) {
								yVelocity = -yVelocity;
							}
							// fix by Maciej Ciemięga.
							if ( (!mInvert && yVelocity > mMaximumMinorVelocity) || (mInvert && yVelocity < mMaximumMinorVelocity) ) {
								yVelocity = mMaximumMinorVelocity;
							}
						}

						float velocity = (float)Java.Lang.Math.Hypot( xVelocity, yVelocity );
						if ( negative ) {
							velocity = -velocity;
						}

						int handleTop = mHandle.Top;
						int handleLeft = mHandle.Left;
						int handleBottom = mHandle.Bottom;
						int handleRight = mHandle.Right;

						if ( Math.Abs( velocity ) < mMaximumTapVelocity ) {
							Boolean c1;
							Boolean c2;
							Boolean c3;
							Boolean c4;

							if( mInvert ) {
								c1 = ( mExpanded && (Bottom - handleBottom ) < mTapThreshold + mBottomOffset );
								c2 = ( !mExpanded && handleTop < mTopOffset + mHandleHeight - mTapThreshold );
								c3 = ( mExpanded && (Right - handleRight ) < mTapThreshold + mBottomOffset );
								c4 = ( !mExpanded && handleLeft > mTopOffset + mHandleWidth + mTapThreshold );
							} else {
								c1 = ( mExpanded && handleTop < mTapThreshold + mTopOffset );
								c2 = ( !mExpanded && handleTop > mBottomOffset + Bottom - Top - mHandleHeight - mTapThreshold );
								c3 = ( mExpanded && handleLeft < mTapThreshold + mTopOffset );
								c4 = ( !mExpanded && handleLeft > mBottomOffset + Right - Left - mHandleWidth - mTapThreshold );
							}

							if ( vertical ? c1 || c2 : c3 || c4 ) {

								if ( mAllowSingleTap ) {
									PlaySoundEffect( SoundEffects.Click );

									if ( mExpanded ) {
										animateClose( vertical ? handleTop : handleLeft );
									} else {
										animateOpen( vertical ? handleTop : handleLeft );
									}
								} else {
									performFling( vertical ? handleTop : handleLeft, velocity, false );
								}
							} else {
								performFling( vertical ? handleTop : handleLeft, velocity, false );
							}
						} else {
							performFling( vertical ? handleTop : handleLeft, velocity, false );
						}
					}
					break;
				}
			}

			return mTracking || mAnimating || base.OnTouchEvent( e );
		}

		private void animateClose( int position )
		{
			prepareTracking( position );
			performFling( position, mMaximumAcceleration, true );
		}

		private void animateOpen( int position )
		{
			prepareTracking( position );
			performFling( position, -mMaximumAcceleration, true );
		}

		private void performFling( int position, float velocity, Boolean always )
		{
			mAnimationPosition = position;
			mAnimatedVelocity = velocity;

			Boolean c1;
			Boolean c2;
			Boolean c3;

			if ( mExpanded ) 
			{
				int bottom = mVertical ? Bottom : Right;
				int handleHeight = mVertical ? mHandleHeight : mHandleWidth;

				c1 = mInvert ? velocity < mMaximumMajorVelocity : velocity > mMaximumMajorVelocity;
				c2 = mInvert ? ( bottom - (position + handleHeight) ) + mBottomOffset > handleHeight : position > mTopOffset + ( mVertical ? mHandleHeight : mHandleWidth );
				c3 = mInvert ? velocity < -mMaximumMajorVelocity : velocity > -mMaximumMajorVelocity;
				if ( always || ( c1 || ( c2 && c3 ) ) ) {
					// We are expanded, So animate to CLOSE!
					mAnimatedAcceleration = mMaximumAcceleration;
					if( mInvert )
					{
						if ( velocity > 0 ) {
							mAnimatedVelocity = 0;
						}
					} else {
						if ( velocity < 0 ) {
							mAnimatedVelocity = 0;
						}
					}
				} else {
					// We are expanded, but they didn't move sufficiently to cause
					// us to retract. Animate back to the expanded position. so animate BACK to expanded!
					mAnimatedAcceleration = -mMaximumAcceleration;

					if( mInvert ) {
						if ( velocity < 0 ) {
							mAnimatedVelocity = 0;
						}
					} else {
						if ( velocity > 0 ) {
							mAnimatedVelocity = 0;
						}
					}
				}
			} else {

				// WE'RE COLLAPSED

				c1 = mInvert ? velocity < mMaximumMajorVelocity : velocity > mMaximumMajorVelocity;
				c2 = mInvert ? ( position < ( mVertical ? Height : Width ) / 2 ) : ( position > ( mVertical ? Height: Width) / 2 );
				c3 = mInvert ? velocity < -mMaximumMajorVelocity : velocity > -mMaximumMajorVelocity;

				if ( !always && ( c1 || ( c2 && c3 ) ) ) {
					mAnimatedAcceleration = mMaximumAcceleration;

					if( mInvert ) {
						if ( velocity > 0 ) {
							mAnimatedVelocity = 0;
						}
					} else {
						if ( velocity < 0 ) {
							mAnimatedVelocity = 0;
						}
					}
				} else {
					mAnimatedAcceleration = -mMaximumAcceleration;

					if( mInvert ) {
						if ( velocity < 0 ) {
							mAnimatedVelocity = 0;
						}
					} else {
						if ( velocity > 0 ) {
							mAnimatedVelocity = 0;
						}
					}
				}
			}

			long now = SystemClock.UptimeMillis();
			mAnimationLastTime = now;
			mCurrentAnimationTime = now + ANIMATION_FRAME_DURATION;
			mAnimating = true;
			mHandler.RemoveMessages( MSG_ANIMATE );
			mHandler.SendMessageAtTime( mHandler.ObtainMessage( MSG_ANIMATE ), mCurrentAnimationTime );
			stopTracking();
		}

		private void prepareTracking( int position )
		{
			mTracking = true;
			mVelocityTracker = VelocityTracker.Obtain();
			Boolean opening = !mExpanded;

			if ( opening ) {
				mAnimatedAcceleration = mMaximumAcceleration;
				mAnimatedVelocity = mMaximumMajorVelocity;
				if( mInvert )
					mAnimationPosition = mTopOffset;
				else
					mAnimationPosition = mBottomOffset + ( mVertical ? Height - mHandleHeight : Width - mHandleWidth );
				moveHandle( (int)mAnimationPosition );
				mAnimating = true;
				mHandler.RemoveMessages( MSG_ANIMATE );
				long now = SystemClock.UptimeMillis();
				mAnimationLastTime = now;
				mCurrentAnimationTime = now + ANIMATION_FRAME_DURATION;
				mAnimating = true;
			} else {
				if ( mAnimating ) {
					mAnimating = false;
					mHandler.RemoveMessages( MSG_ANIMATE );
				}
				moveHandle( position );
			}
		}

		private void moveHandle( int position )
		{
			View handle = mHandle;

			if ( mVertical ) {
				if ( position == EXPANDED_FULL_OPEN ) {
					if( mInvert )
						handle.OffsetTopAndBottom( mBottomOffset + Bottom - Top - mHandleHeight );
					else
						handle.OffsetTopAndBottom( mTopOffset - handle.Top );
					Invalidate();
				} else if ( position == COLLAPSED_FULL_CLOSED ) {
					if( mInvert ) {
						handle.OffsetTopAndBottom( mTopOffset - handle.Top);
					} else {
						handle.OffsetTopAndBottom( mBottomOffset + Bottom - Top - mHandleHeight - handle.Top);
					}
					Invalidate();
				} else 
				{
					int top = handle.Top;
					int deltaY = position - top;
					if ( position < mTopOffset ) {
						deltaY = mTopOffset - top;
					} else if ( deltaY > mBottomOffset + Bottom - Top - mHandleHeight - top ) {
						deltaY = mBottomOffset + Bottom - Top - mHandleHeight - top;
					}

					handle.OffsetTopAndBottom( deltaY );

					Rect frame = mFrame;
					Rect region = mInvalidate;

					handle.GetHitRect( frame );
					region.Set( frame );

					region.Union( frame.Left, frame.Top - deltaY, frame.Right, frame.Bottom - deltaY );
					region.Union( 0, frame.Bottom - deltaY, Width, frame.Bottom - deltaY + mContent.Height );

					Invalidate( region );
				}
			} else {
				if ( position == EXPANDED_FULL_OPEN ) {
					if( mInvert )
						handle.OffsetLeftAndRight( mBottomOffset + Right - Left - mHandleWidth );
					else
						handle.OffsetLeftAndRight( mTopOffset - handle.Left );
					Invalidate();
				} else if ( position == COLLAPSED_FULL_CLOSED ) {
					if( mInvert )
						handle.OffsetLeftAndRight( mTopOffset - handle.Left );
					else
						handle.OffsetLeftAndRight( mBottomOffset + Right - Left - mHandleWidth - handle.Left );
					Invalidate();
				} else {
					int left = handle.Left;
					int deltaX = position - left;
					if ( position < mTopOffset ) {
						deltaX = mTopOffset - left;
					} else if ( deltaX > mBottomOffset + Right - Left - mHandleWidth - left ) {
						deltaX = mBottomOffset + Right - Left - mHandleWidth - left;
					}
					handle.OffsetLeftAndRight( deltaX );

					Rect frame = mFrame;
					Rect region = mInvalidate;

					handle.GetHitRect( frame );
					region.Set( frame );

					region.Union( frame.Left - deltaX, frame.Top, frame.Right - deltaX, frame.Bottom );
					region.Union( frame.Right - deltaX, 0, frame.Right - deltaX + mContent.Width, Height );

					Invalidate( region );
				}
			}
		}

		private void prepareContent()
		{
			if ( mAnimating ) { return; }

			// Something changed in the content, we need to honor the layout request
			// before creating the cached bitmap
			View content = mContent;
			if ( content.IsLayoutRequested) {

				if ( mVertical ) {
					int handleHeight = mHandleHeight;
					int height = Bottom - Top - handleHeight - mTopOffset;
					content.Measure( MeasureSpec.MakeMeasureSpec( Right - Left, MeasureSpecMode.Exactly ), MeasureSpec.MakeMeasureSpec( height, MeasureSpecMode.Exactly ) );

					if ( mInvert ) 
						content.Layout( 0, mTopOffset, content.MeasuredWidth, mTopOffset + content.MeasuredHeight );
					else 
						content.Layout( 0, mTopOffset + handleHeight, content.MeasuredWidth, mTopOffset + handleHeight + content.MeasuredHeight );

				} else {

					int handleWidth = mHandle.Width;
					int width = Right - Left - handleWidth - mTopOffset;
					content.Measure( MeasureSpec.MakeMeasureSpec( width, MeasureSpecMode.Exactly ), MeasureSpec.MakeMeasureSpec( Bottom - Top, MeasureSpecMode.Exactly ) );

					if( mInvert )
						content.Layout( mTopOffset, 0, mTopOffset + content.MeasuredWidth, content.MeasuredHeight );
					else
						content.Layout( handleWidth + mTopOffset, 0, mTopOffset + handleWidth + content.MeasuredWidth, content.MeasuredHeight );
				}
			}
			// Try only once... we should really loop but it's not a big deal
			// if the draw was cancelled, it will only be temporary anyway
			content.ViewTreeObserver.DispatchOnPreDraw();
			content.BuildDrawingCache();

			content.Visibility = ViewStates.Gone;
		}

		private void stopTracking()
		{
			mHandle.Pressed = false;
			mTracking = false;

			if ( mOnDrawerScrollListener != null ) {
				mOnDrawerScrollListener.onScrollEnded();
			}

			if ( mVelocityTracker != null ) {
				mVelocityTracker.Recycle();
				mVelocityTracker = null;
			}
		}

		public void doAnimation()
		{
			if ( mAnimating ) {
				incrementAnimation();

				if( mInvert )
				{
					if ( mAnimationPosition < mTopOffset ) {
						mAnimating = false;
						closeDrawer();
					} else if ( mAnimationPosition >= mTopOffset + ( mVertical ? Height : Width ) - 1 ) {
						mAnimating = false;
						openDrawer();
					} else {
						moveHandle( (int)mAnimationPosition );
						mCurrentAnimationTime += ANIMATION_FRAME_DURATION;
						mHandler.SendMessageAtTime( mHandler.ObtainMessage( MSG_ANIMATE ), mCurrentAnimationTime );
					}				
				} else {
					if ( mAnimationPosition >= mBottomOffset + ( mVertical ? Height : Width ) - 1 ) {
						mAnimating = false;
						closeDrawer();
					} else if ( mAnimationPosition < mTopOffset ) {
						mAnimating = false;
						openDrawer();
					} else {
						moveHandle( (int)mAnimationPosition );
						mCurrentAnimationTime += ANIMATION_FRAME_DURATION;
						mHandler.SendMessageAtTime( mHandler.ObtainMessage( MSG_ANIMATE ), mCurrentAnimationTime );
					}
				}
			}
		}

		private void incrementAnimation()
		{
			long now = SystemClock.UptimeMillis();
			float t = ( now - mAnimationLastTime ) / 1000.0f; // ms -> s
			float position = mAnimationPosition;
			float v = mAnimatedVelocity; // px/s
			float a = mInvert ? mAnimatedAcceleration : mAnimatedAcceleration; // px/s/s
			mAnimationPosition = position + ( v * t ) + ( 0.5f * a * t * t ); // px
			mAnimatedVelocity = v + ( a * t ); // px/s
			mAnimationLastTime = now; // ms
		}

		/**
	 * Toggles the drawer open and close. Takes effect immediately.
	 * 
	 * @see #open()
	 * @see #close()
	 * @see #animateClose()
	 * @see #animateOpen()
	 * @see #animateToggle()
	 */
		public void toggle()
		{
			if ( !mExpanded ) {
				openDrawer();
			} else {
				closeDrawer();
			}
			Invalidate();
			RequestLayout();
		}

		/**
	 * Toggles the drawer open and close with an animation.
	 * 
	 * @see #open()
	 * @see #close()
	 * @see #animateClose()
	 * @see #animateOpen()
	 * @see #toggle()
	 */
		public void animateToggle()
		{
			if ( !mExpanded ) {
				animateOpen();
			} else {
				animateClose();
			}
		}

		/**
	 * Opens the drawer immediately.
	 * 
	 * @see #toggle()
	 * @see #close()
	 * @see #animateOpen()
	 */
		public void open()
		{
			openDrawer();
			Invalidate();
			RequestLayout();

			SendAccessibilityEvent(Android.Views.Accessibility.EventTypes.WindowStateChanged );
		}

		/**
	 * Closes the drawer immediately.
	 * 
	 * @see #toggle()
	 * @see #open()
	 * @see #animateClose()
	 */
		public void close()
		{
			closeDrawer();
			Invalidate();
			RequestLayout();
		}

		/**
	 * Closes the drawer with an animation.
	 * 
	 * @see #close()
	 * @see #open()
	 * @see #animateOpen()
	 * @see #animateToggle()
	 * @see #toggle()
	 */
		public void animateClose()
		{
			prepareContent();
			OnDrawerScrollListener scrollListener = mOnDrawerScrollListener;
			if ( scrollListener != null ) {
				scrollListener.onScrollStarted();
			}
			animateClose( mVertical ? mHandle.Top : mHandle.Left );

			if ( scrollListener != null ) {
				scrollListener.onScrollEnded();
			}
		}

		/**
	 * Opens the drawer with an animation.
	 * 
	 * @see #close()
	 * @see #open()
	 * @see #animateClose()
	 * @see #animateToggle()
	 * @see #toggle()
	 */
		public void animateOpen()
		{
			prepareContent();
			OnDrawerScrollListener scrollListener = mOnDrawerScrollListener;
			if ( scrollListener != null ) {
				scrollListener.onScrollStarted();
			}
			animateOpen( mVertical ? mHandle.Top : mHandle.Left );

			SendAccessibilityEvent(Android.Views.Accessibility.EventTypes.WindowStateChanged );

			if ( scrollListener != null ) {
				scrollListener.onScrollEnded();
			}
		}

		private void closeDrawer()
		{
			moveHandle( COLLAPSED_FULL_CLOSED );
			mContent.Visibility = ViewStates.Gone;
			mContent.DestroyDrawingCache();

			if ( !mExpanded ) { return; }

			mExpanded = false;
			if ( mOnDrawerCloseListener != null ) {
				mOnDrawerCloseListener.onDrawerClosed();
			}
		}

		private void openDrawer()
		{
			moveHandle( EXPANDED_FULL_OPEN );
			mContent.Visibility = ViewStates.Visible;

			if ( mExpanded ) { return; }

			mExpanded = true;

			if ( mOnDrawerOpenListener != null ) {
				mOnDrawerOpenListener.onDrawerOpened();
			}
		}

		/**
	 * Sets the listener that receives a notification when the drawer becomes
	 * open.
	 * 
	 * @param onDrawerOpenListener
	 *           The listener to be notified when the drawer is opened.
	 */
		public void setOnDrawerOpenListener( OnDrawerOpenListener onDrawerOpenListener )
		{
			mOnDrawerOpenListener = onDrawerOpenListener;
		}

		/**
	 * Sets the listener that receives a notification when the drawer becomes
	 * close.
	 * 
	 * @param onDrawerCloseListener
	 *           The listener to be notified when the drawer is closed.
	 */
		public void setOnDrawerCloseListener( OnDrawerCloseListener onDrawerCloseListener )
		{
			mOnDrawerCloseListener = onDrawerCloseListener;
		}

		/**
	 * Sets the listener that receives a notification when the drawer starts or
	 * ends a scroll. A fling is considered as a scroll. A fling will also
	 * trigger a drawer opened or drawer closed event.
	 * 
	 * @param onDrawerScrollListener
	 *           The listener to be notified when scrolling starts or stops.
	 */
		public void setOnDrawerScrollListener( OnDrawerScrollListener onDrawerScrollListener )
		{
			mOnDrawerScrollListener = onDrawerScrollListener;
		}

		/**
	 * Returns the handle of the drawer.
	 * 
	 * @return The View reprenseting the handle of the drawer, identified by the
	 *         "handle" id in XML.
	 */
		public View getHandle()
		{
			return mHandle;
		}

		/**
	 * Returns the content of the drawer.
	 * 
	 * @return The View reprenseting the content of the drawer, identified by the
	 *         "content" id in XML.
	 */
		public View getContent()
		{
			return mContent;
		}

		/**
	 * Unlocks the SlidingDrawer so that touch events are processed.
	 * 
	 * @see #lock()
	 */
		public void unlock()
		{
			mLocked = false;
		}

		/**
	 * Locks the SlidingDrawer so that touch events are ignores.
	 * 
	 * @see #unlock()
	 */
		public void Lock()
		{
			mLocked = true;
		}

		/**
	 * Indicates whether the drawer is currently fully opened.
	 * 
	 * @return True if the drawer is opened, false otherwise.
	 */
		public Boolean isOpened()
		{
			return mExpanded;
		}

		/**
	 * Indicates whether the drawer is scrolling or flinging.
	 * 
	 * @return True if the drawer is scroller or flinging, false otherwise.
	 */
		public Boolean isMoving()
		{
			return mTracking || mAnimating;
		}

		private class SlidingHandler : Handler {
			MultiDirectionSlidingDrawer MDS;
			public SlidingHandler(MultiDirectionSlidingDrawer mds) {
				MDS = mds;
			}
			public void handleMessage( Message m )
			{
				switch ( m.What ) {
				case 1000:
					MDS.doAnimation();
					break;
				}
			}
		}


		 
//		public  class DrawerToggler : IOnClickListener {
//
//			MultiDirectionSlidingDrawer MDS;
//			public DrawerToggler(MultiDirectionSlidingDrawer mds) {
//				MDS = mds;
//			}
//
//			public void OnClick( View v )
//			{
//				if ( MDS.mLocked ) { return; }
//
//				if ( MDS.mAnimateOnClick ) {
//					MDS.animateToggle();
//				} else {
//					MDS.toggle();
//				}
//			}
//
//			public void Dispose() {
//
//			}
//		}

	}
}

