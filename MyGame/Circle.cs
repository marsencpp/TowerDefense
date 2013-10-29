using System;
using System.Drawing;

namespace TowerDefense
{
    class Circle
    {
        public float Radius;
        public float X;
        public float Y;

        public Circle(float x, float y, float r)
        {
            X = x;
            Y = y;
            Radius = r;
        }

        public Circle(Point pt, float r)
        {
            X = pt.X;
            Y = pt.Y;
            Radius = r;
        }

        public static float Distance(Point pt1, Point pt2)
        {
            var x = pt1.X - pt2.X;
            var y = pt1.Y - pt2.Y;
            return (float) Math.Sqrt(x*x + y*y);
        }
        public static bool Intersect(Circle circle, Rectangle rectangle)
        {
            var maxY = rectangle.Bottom;
            var minY = rectangle.Top;
            var maxX = rectangle.Right;
            var minX = rectangle.Left;
            var sq = 0;
            var pt = new Point((int) circle.X, (int) circle.Y);
            if (pt.X < minX && pt.Y < minY) sq = 1;
            if (pt.X > minX && pt.X < maxX && pt.Y < minY) sq = 2;
            if (pt.X > maxX && pt.Y < minY) sq = 3;
            if (pt.X > maxX && pt.Y > minY && pt.Y < maxY) sq = 4;
            if (pt.X > maxX && pt.Y > maxY) sq = 5;
            if (pt.X > minX && pt.X < maxX && pt.Y > maxY) sq = 6;
            if (pt.X < minX && pt.Y > maxY) sq = 7;
            if (pt.X < minX && pt.Y > minY && pt.X < maxY) sq = 8;
            switch (sq)
            {
                case 1:
                    return Distance(pt, new Point(minX, minY)) < circle.Radius;
                case 2:
                    return Distance(pt, new Point(pt.X, minY)) < circle.Radius;
                case 3:
                    return Distance(pt, new Point(maxX, minY)) < circle.Radius;
                case 4:
                    return Distance(pt, new Point(maxX, pt.Y)) < circle.Radius;
                case 5:
                    return Distance(pt, new Point(maxX, maxY)) < circle.Radius;
                case 6:
                    return Distance(pt, new Point(pt.X, maxY)) < circle.Radius;
                case 7:
                    return Distance(pt, new Point(minX, maxY)) < circle.Radius;
                case 8:
                    return Distance(pt, new Point(minX, pt.Y)) < circle.Radius;
                default: 
                    return false;
            }
        }
    }
}
