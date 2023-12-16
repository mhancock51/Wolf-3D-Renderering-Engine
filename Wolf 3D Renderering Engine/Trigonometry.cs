using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TrigonometryLibrary
{
    public static class Trigonometry
    {
        public static float GetAdjacent(float angle, float hypotenuse)
        {
            float radian = DegreeToRadian(angle);
            return hypotenuse * (float)Math.Cos(radian);
        }

        public static float GetOpposite(float angle, float adjacent)
        {
            float radian = DegreeToRadian(angle);
            return adjacent * (float)Math.Tan(radian);
        }

        public static float GetHypotenuse(float angle, float adjacent)
        {
            float radian = DegreeToRadian(angle);
            return adjacent * (float)Math.Cos(radian);
        }

        public static PointF GetDeltaCoordsOfTriangle(float angle, float hypotenuse)
        {
            float radian = DegreeToRadian(angle);

            float dy = hypotenuse * (float)Math.Cos(radian);
            float dx = hypotenuse * (float)Math.Sin(radian);

            return new PointF(dx, -dy);
        }

        public static float DegreeToRadian(float angle)
        {
            return (float)(Math.PI / 180) * angle;
        }

        public static float GetDistanceFromAxisDeltas(float dx, float dy)
        {
            float distance = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            if (distance < 0)
            {
                distance *= -1;
            }
            return distance;
        }
    }
}
