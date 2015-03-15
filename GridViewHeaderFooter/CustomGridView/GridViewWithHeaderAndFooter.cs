using System;
using Android.Widget;
using Android.Views;
using System.Collections.Generic;
using Android.Content;
using Android.Util;
using Android.Database;
using System.Linq;
using Android.OS;
using Java.Lang.Reflect;

namespace CustomGridView
{
	public class GridViewWithHeaderAndFooter : GridView
	{
		public static bool _debug = false;

		/**
     * A class that represents a fixed view in a list, for example a header at the top
     * or a footer at the bottom.
     */
		 class FixedViewInfo {
			/**
         * The view to add to the grid
         */
			public View view;
			public ViewGroup viewContainer;
			/**
         * The data backing the view. This is returned from {@link ListAdapter#getItem(int)}.
         */
			public Object data;
			/**
         * <code>true</code> if the fixed view should be selectable in the grid
         */
			public bool isSelectable;
		}
		int _numColumns = 2;
		View _viewForMeasureRowHeight = null;
		int _rowHeight = -1;
		static string _logTag = "grid-view-with-header-and-footer";

		List<FixedViewInfo> _headerViewInfos = new List<FixedViewInfo>();
		List<FixedViewInfo> _footerViewInfos = new List<FixedViewInfo>();

		void initHeaderGridView() {
		}
		public GridViewWithHeaderAndFooter(IntPtr javaReference, Android.Runtime.JniHandleOwnership jniHandleOwnership) 
			: base(javaReference, jniHandleOwnership) { }

		public GridViewWithHeaderAndFooter(Context context) :base(context){
			initHeaderGridView();
		}

		public GridViewWithHeaderAndFooter(Context context, IAttributeSet attrs):base(context, attrs) {
			initHeaderGridView();
		}

		public GridViewWithHeaderAndFooter(Context context, IAttributeSet attrs, int defStyle) :base(context, attrs, defStyle){
			initHeaderGridView();
		}
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			IListAdapter adapter = Adapter;
			if (adapter != null && adapter is HeaderViewGridAdapter) {
				((HeaderViewGridAdapter) adapter).setNumColumns(GetNumColumnsCompatible());
				((HeaderViewGridAdapter) adapter).setRowHeight(GetRowHeight());
			}
		}
		public override void SetClipChildren (bool clipChildren)
		{
			// Ignore, since the header rows depend on not being clipped
		}
		/**
     * Do not call this method unless you know how it works.
     *
     * @param clipChildren
     */
		public void SetClipChildrenSupper(bool clipChildren) {
			base.SetClipChildren(false);
		}
		/**
     * Add a fixed view to appear at the top of the grid. If addHeaderView is
     * called more than once, the views will appear in the order they were
     * added. Views added using this call can take focus if they want.
     * <p/>
     * NOTE: Call this before calling setAdapter. This is so HeaderGridView can wrap
     * the supplied cursor with one that will also account for header views.
     *
     * @param v The view to add.
     */
		public void AddHeaderView(View v) {
			AddHeaderView(v, null, true);
		}

		/**
     * Add a fixed view to appear at the top of the grid. If addHeaderView is
     * called more than once, the views will appear in the order they were
     * added. Views added using this call can take focus if they want.
     * <p/>
     * NOTE: Call this before calling setAdapter. This is so HeaderGridView can wrap
     * the supplied cursor with one that will also account for header views.
     *
     * @param v            The view to add.
     * @param data         Data to associate with this view
     * @param isSelectable whether the item is selectable
     */
		public void AddHeaderView(View v, Object data, bool isSelectable) {
			IListAdapter adapter = Adapter;
			if (adapter != null && !(adapter is HeaderViewGridAdapter)) {
				throw new Java.Lang.IllegalStateException(
					"Cannot add header view to grid -- setAdapter has already been called.");
			}
			ViewGroup.LayoutParams lyp = v.LayoutParameters;

			FixedViewInfo info = new FixedViewInfo();
			FrameLayout fl = new FullWidthFixedViewLayout(Context,this);
			if (lyp == null) {
				lyp = new ViewGroup.LayoutParams (WindowManagerLayoutParams.MatchParent,WindowManagerLayoutParams.MatchParent);
			}

			if (lyp != null) {
				v.LayoutParameters=new FrameLayout.LayoutParams(lyp.Width, lyp.Height);
				fl.LayoutParameters=new AbsListView.LayoutParams(lyp.Width, lyp.Height);
			}
			fl.AddView(v);
			info.view = v;
			info.viewContainer = fl;
			info.data = data;
			info.isSelectable = isSelectable;
			_headerViewInfos.Add(info);
			// in the case of re-adding a header view, or adding one later on,
			// we need to notify the observer
			if (adapter != null) {
				((HeaderViewGridAdapter) adapter).notifyDataSetChanged();
			}
		}
		public void AddFooterView(View v) {
			AddFooterView(v, null, true);
		}

		public void AddFooterView(View v, Object data, bool isSelectable) {
			IListAdapter mAdapter = Adapter;
			if (mAdapter != null && !(mAdapter is HeaderViewGridAdapter)) {
				throw new Java.Lang.IllegalStateException(
					"Cannot add header view to grid -- setAdapter has already been called.");
			}

			ViewGroup.LayoutParams lyp = v.LayoutParameters;

			FixedViewInfo info = new FixedViewInfo();
			FrameLayout fl = new FullWidthFixedViewLayout(Context,this);

			if (lyp != null) {
				v.LayoutParameters=new FrameLayout.LayoutParams(lyp.Width, lyp.Height);
				fl.LayoutParameters=new AbsListView.LayoutParams(lyp.Width, lyp.Height);
			}
			fl.AddView(v);
			info.view = v;
			info.viewContainer = fl;
			info.data = data;
			info.isSelectable = isSelectable;
			_footerViewInfos.Add(info);

			if (mAdapter != null) {
				((HeaderViewGridAdapter) mAdapter).notifyDataSetChanged();
			}
		}

		public int GetHeaderViewCount() {
			return _headerViewInfos.Count;
		}

		public int GetFooterViewCount() {
			return _footerViewInfos.Count;
		}

		/**
     * Removes a previously-added header view.
     *
     * @param v The view to remove
     * @return true if the view was removed, false if the view was not a header
     * view
     */
		public bool RemoveHeaderView(View v) {
			if (_headerViewInfos.Count > 0) {
				bool result = false;
				IListAdapter adapter = Adapter;
				if (adapter != null && ((HeaderViewGridAdapter) adapter).removeHeader(v)) {
					result = true;
				}
				RemoveFixedViewInfo(v, _headerViewInfos);
				return result;
			}
			return false;
		}

		/**
     * Removes a previously-added footer view.
     *
     * @param v The view to remove
     * @return true if the view was removed, false if the view was not a header
     * view
     */
		public bool RemoveFooterView(View v) {
			if (_footerViewInfos.Count > 0) {
				bool result = false;
				IListAdapter adapter = Adapter;
				if (adapter != null && ((HeaderViewGridAdapter) adapter).removeFooter(v)) {
					result = true;
				}
				RemoveFixedViewInfo(v, _footerViewInfos);
				return result;
			}
			return false;
		}

		private void RemoveFixedViewInfo(View v, List<FixedViewInfo> where) {
			int len = where.Count;
			for (int i = 0; i < len; ++i) {
				FixedViewInfo info = where[i];
				if (info.view == v) {
						where.RemoveAt(i);
					break;
				}
			}
		}

		private int GetNumColumnsCompatible() {
			if ((int)Build.VERSION.SdkInt >= 11) {
				return base.NumColumns;
			} else {
				try {
					Field numColumns = Class.Superclass.GetDeclaredField("_numColumns");
					numColumns.Accessible=true;
					return numColumns.GetInt(this);
				} catch (Exception e) {
					if (_numColumns != -1) {
						return _numColumns;
					}
					throw new Java.Lang.RuntimeException("Can not determine the _numColumns for this API platform, please call setNumColumns to set it.");
				}
			}
		}

		int GetColumnWidthCompatible() {
			if ((int)Build.VERSION.SdkInt >= 16) {
				#if __ANDROID16__
				return base.getColumnWidth();
				#endif
			} else {
				try {
					Field numColumns = Class.Superclass.GetDeclaredField("mColumnWidth");
					numColumns.Accessible=true;
					return numColumns.GetInt(this);
				} catch (Java.Lang.NoSuchFieldException e) {
					throw new Java.Lang.RuntimeException(e);
				} catch (Java.Lang.IllegalAccessException e) {
					throw new Java.Lang.RuntimeException(e);
				}
			}
			return 1;
		}
		protected override void OnDetachedFromWindow ()
		{
			base.OnDetachedFromWindow ();
			_viewForMeasureRowHeight = null;
		}
		public void InvalidateRowHeight() {
			_rowHeight = -1;
		}

		public int GetRowHeight() {
			if (_rowHeight > 0) {
				return _rowHeight;
			}
			IListAdapter adapter = Adapter;
			int numColumns = GetNumColumnsCompatible();

			// adapter has not been set or has no views in it;
			if (adapter == null || adapter.Count <= numColumns * (_headerViewInfos.Count + _footerViewInfos.Count)) {
				return -1;
			}
			int mColumnWidth = GetColumnWidthCompatible();
			View view = Adapter.GetView(numColumns * _headerViewInfos.Count, _viewForMeasureRowHeight, this);
			AbsListView.LayoutParams p = (AbsListView.LayoutParams) view.LayoutParameters;
			if (p == null) {
				p = new AbsListView.LayoutParams(-1, -2, 0);
				view.LayoutParameters=p;
			}
			int childHeightSpec = GetChildMeasureSpec(
				MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), 0, p.Height);
			int childWidthSpec = GetChildMeasureSpec(
				MeasureSpec.MakeMeasureSpec(mColumnWidth, MeasureSpecMode.Exactly), 0, p.Width);
			view.Measure(childWidthSpec, childHeightSpec);
			_viewForMeasureRowHeight = view;
			_rowHeight = view.MeasuredHeight;
			return _rowHeight;
		}

		public void TryToScrollToBottomSmoothly() {
			int lastPos = Adapter.Count - 1;
			if ((int)Build.VERSION.SdkInt >= 11) {
				SmoothScrollToPositionFromTop(lastPos, 0);
			} else {
				SetSelection(lastPos);
			}
		}

		public void TryToScrollToBottomSmoothly(int duration) {
			int lastPos = Adapter.Count - 1;
			if ((int)Build.VERSION.SdkInt >= 11) {
				SmoothScrollToPositionFromTop(lastPos, 0, duration);
			} else {
				SetSelection(lastPos);
			}
		}
			
		public override IListAdapter Adapter {
			get {
				return base.Adapter;
			}
			set {
				if (_headerViewInfos.Count > 0 || _footerViewInfos.Count > 0) {
					HeaderViewGridAdapter headerViewGridAdapter = new HeaderViewGridAdapter(_headerViewInfos, _footerViewInfos, value,this);
					int numColumns = GetNumColumnsCompatible();
					if (numColumns > 1) {
						headerViewGridAdapter.setNumColumns(numColumns);
					}
					headerViewGridAdapter.setRowHeight(GetRowHeight());
					base.SetAdapter(headerViewGridAdapter);
				} else {
					base.SetAdapter(value);
				}
			}
		}

		class FullWidthFixedViewLayout : FrameLayout {

			GridViewWithHeaderAndFooter obj;
			public FullWidthFixedViewLayout(Context context,GridViewWithHeaderAndFooter obj) :base(context){
				this.obj=obj;
			}
			protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
			{
				int realLeft = obj.PaddingLeft + PaddingLeft;
				// Try to make where it should be, from left, full width
				if (realLeft != left) {
					OffsetLeftAndRight(realLeft - left);
				}
				base.OnLayout (changed, left, top, right, bottom);
			}
			protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
			{
				int targetWidth = obj.MeasuredWidth- obj.PaddingLeft- obj.PaddingRight;
				widthMeasureSpec = MeasureSpec.MakeMeasureSpec(targetWidth,
					MeasureSpec.GetMode(widthMeasureSpec));
				base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			}
		}
		public override void SetNumColumns (int numColumns)
		{
			base.SetNumColumns (numColumns);
			_numColumns = numColumns;
			IListAdapter adapter = Adapter;
			if (adapter != null && adapter is HeaderViewGridAdapter) {
				((HeaderViewGridAdapter) adapter).setNumColumns(numColumns);
			}
		}
		class HeaderViewGridAdapter : Java.Lang.Object,IWrapperListAdapter, IFilterable {
			// This is used to notify the container of updates relating to number of columns
			// or headers changing, which changes the number of placeholders needed
			DataSetObservable dataSetObservable = new DataSetObservable();
			IListAdapter mAdapter;
			static List<FixedViewInfo> emptyInfoList = new List<FixedViewInfo>();

			// This ArrayList is assumed to NOT be null.
			List<FixedViewInfo> _headerViewInfos;
			List<FixedViewInfo> _footerViewInfos;
			int _numColumns = 1;
			int _rowHeight = -1;
			bool areAllFixedViewsSelectable;
			bool isFilterable;
			bool cachePlaceHoldView = true;
			// From Recycle Bin or calling getView, this a question...
			private bool mCacheFirstHeaderView = false;
			GridViewWithHeaderAndFooter obj;
			public HeaderViewGridAdapter(List<FixedViewInfo> headerViewInfos, List<FixedViewInfo> footViewInfos, IListAdapter adapter,GridViewWithHeaderAndFooter obj) {
				mAdapter = adapter;
				isFilterable = adapter is IFilterable;
				this.obj=obj;
				if (headerViewInfos == null) {
					_headerViewInfos = emptyInfoList;
				} else {
					_headerViewInfos = headerViewInfos;
				}

				if (footViewInfos == null) {
					_footerViewInfos = emptyInfoList;
				} else {
					_footerViewInfos = footViewInfos;
				}
				areAllFixedViewsSelectable = areAllListInfosSelectable(_headerViewInfos)
					&& areAllListInfosSelectable(_footerViewInfos);
			}

			public void setNumColumns(int numColumns) {
				if (numColumns < 1) {
					return;
				}
				if (_numColumns != numColumns) {
					_numColumns = numColumns;
					notifyDataSetChanged();
				}
			}

			public void setRowHeight(int height) {
				_rowHeight = height;
			}

			public int getHeadersCount() {
				return _headerViewInfos.Count;
			}

			public int getFootersCount() {
				return _footerViewInfos.Count;
			}

			bool IListAdapter.AreAllItemsEnabled ()
			{
				if (mAdapter != null) {
					return areAllFixedViewsSelectable && mAdapter.AreAllItemsEnabled();
				} else {
					return true;
				}
			}
			bool IListAdapter.IsEnabled (int position)
			{
				// Header (negative positions will throw an IndexOutOfBoundsException)
				int numHeadersAndPlaceholders = getHeadersCount() * _numColumns;
				if (position < numHeadersAndPlaceholders) {
					return position % _numColumns == 0
						&& _headerViewInfos.ElementAt(position / _numColumns).isSelectable;
				}

				// Adapter
				int adjPosition = position - numHeadersAndPlaceholders;
				int adapterCount = 0;
				if (mAdapter != null) {
					adapterCount = getAdapterAndPlaceHolderCount();
					if (adjPosition < adapterCount) {
						return adjPosition < mAdapter.Count && mAdapter.IsEnabled(adjPosition);
					}
				}

				// Footer (off-limits positions will throw an IndexOutOfBoundsException)
				int footerPosition = adjPosition - adapterCount;
				return footerPosition % _numColumns == 0
					&& _footerViewInfos.ElementAt(footerPosition / _numColumns).isSelectable;
			}
			Java.Lang.Object IAdapter.GetItem (int position)
			{
				// Header (negative positions will throw an ArrayIndexOutOfBoundsException)
				int numHeadersAndPlaceholders = getHeadersCount() * _numColumns;
				if (position < numHeadersAndPlaceholders) {
					if (position % _numColumns == 0) {
						return (Java.Lang.Object)_headerViewInfos.ElementAt(position / _numColumns).data;
					}
					return null;
				}

				// Adapter
				int adjPosition = position - numHeadersAndPlaceholders;
				int adapterCount = 0;
				if (mAdapter != null) {
					adapterCount = getAdapterAndPlaceHolderCount();
					if (adjPosition < adapterCount) {
						if (adjPosition < mAdapter.Count) {
							return mAdapter.GetItem(adjPosition);
						} else {
							return null;
						}
					}
				}

				// Footer (off-limits positions will throw an IndexOutOfBoundsException)
				int footerPosition = adjPosition - adapterCount;
				if (footerPosition % _numColumns == 0) {
					return (Java.Lang.Object)_footerViewInfos.ElementAt(footerPosition).data;
				} else {
					return null;
				}
			}
			long IAdapter.GetItemId (int position)
			{
				int numHeadersAndPlaceholders = getHeadersCount() * _numColumns;
				if (mAdapter != null && position >= numHeadersAndPlaceholders) {
					int adjPosition = position - numHeadersAndPlaceholders;
					int adapterCount = mAdapter.Count;
					if (adjPosition < adapterCount) {
						return mAdapter.GetItemId(adjPosition);
					}
				}
				return -1;
			}
			int IAdapter.GetItemViewType (int position)
			{
				int numHeadersAndPlaceholders = getHeadersCount() * _numColumns;
				int adapterViewTypeStart = mAdapter == null ? 0 : mAdapter.ViewTypeCount - 1;
				int type = AdapterView.ItemViewTypeHeaderOrFooter;
				if (cachePlaceHoldView) {
					// Header
					if (position < numHeadersAndPlaceholders) {
						if (position == 0) {
							if (mCacheFirstHeaderView) {
								type = adapterViewTypeStart + _headerViewInfos.Count + _footerViewInfos.Count + 1 + 1;
							}
						}
						if (position % _numColumns != 0) {
							type = adapterViewTypeStart + (position / _numColumns + 1);
						}
					}
				}

				// Adapter
				int adjPosition = position - numHeadersAndPlaceholders;
				int adapterCount = 0;
				if (mAdapter != null) {
					adapterCount = getAdapterAndPlaceHolderCount();
					if (adjPosition >= 0 && adjPosition < adapterCount) {
						if (adjPosition < mAdapter.Count) {
							type = mAdapter.GetItemViewType(adjPosition);
						} else {
							if (cachePlaceHoldView) {
								type = adapterViewTypeStart + _headerViewInfos.Count + 1;
							}
						}
					}
				}

				if (cachePlaceHoldView) {
					// Footer
					int footerPosition = adjPosition - adapterCount;
					if (footerPosition >= 0 && footerPosition < obj.Count && (footerPosition % _numColumns) != 0) {
						type = adapterViewTypeStart + _headerViewInfos.Count() + 1 + (footerPosition / _numColumns + 1);
					}
				}
				if (_debug) {
					Log.Debug(_logTag, String.Format("getItemViewType: pos: %s, result: %s", position, type, cachePlaceHoldView, mCacheFirstHeaderView));
				}
				return type;
			}
			View IAdapter.GetView (int position, View convertView, ViewGroup parent)
			{
				if (_debug) {
					Log.Debug(_logTag, String.Format("getView: %s, reused: %s", position, convertView == null));
				}
				// Header (negative positions will throw an ArrayIndexOutOfBoundsException)
				int numHeadersAndPlaceholders = getHeadersCount() * _numColumns;
				if (position < numHeadersAndPlaceholders) {
					View headerViewContainer = _headerViewInfos
						.ElementAt(position / _numColumns).viewContainer;
					if (position % _numColumns == 0) {
						return headerViewContainer;
					} else {
						if (convertView == null) {
							convertView = new View(parent.Context);
						}
						// We need to do this because GridView uses the height of the last item
						// in a row to determine the height for the entire row.
						convertView.Visibility=ViewStates.Invisible;
						convertView.SetMinimumHeight(headerViewContainer.Height);
						return convertView;
					}
				}
				// Adapter
				int adjPosition = position - numHeadersAndPlaceholders;
				int adapterCount = 0;
				if (mAdapter != null) {
					adapterCount = getAdapterAndPlaceHolderCount();
					if (adjPosition < adapterCount) {
						if (adjPosition < mAdapter.Count) {
							View view = mAdapter.GetView(adjPosition, convertView, parent);
							return view;
						} else {
							if (convertView == null) {
								convertView = new View(parent.Context);
							}
							convertView.Visibility = ViewStates.Invisible;
							convertView.SetMinimumHeight(_rowHeight);
							return convertView;
						}
					}
				}
				// Footer
				int footerPosition = adjPosition - adapterCount;
				if (footerPosition < obj.Count) {
					View footViewContainer = _footerViewInfos
						.ElementAt(footerPosition / _numColumns).viewContainer;
					if (position % _numColumns == 0) {
						return footViewContainer;
					} else {
						if (convertView == null) {
							convertView = new View(parent.Context);
						}
						// We need to do this because GridView uses the height of the last item
						// in a row to determine the height for the entire row.
						convertView.Visibility = ViewStates.Invisible;
						convertView.SetMinimumHeight(footViewContainer.Height);
						return convertView;
					}
				}
				throw new Java.Lang.ArrayIndexOutOfBoundsException(position);
			}
			void IAdapter.RegisterDataSetObserver (DataSetObserver observer)
			{
				dataSetObservable.RegisterObserver(observer);
				if (mAdapter != null) {
					mAdapter.RegisterDataSetObserver(observer);
				}
			}
			void IAdapter.UnregisterDataSetObserver (DataSetObserver observer)
			{
				dataSetObservable.UnregisterObserver(observer);
				if (mAdapter != null) {
					mAdapter.UnregisterDataSetObserver(observer);
				}
			}
			int IAdapter.Count {
				get {
					if (mAdapter != null) {
						return (getFootersCount() + getHeadersCount()) * _numColumns + getAdapterAndPlaceHolderCount();
					} else {
						return (getFootersCount() + getHeadersCount()) * _numColumns;
					}
				}
			}
			bool IAdapter.HasStableIds {
				get {
					if (mAdapter != null) {
						return mAdapter.HasStableIds;
					}
					return false;
				}
			}
			bool IAdapter.IsEmpty {
				get {
					return (mAdapter == null || mAdapter.IsEmpty) && getHeadersCount() == 0 && getFootersCount() == 0;
				}
			}
			int IAdapter.ViewTypeCount {
				get {
					int count = mAdapter == null ? 1 : mAdapter.ViewTypeCount;
					if (cachePlaceHoldView) {
						int offset = _headerViewInfos.Count + 1 + _footerViewInfos.Count;
						if (mCacheFirstHeaderView) {
							offset += 1;
						}
						count += offset;
					}
					if (_debug) {
						Log.Debug(_logTag, String.Format("getViewTypeCount: %s", count));
					}
					return count;
				}
			}
			IListAdapter IWrapperListAdapter.WrappedAdapter {
				get {
					return mAdapter;
				}
			}

			Filter IFilterable.Filter {
				get {
					if (isFilterable) {
						return ((IFilterable) mAdapter).Filter;
					}
					return null;
				}
			}
			private bool areAllListInfosSelectable(List<FixedViewInfo> infos) {
				if (infos != null) {
					foreach (FixedViewInfo info in infos) {
						if (!info.isSelectable) {
							return false;
						}
					}
				}
				return true;
			}

			public bool removeHeader(View v) {
				for (int i = 0; i < _headerViewInfos.Count; i++) {
					FixedViewInfo info = _headerViewInfos.ElementAt(i);
					if (info.view == v) {
						_headerViewInfos.RemoveAt(i);
						areAllFixedViewsSelectable =
							areAllListInfosSelectable(_headerViewInfos) && areAllListInfosSelectable(_footerViewInfos);
						dataSetObservable.NotifyChanged();
						return true;
					}
				}
				return false;
			}

			public bool removeFooter(View v) {
				for (int i = 0; i < _footerViewInfos.Count; i++) {
					FixedViewInfo info = _footerViewInfos.ElementAt(i);
					if (info.view == v) {
						_footerViewInfos.RemoveAt(i);
						areAllFixedViewsSelectable =
							areAllListInfosSelectable(_headerViewInfos) && areAllListInfosSelectable(_footerViewInfos);
						dataSetObservable.NotifyChanged();
						return true;
					}
				}
				return false;
			}
			private int getAdapterAndPlaceHolderCount() {
				int adapterCount = (int) (Math.Ceiling(1f * mAdapter.Count / _numColumns) * _numColumns);
				return adapterCount;
			}
			public void notifyDataSetChanged() {
				dataSetObservable.NotifyChanged();
			}
		}
	}
}

