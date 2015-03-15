using System;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Android.Content;
using Android.Util;

namespace LibraryFlabbyListView
{
	public class FlabbyListView : ListView,Android.Widget.AbsListView.IOnScrollListener
	{
		private static float _pixelsScrollToCancelExpansion=100;
		private View _trackedChild;
		private FlabbyLayout _downView;
		private FlabbyLayout _downBelowView;
		private Rect _rect = new Rect();
		private int[] _listViewCoords;
		private int _childCount;
		private int _trackedChildPrevPosition;
		private int _trackedChildPrevTop;
		private float _oldDeltaY;
		private float _downXValue;
		private float _downYValue;


		public int TrackedChildPrevPosition
		{
			get{
				return this._trackedChildPrevTop;
			}
			set {
				this._trackedChildPrevTop = value;
				Invalidate ();
			}
		}

		public int TrackedChildPrevTop
		{
			get{
				return this._trackedChildPrevPosition;
			}
			set {
				this._trackedChildPrevPosition = value;
				Invalidate ();
			}
		}

		public float OldDeltaY
		{
			get{
				return this._oldDeltaY;
			}
			set {
				this._oldDeltaY = value;
				Invalidate ();
			}
		}

		public float DownXValue
		{
			get{
				return this._downXValue;
			}
			set {
				this._downXValue = value;
				Invalidate ();
			}
		}

		public float DownYValue
		{
			get{
				return this._downYValue;
			}
			set {
				this._downYValue = value;
				Invalidate ();
			}
		}

		public FlabbyListView(Context context) :base(context){
			init(context);
		}

		public FlabbyListView(Context context, IAttributeSet attrs) :base(context, attrs){
			init(context);
		}

		public FlabbyListView(Context context, IAttributeSet attrs, int defStyle) :base(context, attrs, defStyle){
			init(context);
		}
		private void init(Context context) {
			SetOnScrollListener(this);
		}

		public interface ListViewObserverDelegate {
			void onListScroll(View view, float deltaY);
		}


		private View GetChildInTheMiddle() {
			return GetChildAt(ChildCount / 2);
		}

		/**
     * Calculate the scroll distance comparing the distance with the top of the list of the current
     * child and the last one tracked
     *
     * @param l    - Current horizontal scroll origin.
     * @param t    - Current vertical scroll origin.
     * @param oldl - Previous horizontal scroll origin.
     * @param oldt - Previous vertical scroll origin.
     */
		protected override void OnScrollChanged (int l, int t, int oldl, int oldt)
		{
			base.OnScrollChanged (l, t, oldl, oldt);
			if (_trackedChild == null) {

				if (ChildCount > 0) {
					_trackedChild = GetChildInTheMiddle();
					TrackedChildPrevTop = _trackedChild.Top;
					TrackedChildPrevPosition = GetPositionForView(_trackedChild);
				}
			} else {

				bool ChildIsSafeToTrack = _trackedChild.Parent == this && GetPositionForView(_trackedChild) == TrackedChildPrevPosition;
				if (ChildIsSafeToTrack) {
					int top = _trackedChild.Top;
					float DeltaY = top - TrackedChildPrevTop;

					if (DeltaY == 0) {
						//When we scroll so fast the list this value becomes 0 all the time
						// so we don't want the other list stop, and we give it the last
						//no 0 value we have
						DeltaY = OldDeltaY;
					} else {
						OldDeltaY = DeltaY;
					}

					UpdateChildrenControlPoints(DeltaY);
					TrackedChildPrevTop = top;
				} else {
					_trackedChild = null;
				}
			}
		}
		public override bool OnTouchEvent (MotionEvent e)
		{
			switch (e.Action) {
			case MotionEventActions.Down:
				ActionDown(e);
				break;
			case MotionEventActions.Move:
				ActionMove(e);

				break;
			case MotionEventActions.Up:
				SendDownViewEvent(e);
				SendBelowDownViewEvent(e);
				break;
			}
			return base.OnTouchEvent (e);
		}
		private void ActionDown(MotionEvent e) {
			DownXValue = e.GetX();
			DownYValue = e.GetY();
			SetDownView(e);
			SendDownViewEvent(e);
			SendBelowDownViewEvent(e);
		}

		private void ActionMove(MotionEvent e) {
			float CurrentX = e.GetX();
			float CurrentY = e.GetY();
			float OffsetX = DownXValue - CurrentX;
			float OffsetY = DownYValue - CurrentY;
			if (Math.Abs(OffsetX) > Math.Abs(OffsetY)) {
				SendDownViewEvent(e);
				SendBelowDownViewEvent(e);
			} else if (Math.Abs(OffsetY) > _pixelsScrollToCancelExpansion) {
				e = MotionEvent.Obtain(Java.Lang.JavaSystem.CurrentTimeMillis(), Java.Lang.JavaSystem.CurrentTimeMillis(), MotionEventActions.Up, 0, 0, 0);
				SendDownViewEvent(e);
				SendBelowDownViewEvent(e);
			}
		}

		private void SendDownViewEvent(MotionEvent e) {
			if (_downView != null) {
				_downView.OnTouchEvent(e);
			}
		}

		private void SendBelowDownViewEvent(MotionEvent e) {
			if (_downBelowView != null) {
				_downBelowView.OnTouchEvent(e);
			}
		}

		private void SetDownView(MotionEvent e) {
			_childCount = ChildCount;
			_listViewCoords = new int[2];
			GetLocationOnScreen(_listViewCoords);
			int x = (int) e.RawX - _listViewCoords[0];
			int y = (int) e.RawY - _listViewCoords[1];
			FlabbyLayout Child;
			for (int i = 0; i < _childCount; i++) {
				Child = (FlabbyLayout) GetChildAt(i);
				Child.GetHitRect(_rect);
				if (_rect.Contains(x, y)) {
					_downView = Child;
					if(_downView!=null) {
						_downView.SetAsSelected(true);
					}
					_downBelowView = (FlabbyLayout) GetChildAt(i + 1);
				} else {
					Child.SetAsSelected(false);
				}
			}
		}
		private void UpdateChildrenControlPoints(float deltaY) {
			View Child;
			FlabbyLayout flabbyChild;
			for (int i = 0; i <= LastVisiblePosition - FirstVisiblePosition; i++) {
				Child = GetChildAt(i);
				if (Child is FlabbyLayout) {
					flabbyChild = (FlabbyLayout) Child;
					if (Child != null) {
						flabbyChild.UpdateControlPoints(deltaY);
					}
				}
			}
		}
		void AbsListView.IOnScrollListener.OnScroll (AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
		{		}

		void AbsListView.IOnScrollListener.OnScrollStateChanged (AbsListView view, ScrollState scrollState)
		{
			if (scrollState == ScrollState.Idle) {
				UpdateChildrenControlPoints(0);
			}
		}
	}
}

