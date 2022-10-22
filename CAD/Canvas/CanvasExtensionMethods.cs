
namespace CAD.Canvas
{
    static class CanvasExtensionMethods
    {
        public static System.Windows.Point ToWpfPoint(this System.Drawing.Point aPoint)
        {
            return new System.Windows.Point(aPoint.X, aPoint.Y);
        }
        public static System.Drawing.Point FromWpfPoint(this System.Windows.Point aPoint)
        {
            return new System.Drawing.Point((int)aPoint.X, (int)aPoint.Y);
        }
        public static  System.Drawing.RectangleF ToRectangleF(this System.Windows.Rect rect)
        {
            return new System.Drawing.RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
        }
        public static System.Windows.Rect FromRectangleF(this System.Drawing.RectangleF rect)
        {
            return new System.Windows.Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
