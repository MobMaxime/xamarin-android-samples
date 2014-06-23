using System;

namespace SlidingMenuSharp
{
    public class PageSelectedEventArgs : EventArgs
    {
        public int Position { get; set; }
    }

    public class PageScrolledEventArgs : PageSelectedEventArgs
    {
        public float PositionOffset { get; set; }
        public int PositionOffsetPixels { get; set; }
    }

    public class PageScrollStateChangedEventArgs : EventArgs
    {
        public int State { get; set; }
    }
}