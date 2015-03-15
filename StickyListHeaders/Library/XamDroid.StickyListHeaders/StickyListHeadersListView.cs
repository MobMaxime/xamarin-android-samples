/*
 * Copyright (C) 2013 @JamesMontemagno http://www.montemagno.com http://www.refractored.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Converted from: https://github.com/emilsjolander/StickyListHeaders
 */
using System;
using System.Collections.Generic;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using com.refractored.components.stickylistheaders.Interfaces;
using Exception = System.Exception;
using Math = System.Math;

namespace com.refractored.components.stickylistheaders
{
    public class StickyListHeadersListView : ListView, AbsListView.IOnScrollListener
    {
        public IOnScrollListener OnScrollListenerDelegate { get; set; }

        private bool m_AreHeadersSticky = true;
        /// <summary>
        /// gets or sets if the headers are sticky
        /// </summary>
        public bool AreHeadersSticky
        {
            get { return m_AreHeadersSticky; }
            set
            {
                if (m_AreHeadersSticky == value)
                    return;

                m_AreHeadersSticky = value;
                RequestLayout();
            }
        }

        private IOnHeaderListClickListener m_OnHeaderListClickListener;
        public IOnHeaderListClickListener OnHeaderListClickListener
        {
            get { return m_OnHeaderListClickListener; }
            set
            {
                m_OnHeaderListClickListener = value;
                m_AdapterHeaderAdapterClickListener = new AdapterHeaderAdapterClickListener(m_OnHeaderListClickListener, this);

            }
        }
        public bool IsDrawingListUnderStickyHeader { get; set; }

        private int m_HeaderBottomPosition;
        private View m_Header;
        private int m_DividerHeight;
        private Drawable m_Divider;
        private bool m_ClippingToPadding;
        private Rect m_ClippingRect = new Rect();
		private string m_CurrentHeaderId;
        private AdapterWrapper m_Adapter;
        private float m_HeaderDownY = -1;
        private bool m_HeaderBeingPressed = false;
        private int m_HeaderPosition;
        private ViewConfiguration m_ViewConfiguration;
        private List<View> m_FooterViews;
        private Rect m_SelectorRect = new Rect(); //for if reflection fails
        private IntPtr m_SelectorPositionField;
        private AdapterHeaderAdapterClickListener m_AdapterHeaderAdapterClickListener;
        private DataSetObserver m_DataSetObserver;
        private bool _initialized;

        private class AdapterHeaderAdapterClickListener : IOnHeaderAdapterClickListener
        {
            private readonly IOnHeaderListClickListener m_OnHeaderListClickListener;
            private readonly StickyListHeadersListView m_StickyListHeadersListView;

            public AdapterHeaderAdapterClickListener(IOnHeaderListClickListener listClickListener, StickyListHeadersListView stickyListHeadersListView)
            {
                m_OnHeaderListClickListener = listClickListener;
                m_StickyListHeadersListView = stickyListHeadersListView;
            }

			public void OnHeaderClick(View header, int itemPosition, string headerId)
            {
                if (m_OnHeaderListClickListener == null)
                    return;

                m_OnHeaderListClickListener.OnHeaderClick(m_StickyListHeadersListView, header, itemPosition, headerId, false);
            }
        }

        private class StickyListHeadersListViewObserver : DataSetObserver
        {
            private readonly StickyListHeadersListView m_ListView;
            public StickyListHeadersListViewObserver(StickyListHeadersListView listView)
            {
                m_ListView = listView;
            }
            public override void OnInvalidated()
            {
                m_ListView.Reset();
            }

            public override void OnChanged()
            {
                m_ListView.Reset();
            }
        }


        public StickyListHeadersListView(System.IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {

        }


        public StickyListHeadersListView(Context context)
            : this(context, null)
        {

        }

        public StickyListHeadersListView(Context context, IAttributeSet attrs) :
            this(context, attrs, Android.Resource.Attribute.ListViewStyle)
        {

        }

        public StickyListHeadersListView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            if (!_initialized)
            {
                m_DataSetObserver = new StickyListHeadersListViewObserver(this);
                m_AdapterHeaderAdapterClickListener = new AdapterHeaderAdapterClickListener(OnHeaderListClickListener, this);

                base.SetOnScrollListener(this);
                //null out divider, dividers are handled by adapter so they look good with headers
                base.Divider = null;
                base.DividerHeight = 0;

                m_ViewConfiguration = ViewConfiguration.Get(context);
                m_ClippingToPadding = true;

                try
                {
                    //reflection to get selector ref
                    var absListViewClass = JNIEnv.FindClass(typeof(AbsListView));
                    var selectorRectId = JNIEnv.GetFieldID(absListViewClass, "mSelectorRect", "()Landroid/graphics/Rect");
                    var selectorRectField = JNIEnv.GetObjectField(absListViewClass, selectorRectId);
                    m_SelectorRect = Java.Lang.Object.GetObject<Rect>(selectorRectField, JniHandleOwnership.TransferLocalRef);

                    var selectorPositionId = JNIEnv.GetFieldID(absListViewClass, "mSelectorPosition", "()Ljava/lang/Integer");
                    m_SelectorPositionField = JNIEnv.GetObjectField(absListViewClass, selectorPositionId);
                }
                catch (Exception)
                {

                }
                _initialized = true;
            }
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();

            Initialize(this.Context);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            if (changed)
            {
                Reset();
                ScrollChanged(FirstVisiblePosition);
            }
        }

        private void Reset()
        {
            m_Header = null;
			m_CurrentHeaderId =null;
            m_HeaderPosition = -1;
            m_HeaderBottomPosition = -1;
        }

        public override bool PerformItemClick(View view, int position, long id)
        {
            if (view is WrapperView)
                view = ((WrapperView)view).Item;

            return base.PerformItemClick(view, position, id);
        }

        public override void SetSelectionFromTop(int position, int y)
        {
            if (HasStickyHeaderAtPosition(position))
                y += GetHeaderHeight();

            base.SetSelectionFromTop(position, y);
        }


#if __ANDROID_11__

        public override void SmoothScrollToPositionFromTop(int position, int offset)
        {
            if (HasStickyHeaderAtPosition(position))
                    offset += GetHeaderHeight();

            base.SmoothScrollToPositionFromTop(position, offset);
        }

        public override void SmoothScrollToPositionFromTop(int position, int offset, int duration)
        {
            if (HasStickyHeaderAtPosition(position))
                    offset += GetHeaderHeight();

            base.SmoothScrollToPositionFromTop(position, offset, duration);
        }
#endif

        private bool HasStickyHeaderAtPosition(int position)
        {
            position -= HeaderViewsCount;
            return AreHeadersSticky && position > 0 &&
                   position < m_Adapter.Count &&
                   m_Adapter.GetHeaderId(position) == m_Adapter.GetHeaderId(position - 1);
        }

        public override Drawable Divider
        {
            get { return m_Divider; }
            set
            {
                m_Divider = value;
                if (m_Divider != null)
                {
                    var dividerDrawableHeight = m_Divider.IntrinsicHeight;
                    if (dividerDrawableHeight >= 0)
                    {
                        DividerHeight = dividerDrawableHeight;
                    }
                }

                if (m_Adapter != null)
                {
                    m_Adapter.Divider = m_Divider;
                    RequestLayout();
                    Invalidate();
                }
            }
        }

        public override int DividerHeight
        {
            get
            {
                return m_DividerHeight;
            }
            set
            {
                m_DividerHeight = value;
                if (m_Adapter != null)
                {
                    m_Adapter.DividerHeight = m_DividerHeight;
                    RequestLayout();
                    Invalidate();
                }
            }
        }

        public override void SetOnScrollListener(IOnScrollListener l)
        {
            OnScrollListenerDelegate = l;
        }

        public override IListAdapter Adapter
        {
            get
            {
                return base.Adapter;
            }
            set
            {
                if (IsInEditMode)
                {
                    base.Adapter = value;
                    return;
                }

                if (value == null)
                {
                    m_Adapter = null;
                    Reset();
                    base.Adapter = null;
                    return;
                }

                if (!(value is IStickyListHeadersAdapter))
                {
                    throw new IllegalArgumentException("Adapter must implement IStickyListHeadersAdapater");
                }

                m_Adapter = WrapAdapter(value);
                Reset();
                base.Adapter = m_Adapter;

            }
        }

        private AdapterWrapper WrapAdapter(IListAdapter adapter)
        {
            AdapterWrapper wrapper = null;
            var indexer = adapter as ISectionIndexer;
            wrapper = indexer != null ? new SectionIndexerAdapterWrapper(Context, indexer) : new AdapterWrapper(Context, adapter);

            wrapper.Divider = m_Divider;
            wrapper.DividerHeight = m_DividerHeight;
            wrapper.RegisterDataSetObserver(m_DataSetObserver);
            wrapper.OnHeaderAdapterClickListener = m_AdapterHeaderAdapterClickListener;
            return wrapper;
        }

        public IStickyListHeadersAdapter WrappedAdapter
        {
            get { return m_Adapter == null ? null : m_Adapter.Delegate; }
        }

        public View GetWrappedView(int position)
        {
            var view = GetChildAt(position);
            var wrapperView = view as WrapperView;
            if (wrapperView != null)
            {
                return wrapperView.Item;
            }

            return view;
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            if ((int)Build.VERSION.SdkInt < 8) //froyo
            {
                ScrollChanged(FirstVisiblePosition);
            }

            PositionSelectorRect();

            if (!AreHeadersSticky || m_Header == null)
            {
                base.DispatchDraw(canvas);
                return;
            }

            if (!IsDrawingListUnderStickyHeader)
            {
                m_ClippingRect.Set(0, m_HeaderBottomPosition, Width, Height);
                canvas.Save();
                canvas.ClipRect(m_ClippingRect);
            }

            base.DispatchDraw(canvas);

            if (!IsDrawingListUnderStickyHeader)
            {
                canvas.Restore();
            }

            DrawStickyHeader(canvas);
        }

        private void PositionSelectorRect()
        {
            if (m_SelectorRect.IsEmpty)
                return;

            var selectorPosition = GetSelectorPosition();
            if (selectorPosition < 0)
                return;

            var firstVisibleItem = FixedFirstVisibleItem(FirstVisiblePosition);
            var view = GetChildAt(selectorPosition - firstVisibleItem) as WrapperView;
            if (view == null)
                return;
            m_SelectorRect.Top = view.Top + view.ItemTop;
        }

        private int GetSelectorPosition()
        {
            if (m_SelectorPositionField == IntPtr.Zero)//
            {
                for (int i = 0; i < ChildCount; i++)
                {
                    if (GetChildAt(i).Bottom == m_SelectorRect.Bottom)
                        return i + FixedFirstVisibleItem(FirstVisiblePosition);
                }
            }
            else
            {
                try
                {
                    return GetObject<Integer>(m_SelectorPositionField, JniHandleOwnership.TransferLocalRef).IntValue();
                }
                catch (Exception)
                {

                }
            }


            return -1;
        }

        private void DrawStickyHeader(Canvas canvas)
        {
            var headerHeight = GetHeaderHeight();
            var top = m_HeaderBottomPosition - headerHeight;

            //clip the headers drawing area
            m_ClippingRect.Left = PaddingLeft;
            m_ClippingRect.Right = Width - PaddingRight;
            m_ClippingRect.Bottom = top + headerHeight;
            m_ClippingRect.Top = m_ClippingToPadding ? PaddingTop : 0;

            canvas.Save();
            canvas.ClipRect(m_ClippingRect);
            canvas.Translate(PaddingLeft, top);
            m_Header.Draw(canvas);
            canvas.Restore();
        }

        private void MeasureHeader()
        {
            var widthMeasureSpec = MeasureSpec.MakeMeasureSpec(Width - PaddingLeft - PaddingRight -
                                                               (IsScrollBarOverlay() ? 0 : VerticalScrollbarWidth),
                                                               MeasureSpecMode.Exactly);

            var heightMeasureSpec = 0;
            var layoutParams = m_Header.LayoutParameters;
            if (layoutParams == null)
            {
                m_Header.LayoutParameters = new MarginLayoutParams(ViewGroup.LayoutParams.MatchParent,
                                                                             ViewGroup.LayoutParams.WrapContent);
            }
            if (layoutParams != null && layoutParams.Height > 0)
            {
                heightMeasureSpec = MeasureSpec.MakeMeasureSpec(layoutParams.Height, MeasureSpecMode.Exactly);
            }
            else
            {
                heightMeasureSpec = MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            }

            m_Header.Measure(widthMeasureSpec, heightMeasureSpec);
			//TODO  for rtl support
//#if __ANDROID_17__
//            if ((int)Build.VERSION.SdkInt >= 17) //JB_MR1
//            {
//				m_Header.LayoutDirection =  this.LayoutDirection;
//            }
//#endif

            m_Header.Layout(PaddingLeft, 0, Width - PaddingRight, m_Header.MeasuredHeight);
        }

        private bool IsScrollBarOverlay()
        {
            return ScrollBarStyle == ScrollbarStyles.InsideOverlay || ScrollBarStyle == ScrollbarStyles.OutsideOverlay;

        }

        private int GetHeaderHeight()
        {
            return m_Header == null ? 0 : m_Header.MeasuredHeight;
        }

        public override void SetClipToPadding(bool clipToPadding)
        {
            base.SetClipToPadding(clipToPadding);
            m_ClippingToPadding = clipToPadding;
        }

        private void ScrollChanged(int reportedFirstVisibleItem)
        {
            var adapaterCount = m_Adapter == null ? 0 : m_Adapter.Count;
            if (adapaterCount == 0 || !AreHeadersSticky)
                return;

            var listViewHeaderCount = HeaderViewsCount;
            var firstVisibleItem = FixedFirstVisibleItem(reportedFirstVisibleItem) - listViewHeaderCount;

            if (firstVisibleItem < 0 || firstVisibleItem > adapaterCount - 1)
            {
                Reset();
                UpdateHeaderVisibilities();
                Invalidate();
                return;
            }

            if (m_HeaderPosition == -1 || m_HeaderPosition != firstVisibleItem)
            {
                m_HeaderPosition = firstVisibleItem;
                m_CurrentHeaderId = m_Adapter.GetHeaderId(firstVisibleItem);
                m_Header = m_Adapter.GetHeaderView(m_HeaderPosition, m_Header, this);
                MeasureHeader();
            }

            var childCount = ChildCount;
            if (childCount != 0)
            {
                View viewToWatch = null;
                var watchingChildDistance = int.MaxValue;
                var viewToWatchIsFooter = false;
                for (int i = 0; i < childCount; i++)
                {
                    var child = base.GetChildAt(i);
                    var childIsFooter = m_FooterViews != null && m_FooterViews.Contains(child);
                    var childDistance = child.Top - (m_ClippingToPadding ? PaddingTop : 0);
                    if (childDistance < 0)
                        continue;

                    if (viewToWatch == null ||
                        (!viewToWatchIsFooter && !((WrapperView)viewToWatch).HasHeader) ||
                        ((childIsFooter || ((WrapperView)child).HasHeader) && childDistance < watchingChildDistance))
                    {
                        viewToWatch = child;
                        viewToWatchIsFooter = childIsFooter;
                        watchingChildDistance = childDistance;
                    }
                }

                var headerHeight = GetHeaderHeight();
                if (viewToWatch != null && (viewToWatchIsFooter || ((WrapperView)viewToWatch).HasHeader))
                {
                    if (firstVisibleItem == listViewHeaderCount && base.GetChildAt(0).Top > 0 && !m_ClippingToPadding)
                    {
                        m_HeaderBottomPosition = 0;
                    }
                    else
                    {
                        var paddingTop = m_ClippingToPadding ? PaddingTop : 0;
                        m_HeaderBottomPosition = Math.Min(viewToWatch.Top, headerHeight + paddingTop);
                        m_HeaderBottomPosition = m_HeaderBottomPosition < paddingTop
                                                     ? headerHeight + paddingTop
                                                     : m_HeaderBottomPosition;

                    }
                }
                else
                {
                    m_HeaderBottomPosition = headerHeight + (m_ClippingToPadding ? PaddingTop : 0);
                }

            }

            UpdateHeaderVisibilities();
            Invalidate();
        }

        public override void AddFooterView(View v)
        {
            base.AddFooterView(v);
            if (m_FooterViews == null)
                m_FooterViews = new List<View>();

            m_FooterViews.Add(v);
        }

        public override bool RemoveFooterView(View v)
        {
            if (base.RemoveFooterView(v))
            {
                m_FooterViews.Remove(v);
                return true;
            }

            return false;
        }

        private void UpdateHeaderVisibilities()
        {
            var top = m_ClippingToPadding ? PaddingTop : 0;
            var childCount = ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = base.GetChildAt(i) as WrapperView;
                if (child == null)
                    continue;

                if (!child.HasHeader)
                    continue;

                var childHeader = child.Header;
                childHeader.Visibility = child.Top < top ? ViewStates.Invisible : ViewStates.Visible;
            }
        }

        private int FixedFirstVisibleItem(int firstVisibileItem)
        {
            if ((int)Build.VERSION.SdkInt >= 11) //HC
            {
                return firstVisibileItem;
            }

            for (int i = 0; i < ChildCount; i++)
            {
                if (GetChildAt(i).Bottom >= 0)
                {
                    firstVisibileItem += i;
                    break;
                }
            }

            //Work around to fix bug with firstVisibileItem being to high beacuse
            //ListView does not take clipTOPadding=false into account
            if (!m_ClippingToPadding && PaddingTop > 0 && base.GetChildAt(0).Top > 0 && firstVisibileItem > 0)
            {
                firstVisibileItem -= 1;
            }

            return firstVisibileItem;
        }

        //TODO handle touched better, multitouch etc.
        public override bool OnTouchEvent(MotionEvent e)
        {

            var action = e.Action;
            if (action == MotionEventActions.Down && e.GetY() <= m_HeaderBottomPosition)
            {
                m_HeaderDownY = e.GetY();
                m_HeaderBeingPressed = true;
                m_Header.Pressed = true;
                m_Header.Invalidate();
                Invalidate(0, 0, Width, m_HeaderBottomPosition);
                return true;
            }

            if (m_HeaderBeingPressed)
            {
                if (Math.Abs(e.GetY() - m_HeaderDownY) < m_ViewConfiguration.ScaledTouchSlop)
                {
                    if (action == MotionEventActions.Up || action == MotionEventActions.Cancel)
                    {
                        m_HeaderDownY = -1;
                        m_HeaderBeingPressed = false;
                        m_Header.Pressed = false;
                        m_Header.Invalidate();
                        Invalidate(0, 0, Width, m_HeaderBottomPosition);
                        if (OnHeaderListClickListener != null)
                            OnHeaderListClickListener.OnHeaderClick(this, m_Header, m_HeaderPosition, m_CurrentHeaderId, true);
                    }
                    return true;
                }

                m_HeaderDownY = -1;
                m_HeaderBeingPressed = false;
                m_Header.Pressed = false;
                m_Header.Invalidate();
                Invalidate(0, 0, Width, m_HeaderBottomPosition);
            }
            return base.OnTouchEvent(e);
        }


        public virtual void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            if (OnScrollListenerDelegate != null)
                OnScrollListenerDelegate.OnScroll(view, firstVisibleItem, visibleItemCount, totalItemCount);

            if ((int)Build.VERSION.SdkInt >= 8)//FROYO
                ScrollChanged(firstVisibleItem);
        }

        public virtual void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
            if (OnScrollListenerDelegate == null)
                return;

            OnScrollListenerDelegate.OnScrollStateChanged(view, scrollState);
        }
    }
}