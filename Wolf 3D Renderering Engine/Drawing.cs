using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DrawingLibrary
{
    public static class Drawing
    {
        public static Bitmap DrawRectangle(Bitmap frame, float x, float y, float width, float height, Color colour)
        {
            Graphics graphics = Graphics.FromImage(frame);
            graphics.FillRectangle(new SolidBrush(colour), new RectangleF(x, y, width, height));
            graphics.Dispose();
            return frame;
        }

        public static Bitmap DrawRectangleOutline(Bitmap frame, float x, float y, float width, float height, Color colour)
        {
            Graphics graphics = Graphics.FromImage(frame);
            graphics.DrawRectangle(new Pen(colour), new Rectangle((int)x, (int)y, (int)width, (int)height));
            graphics.Dispose();
            return frame;
        }

        public static Bitmap DrawCircle(Bitmap frame, float x, float y, float size, Color colour)
        {
            Graphics graphics = Graphics.FromImage(frame);
            graphics.FillEllipse(new SolidBrush(colour), new RectangleF(x, y, size, size));
            graphics.Dispose();
            return frame;
        }

        public static Bitmap DrawCircleOutline(Bitmap frame, float x, float y, float size, Color colour)
        {
            Graphics graphics = Graphics.FromImage(frame);
            graphics.DrawEllipse(new Pen(colour), new RectangleF(x, y, size, size));
            graphics.Dispose();
            return frame;
        }

        public static Bitmap DrawLine(Bitmap frame, PointF pt1, PointF pt2, Color colour, int width)
        {
            Graphics graphics = Graphics.FromImage(frame);
            graphics.DrawLine(new Pen(colour, width), pt1, pt2);
            graphics.Dispose();
            return frame;
        }

        public static Bitmap DrawImage(Bitmap frame, Image image, Point point)
        {
            Graphics graphics = Graphics.FromImage(frame);
            graphics.DrawImage(image, point);
            graphics.Dispose();
            return frame;
        }
    }
}
