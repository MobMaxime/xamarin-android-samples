using Android.Graphics;
using Android.Views.Animations;

namespace SlidingMenuSharp
{
    public interface ICanvasTransformer
    {
        /**
         * Transform canvas.
         *
         * @param canvas the canvas
         * @param percentOpen the percent open
         */
        void TransformCanvas(Canvas canvas, float percentOpen);
    }

    public class ZoomTransformer : ICanvasTransformer
    {
        public void TransformCanvas(Canvas canvas, float percentOpen)
        {
            var scale = (float) (percentOpen * 0.25 + 0.75);
            canvas.Scale(scale, scale, canvas.Width / 2f, canvas.Height / 2f);
        }
    }

    public class SlideTransformer : ICanvasTransformer
    {
        private static readonly SlideInterpolator Interpolator = new SlideInterpolator();
        public class SlideInterpolator : Java.Lang.Object, IInterpolator
        {
            public float GetInterpolation(float t)
            {
                t -= 1.0f;
                return t * t * t + 1.0f;
            }
        }

        public void TransformCanvas(Canvas canvas, float percentOpen)
        {
            canvas.Translate(0, canvas.Height * (1 - Interpolator.GetInterpolation(percentOpen)));
        }
    }

    public class ScaleTransformer : ICanvasTransformer
    {
        public void TransformCanvas(Canvas canvas, float percentOpen)
        {
            canvas.Scale(percentOpen, 1, 0, 0);
        }
    }
}