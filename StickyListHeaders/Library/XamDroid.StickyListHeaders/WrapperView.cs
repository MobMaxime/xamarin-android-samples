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

using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Java.Lang;

namespace com.refractored.components.stickylistheaders
{
    /// <summary>
    /// View that wraps a divider header and a normal list item. THe list view sees this as 1 item
    /// </summary>
    public class WrapperView : ViewGroup
    {
        /// <summary>
        /// Gets or sets the current item
        /// </summary>
        public View Item { get; private set; }
        /// <summary>
        /// Gets or sets the Divider
        /// </summary>
        public Drawable Divider { get; private set; }
        /// <summary>
        /// Gets or sets the divider height
        /// </summary>
        public int DividerHeight { get; private set; }
        /// <summary>
        /// Gets or sets the current header
        /// </summary>
        public View Header { get; private set; }
        /// <summary>
        /// Gets or sets the item top position
        /// </summary>
        public int ItemTop { get; private set; }

        public WrapperView(Context context)
            : base(context)
        {

        }

        public void Update(View item, View header, Drawable divider, int dividerHeight)
        {
            if (item == null)
                throw new NullPointerException("List view item must not be null.");

            //Remove the current item if it isn't the same
            //Incase there is recycling of views
            if (Item != item)
            {
                RemoveView(Item);
                Item = item;
                var parent = item.Parent as ViewGroup;
                if (parent != null && parent != this)
                {
                    parent.RemoveView(item);
                }

                AddView(item);
            }

            //Also try this for the header
            if (Header != header)
            {
                if (Header != null)
                    RemoveView(Header);

                Header = header;
                if (header != null)
                    AddView(header);
            }

            if (Divider != divider)
            {
                Divider = divider;
                DividerHeight = dividerHeight;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets if it has a header
        /// </summary>
        public bool HasHeader { get { return Header != null; } }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var measuredWidth = MeasureSpec.GetSize(widthMeasureSpec);
            var childWidthMeasureSpec = MeasureSpec.MakeMeasureSpec(measuredWidth, MeasureSpecMode.Exactly);
            var measuredHeight = 0;

            var height = 0;
            var mode = MeasureSpecMode.Unspecified;
            //Measer the header or the deivider, when there is a header visible it will act as the divider
            if (Header != null)
            {

                if (Header.LayoutParameters != null && Header.LayoutParameters.Height > 0)
                {
                    height = Header.LayoutParameters.Height;
                    mode = MeasureSpecMode.Exactly;
                    
                }
                Header.Measure(childWidthMeasureSpec,
                                   MeasureSpec.MakeMeasureSpec(height, mode));

                measuredHeight += Header.MeasuredHeight;
            }
            else if (Divider != null)
            {
                measuredHeight += DividerHeight;
            }

            //Measure the item
            height = 0;
            mode = MeasureSpecMode.Unspecified;
            if (Item.LayoutParameters != null && Item.LayoutParameters.Height > 0)
            {
                height = Item.LayoutParameters.Height;
                mode = MeasureSpecMode.Exactly;
            }
            Item.Measure(childWidthMeasureSpec,
                               MeasureSpec.MakeMeasureSpec(height, mode));

            measuredHeight += Item.MeasuredHeight;

            SetMeasuredDimension(measuredWidth, measuredHeight);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            l = 0;
            t = 0;
            r = Width;
            b = Height;

            if (Header != null)
            {
                var headerHeight = Header.MeasuredHeight;
                Header.Layout(l, t, r, headerHeight);
                ItemTop = headerHeight;
                Item.Layout(l, headerHeight, r, b);
            }
            else if (Divider != null)
            {
                Divider.SetBounds(l, t, r, DividerHeight);
                ItemTop = DividerHeight;
                Item.Layout(l, DividerHeight, r, b);
            }
            else
            {
                ItemTop = t;
                Item.Layout(l, t, r, b);
            }
        }

        protected override void DispatchDraw(Android.Graphics.Canvas canvas)
        {
            base.DispatchDraw(canvas);

            if (Header == null && Divider != null)
            {
                //Drawable.setbounds does not work on pre honeycomb, so you have to do a little work around
                //for anything pre-HC.
                if ((int) Build.VERSION.SdkInt < 11)
                {
                    canvas.ClipRect(0, 0, Width, DividerHeight);
                }
                Divider.Draw(canvas);
            }
        }
    }
}