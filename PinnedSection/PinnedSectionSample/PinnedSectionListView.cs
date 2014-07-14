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
using Android.Graphics.Drawables;
using Android.Util;
using Android.Database;

namespace PinnedSectionActivity
{
	public class PinnedSectionListView1 : ListView
	{

		//-- inner classes

		/** List adapter to be implemented for being used with PinnedSectionListView adapter. */
		public interface PinnedSectionListAdapter : IListAdapter {
			/** This method shall return 'true' if views of given type has to be pinned. */
			Boolean isItemViewTypePinned(int viewType);
		}

		/** Wrapper class for pinned section view and its position in the list. */
		public class PinnedSection {
			public View view;
			public int position;
			public long id;
		}

		//-- class fields

		// fields used for handling touch events
		private Rect mTouchRect = new Rect();
		private PointF mTouchPoint = new PointF();
		private int mTouchSlop;
		private View mTouchTarget;
		private MotionEvent mDownEvent;

		// fields used for drawing shadow under a pinned section
		private GradientDrawable mShadowDrawable;
		private int mSectionsDistanceY;
		private int mShadowHeight;

		/** Delegating listener, can be null. */
		AbsListView.IOnScrollListener mDelegateOnScrollListener;

		/** Shadow for being recycled, can be null. */
		PinnedSection mRecycleSection;

		/** shadow instance with a pinned view, can be null. */
		PinnedSection mPinnedSection;

		/** Pinned view Y-translation. We use it to stick pinned view to the next section. */
		int mTranslateY;

		public class OnScrollListenerImpl : Java.Lang.Object, Android.Widget.AbsListView.IOnScrollListener {

			PinnedSectionListView1 psl;



			public OnScrollListenerImpl(PinnedSectionListView1 psl)  {
				this.psl = psl;
			}

			public void OnScrollStateChanged(AbsListView view, int scrollState) {
				if (psl.mDelegateOnScrollListener != null) { // delegate
					psl.mDelegateOnScrollListener.OnScrollStateChanged(view, (ScrollState) scrollState);
				}
			}

			public void OnScrollStateChanged (AbsListView view, ScrollState scrollState) {
				if (psl.mDelegateOnScrollListener != null) { // delegate
					psl.mDelegateOnScrollListener.OnScrollStateChanged(view, scrollState);
				}
			}

			public void OnScroll (AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount) {

				Console.WriteLine ("on scrolll event");

				if (psl.mDelegateOnScrollListener != null) { // delegate
					psl.mDelegateOnScrollListener.OnScroll(view, firstVisibleItem, visibleItemCount, totalItemCount);
				}

				// get expected adapter or fail fast
				IListAdapter adapter = psl.Adapter;
				if (adapter == null || visibleItemCount == 0) return; // nothing to do

				Boolean isFirstVisibleItemSection =
					isItemViewTypePinned(adapter, adapter.GetItemViewType(firstVisibleItem));

				if (isFirstVisibleItemSection) {
					View sectionView = psl.GetChildAt(0);
					if (sectionView.Top == psl.PaddingTop) { // view sticks to the top, no need for pinned shadow
						psl.destroyPinnedShadow();
					} else { // section doesn't stick to the top, make sure we have a pinned shadow
						psl.ensureShadowForPosition(firstVisibleItem, firstVisibleItem, visibleItemCount);
					}

				} else { // section is not at the first visible position
					int sectionPosition = psl.findCurrentSectionPosition(firstVisibleItem);
					if (sectionPosition > -1) { // we have section position
						psl.ensureShadowForPosition(sectionPosition, firstVisibleItem, visibleItemCount);
					} else { // there is no section for the first visible item, destroy shadow
						psl.destroyPinnedShadow();
					}
				}
			}

		}	

		/** Default change observer. */

		private class DataSetObserverImpl : DataSetObserver {
			PinnedSectionListView1 psl;
			public DataSetObserverImpl(PinnedSectionListView1 psl) {
				this.psl = psl;
			} 
			public override void OnChanged ()
			{
				psl.recreatePinnedShadow();
			}

			public override void OnInvalidated ()
			{
				psl.recreatePinnedShadow();
			}
		}

		private DataSetObserver mDataSetObserver;
		private IOnScrollListener mOnScrollListener;

		//-- constructors



		public PinnedSectionListView1(Context context) : base(context) {
			initView();
		}

		public PinnedSectionListView1(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) {
			initView();
		}

		public PinnedSectionListView1(Context context, IAttributeSet attrs) : base(context, attrs) {
			initView();
		}

		public PinnedSectionListView1(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) {
			initView();
		}

		private void initView() {

			mDataSetObserver = new DataSetObserverImpl(this);
			mOnScrollListener = new OnScrollListenerImpl(this);

			SetOnScrollListener(mOnScrollListener);
			mTouchSlop = ViewConfiguration.Get (Context).ScaledTouchSlop;
			initShadow(true);
		}

		//-- public API methods

		public void setShadowVisible(Boolean visible) {
			initShadow(visible);
			if (mPinnedSection != null) {
				View v = mPinnedSection.view;
						Invalidate(v.Left, v.Top, v.Right, v.Bottom + mShadowHeight);
			}
		}

		//-- pinned section drawing methods

		public void initShadow(Boolean visible) {
			if (visible) {
				if (mShadowDrawable == null) {
					mShadowDrawable = new GradientDrawable (GradientDrawable.Orientation.TopBottom,
						new int[] { Color.ParseColor("#ffa0a0a0"), Color.ParseColor("#50a0a0a0"), Color.ParseColor("#00a0a0a0")});
					mShadowHeight = (int) (8 * Resources.DisplayMetrics.Density);
				}
			} else {
				if (mShadowDrawable != null) {
					mShadowDrawable = null;
					mShadowHeight = 0;
				}
			}
		}

		/** Create shadow wrapper with a pinned view for a view at given position */
		void createPinnedShadow(int position) {

			// try to recycle shadow
			PinnedSection pinnedShadow = mRecycleSection;
			mRecycleSection = null;

			// create new shadow, if needed
			if (pinnedShadow == null) pinnedShadow = new PinnedSection();
			// request new view using recycled view, if such
			View pinnedView = Adapter.GetView(position, pinnedShadow.view, this);

			// read layout parameters
			LayoutParams layoutParams = (LayoutParams) pinnedView.LayoutParameters;
			if (layoutParams == null) { // create default layout params
				layoutParams = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
			}

			MeasureSpecMode heightMode = MeasureSpec.GetMode(layoutParams.Height);
			int heightSize = MeasureSpec.GetSize(layoutParams.Height);

			if (heightMode == MeasureSpecMode.Unspecified) heightMode = MeasureSpecMode.Exactly;

			int maxHeight = Height - ListPaddingTop - ListPaddingBottom;
			if (heightSize > maxHeight) heightSize = maxHeight;

			// measure & layout
			int ws = MeasureSpec.MakeMeasureSpec(Width - ListPaddingLeft - ListPaddingRight, MeasureSpecMode.Exactly);
			int hs = MeasureSpec.MakeMeasureSpec(heightSize, heightMode);
			pinnedView.Measure(ws, hs);
			pinnedView.Layout(0, 0, pinnedView.MeasuredWidth, pinnedView.MeasuredHeight);
			mTranslateY = 0;

			// initialize pinned shadow
			pinnedShadow.view = pinnedView;
			pinnedShadow.position = position;
			pinnedShadow.id = Adapter.GetItemId(position);

			// store pinned shadow
			mPinnedSection = pinnedShadow;
		}

		/** Destroy shadow wrapper for currently pinned view */
		void destroyPinnedShadow() {
			if (mPinnedSection != null) {
				// keep shadow for being recycled later
				mRecycleSection = mPinnedSection;
				mPinnedSection = null;
			}
		}

		/** Makes sure we have an actual pinned shadow for given position. */
		void ensureShadowForPosition(int sectionPosition, int firstVisibleItem, int visibleItemCount) {
			if (visibleItemCount < 2) { // no need for creating shadow at all, we have a single visible item
				destroyPinnedShadow();
				return;
			}

			if (mPinnedSection != null
				&& mPinnedSection.position != sectionPosition) { // invalidate shadow, if required
				destroyPinnedShadow();
			}

			if (mPinnedSection == null) { // create shadow, if empty
				createPinnedShadow(sectionPosition);
			}

			// align shadow according to next section position, if needed
			int nextPosition = sectionPosition + 1;
			if (nextPosition < Count) {
				int nextSectionPosition = findFirstVisibleSectionPosition(nextPosition,
					visibleItemCount - (nextPosition - firstVisibleItem));
				if (nextSectionPosition > -1) {
					View nextSectionView = GetChildAt(nextSectionPosition - firstVisibleItem);
					int bottom = mPinnedSection.view.Bottom + PaddingTop;
					mSectionsDistanceY = nextSectionView.Top - bottom;
					if (mSectionsDistanceY < 0) {
						// next section overlaps pinned shadow, move it up
						mTranslateY = mSectionsDistanceY;
					} else {
						// next section does not overlap with pinned, stick to top
						mTranslateY = 0;
					}
				} else {
					// no other sections are visible, stick to top
					mTranslateY = 0;
					mSectionsDistanceY = Java.Lang.Integer.MaxValue;
				}
			}

		}


		int findFirstVisibleSectionPosition(int firstVisibleItem, int visibleItemCount) {
			Android.Widget.IListAdapter adapter = Adapter;
			for (int childIndex = 0; childIndex < visibleItemCount; childIndex++) {
				int position = firstVisibleItem + childIndex;
				int viewType = adapter.GetItemViewType(position);
				if (isItemViewTypePinned(adapter, viewType)) return position;
			}
			return -1;
		}


		int findCurrentSectionPosition(int fromPosition) {
			IListAdapter adapter = Adapter;

//			if (adapter.GetType() == ISectionIndexer) {
//				// try fast way by asking section indexer
//				SectionIndexerImpl indexer = (SectionIndexerImpl) adapter;
//				int sectionPosition = indexer.GetSectionForPosition(fromPosition);
//				int itemPosition = indexer.GetPositionForSection(sectionPosition);
//				int typeView = adapter.GetItemViewType(itemPosition);
//				if (isItemViewTypePinned(adapter, typeView)) {
//					return itemPosition;
//				} // else, no luck
//			}

			// try slow way by looking through to the next section item above
			for (int position=fromPosition; position>=0; position--) {
				int viewType = adapter.GetItemViewType(position);

				if (isItemViewTypePinned(adapter, viewType)) return position;
			}
			return -1; // no candidate found
		}

		void recreatePinnedShadow() {
			destroyPinnedShadow();
			IListAdapter adapter = Adapter;
			if (adapter != null && adapter.Count > 0) {
				int firstVisiblePosition = FirstVisiblePosition;
				int sectionPosition = findCurrentSectionPosition(firstVisiblePosition);
				if (sectionPosition == -1) return; // no views to pin, exit
				ensureShadowForPosition(sectionPosition,
					firstVisiblePosition, LastVisiblePosition - firstVisiblePosition);
			}
		}

		public override void SetOnScrollListener (IOnScrollListener l)
		{

			if (l == mOnScrollListener) {
				base.SetOnScrollListener (l);
			} else {
				mDelegateOnScrollListener = (OnScrollListenerImpl)l;
			}
		}

		public override void OnRestoreInstanceState (IParcelable state)
		{
			base.OnRestoreInstanceState (state);

			Post( new Action(() => {
				recreatePinnedShadow();
			}));
		}

		[Obsolete ("Please use the Adapter property setter")]
		public override void SetAdapter (IListAdapter adapter)
		{
			// assert adapter in debug mode
//			if (BuildConfig.DEBUG && adapter != null) {
//				if (!(adapter.GetType() ==  (new PinnedSectionListAdapter()).GetType()))
//					throw IllegalArgumentException("Does your adapter implement PinnedSectionListAdapter?");
//				if (adapter.ViewTypeCount < 2)
//					throw new IllegalArgumentException("Does your adapter handle at least two types" +
//						" of views in getViewTypeCount() method: items and sections?");
//			}

			// unregister observer at old adapter and register on new one
			IListAdapter oldAdapter = Adapter;
			if (oldAdapter != null) oldAdapter.UnregisterDataSetObserver(mDataSetObserver);
			if (adapter != null) adapter.RegisterDataSetObserver(mDataSetObserver);

			// destroy pinned shadow, if new adapter is not same as old one
			if (oldAdapter != adapter) destroyPinnedShadow();

			base.SetAdapter (adapter);

		}

		protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout (changed, left, top, right, bottom);
			if (mPinnedSection != null) {
				int parentWidth = right - left - PaddingLeft - PaddingRight;
				int shadowWidth = mPinnedSection.view.Width;
				if (parentWidth != shadowWidth) {
					recreatePinnedShadow();
				}
			}
		}

		protected override void DispatchDraw (Canvas canvas)
		{
			base.DispatchDraw (canvas);

			if (mPinnedSection != null) {

				// prepare variables
				int pLeft = ListPaddingLeft;
				int pTop = ListPaddingTop;
				View view = mPinnedSection.view;

				// draw child
				canvas.Save();

				int clipHeight = view.Height +
					(mShadowDrawable == null ? 0 : Math.Min(mShadowHeight, mSectionsDistanceY));
				canvas.ClipRect(pLeft, pTop, pLeft + view.Width, pTop + clipHeight);

				canvas.Translate(pLeft, pTop + mTranslateY);
				DrawChild(canvas, mPinnedSection.view, DrawingTime);

				if (mShadowDrawable != null && mSectionsDistanceY > 0) {
					mShadowDrawable.SetBounds(mPinnedSection.view.Left,
						mPinnedSection.view.Bottom,
						mPinnedSection.view.Right,
						mPinnedSection.view.Bottom + mShadowHeight);
					mShadowDrawable.Draw(canvas);
				}

				canvas.Restore();
			}
		}

		public override bool DispatchTouchEvent (MotionEvent ev)
		{
			float x = ev.GetX();
			float y = ev.GetY();
			MotionEventActions action = ev.Action;

			if (action == MotionEventActions.Down
				&& mTouchTarget == null
				&& mPinnedSection != null
				&& isPinnedViewTouched(mPinnedSection.view, x, y)) { // create touch target

				// user touched pinned view
				mTouchTarget = mPinnedSection.view;
				mTouchPoint.X = x;
				mTouchPoint.Y = y;

				// copy down event for eventually be used later
				mDownEvent = MotionEvent.Obtain(ev);
			}

			if (mTouchTarget != null) {
				if (isPinnedViewTouched(mTouchTarget, x, y)) { // forward event to pinned view
					mTouchTarget.DispatchTouchEvent(ev);
				}

				if (action == MotionEventActions.Up) { // perform onClick on pinned view
					base.DispatchTouchEvent(ev);
					performPinnedItemClick();
					clearTouchTarget();

				} else if (action == MotionEventActions.Cancel) { // cancel
					clearTouchTarget();

				} else if (action == MotionEventActions.Move) {
					if (Math.Abs(y - mTouchPoint.Y) > mTouchSlop) {

						// cancel sequence on touch target
						MotionEvent events = MotionEvent.Obtain(ev);
						events.Action = MotionEventActions.Cancel;
						mTouchTarget.DispatchTouchEvent(events);
						events.Recycle();

						// provide correct sequence to super class for further handling
						base.DispatchTouchEvent(mDownEvent);
						base.DispatchTouchEvent(ev);
						clearTouchTarget();
					}
				}

				return true;
			}

			return base.DispatchTouchEvent (ev);
		}

		private Boolean isPinnedViewTouched(View view, float x, float y) {
			view.GetHitRect(mTouchRect);

			// by taping top or bottom padding, the list performs on click on a border item.
			// we don't add top padding here to keep behavior consistent.
			mTouchRect.Top += mTranslateY;

			mTouchRect.Bottom += mTranslateY + PaddingTop;
			mTouchRect.Left += PaddingLeft;
			mTouchRect.Right -= PaddingRight;
			return mTouchRect.Contains((int)x, (int)y);
		}

		private void clearTouchTarget() {
			mTouchTarget = null;
			if (mDownEvent != null) {
				mDownEvent.Recycle ();
				mDownEvent = null;
			}
		}

		private Boolean performPinnedItemClick() {
			if (mPinnedSection == null) return false;

			IOnItemClickListener listener = OnItemClickListener;
			if (listener != null) {
				View view =  mPinnedSection.view;
				PlaySoundEffect(SoundEffects.Click);
				if (view != null) {
					view.SendAccessibilityEvent(Android.Views.Accessibility.EventTypes.ViewClicked);
				}
				listener.OnItemClick(this, view, mPinnedSection.position, mPinnedSection.id);
				return true;
			}
			return false;
		}

		public static Boolean isItemViewTypePinned(IListAdapter adapter, int viewType) {
			if (adapter.GetType() ==  (new HeaderViewListAdapter(null,null, null)).GetType()) {
				adapter = ((HeaderViewListAdapter)adapter).WrappedAdapter;
			}
			return ((PinnedSectionListAdapter) adapter).isItemViewTypePinned(viewType);
		}

	}

}

