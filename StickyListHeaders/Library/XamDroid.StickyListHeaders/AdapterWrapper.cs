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
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Java.Lang;
using com.refractored.components.stickylistheaders.Interfaces;

namespace com.refractored.components.stickylistheaders
{
    /// <summary>
    /// A ListAdapater which wraps a StickyListHeadersAdapter and automatically handles
    /// wrapping the result of StickyListHeadersAdapter.GetView and StickyListAdapter.GetGetHeaderView
    /// </summary>
    public class AdapterWrapper : BaseAdapter, IStickyListHeadersAdapter
    {
        public IStickyListHeadersAdapter Delegate { get; set; }

        private readonly List<View> m_HeaderCache = new List<View>();
        private readonly Context m_Context;
        public IOnHeaderAdapterClickListener OnHeaderAdapterClickListener { get; set; }
        private DataSetObserver m_DataSetObserver;

        public AdapterWrapper(Context context, object adapterDelegate)
        {
            m_Context = context;
            Delegate = adapterDelegate as IStickyListHeadersAdapter;

            if(Delegate == null)
                throw new NullReferenceException("Adapter Delegate must be of type IStickyListHeadersAdapter");
            m_DataSetObserver = new AdapterWrapperObserver(this, m_HeaderCache);
           
            Delegate.RegisterDataSetObserver(m_DataSetObserver);
        }

        public class AdapterWrapperObserver : DataSetObserver
        {
            private List<View> m_HeaderCache;
            private AdapterWrapper m_Wrapper;
            public AdapterWrapperObserver(AdapterWrapper wrapper, List<View> headerCache)
            {
                m_Wrapper = wrapper;
                m_HeaderCache = headerCache;
            }
            public override void OnInvalidated()
            {
                m_HeaderCache.Clear();
               m_Wrapper.Super_NotifyDataSetInvalidated();
            }

            public override void OnChanged()
            {
               m_Wrapper.Super_NotifyDataSetChanged();
            }
        }

        /// <summary>
        /// Gets or sets the divider
        /// </summary>
        public Drawable Divider { get; set; }

        /// <summary>
        /// Gets or sets the divider height
        /// </summary>
        public int DividerHeight { get; set; }

        #region Delegate Overrides
        public override bool AreAllItemsEnabled()
        {
            return Delegate.AreAllItemsEnabled();
        }

        public override bool IsEnabled(int position)
        {
            return Delegate.IsEnabled(position);
        }

        public override int Count
        {
            get { return Delegate.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return Delegate.GetItem(position);
        }

        public override long GetItemId(int position)
        {
            return Delegate.GetItemId(position);
        }

        public override bool HasStableIds
        {
            get
            {
                if (Delegate == null)
                    return true;

                return Delegate.HasStableIds;
            }
        }

        public override int GetItemViewType(int position)
        {
            return Delegate.GetItemViewType(position);
        }

        public override int ViewTypeCount
        {
            get
            {
                return Delegate.ViewTypeCount;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return Delegate.IsEmpty;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var wrapperView = (convertView == null) ? new WrapperView(m_Context) : convertView as WrapperView;

            if(wrapperView == null)
                throw new NullReferenceException("Wrapper view can not be null");

            var item = Delegate.GetView(position, wrapperView.Item, wrapperView);
            View header = null;
            if (PreviousPositionHasSameHeader(position))
            {
                RecycleHeaderIfExists(wrapperView);
            }
            else
            {
                header = ConfigureHeader(wrapperView, position);
            }

            if ((item is ICheckable) && !(wrapperView is CheckableWrapperView))
            {
                //Need to create Checkable subclass of WrapperView for listview to work correctly
                wrapperView = new CheckableWrapperView(m_Context);
            }
            else if (!(item is ICheckable) && (wrapperView is CheckableWrapperView))
            {
                wrapperView = new WrapperView(m_Context);
            }

            wrapperView.Update(item, header, Divider, DividerHeight);
            return wrapperView;

        }

        public override bool Equals(Java.Lang.Object o)
        {
            return Delegate.Equals(o);
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            return ((BaseAdapter)Delegate).GetDropDownView(position, convertView, parent);
        }

        public override int GetHashCode()
        {
            return Delegate.GetHashCode();
        }

        public override void NotifyDataSetChanged()
        {
            ((BaseAdapter)Delegate).NotifyDataSetChanged();
        }

       public void Super_NotifyDataSetChanged()
       {
           base.NotifyDataSetChanged();
       }

        public override void NotifyDataSetInvalidated()
        {
            ((BaseAdapter)Delegate).NotifyDataSetInvalidated();
        }

       public void Super_NotifyDataSetInvalidated()
       {
           base.NotifyDataSetInvalidated();
       }

        public override string ToString()
        {
            return Delegate.ToString();
        }

        #endregion

        /// <summary>
        /// Will recycle header from WrapperView if it exists
        /// </summary>
        /// <param name="wrapperView">wrapper view where header exists</param>
        private void RecycleHeaderIfExists(WrapperView wrapperView)
        {
            var header = wrapperView.Header;
            if(header != null)
                m_HeaderCache.Add(header);
        }

        /// <summary>
        /// Get a header view. This optionally pulls a header from the supplied
        /// Wrapper view and will also recycle it if it exists
        /// </summary>
        /// <param name="wrapperView">Wrapper view to pull header from</param>
        /// <param name="position">Position of the header</param>
        /// <returns>New Header view</returns>
        private View ConfigureHeader(WrapperView wrapperView, int position)
        {
            var header = wrapperView.Header ?? PopHeader();
            header = Delegate.GetHeaderView(position, header, wrapperView);
            if(header == null)
                throw  new NullPointerException("Header view must not be null.");

            header.Clickable = true;
            header.Click += (sender, args) =>
                {
                    if (OnHeaderAdapterClickListener == null)
                        return;

                    var headerId = Delegate.GetHeaderId(position);
                    OnHeaderAdapterClickListener.OnHeaderClick((View)sender, position, headerId);
                };

            return header;
        }

        /// <summary>
        /// get the bottom of the header cache and remove
        /// </summary>
        /// <returns></returns>
        private View PopHeader()
        {
            if (m_HeaderCache.Count <= 0)
            {
                return null;
            }

            var header = m_HeaderCache[0];
            m_HeaderCache.RemoveAt(0);
            return header;
        }

        /// <summary>
        /// Checks if the previous position has the same header ID.
        /// </summary>
        /// <param name="position">Position to check</param>
        /// <returns>True if it does</returns>
        private bool PreviousPositionHasSameHeader(int position)
        {
            return position != 0 && Delegate.GetHeaderId(position) == Delegate.GetHeaderId(position - 1);
        }


        public View GetHeaderView(int position, View convertView, ViewGroup parent)
        {
            return Delegate.GetHeaderView(position, convertView, parent);
        }

		public string GetHeaderId(int position)
        {
            return Delegate.GetHeaderId(position);
        }
    }
}
