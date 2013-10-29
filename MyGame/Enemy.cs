using System.Drawing;

namespace TowerDefense
{
    class Enemy
    {
        public static Point StartPoint;
        public float PosX;
        public float PosY;
        public Rectangle Rect=new Rectangle(StartPoint, new Size(10, 10));
        public float VelX;
        public float VelY;
        public static float VelHor =  7.5f;
        public static float VelVert = 7.5f;
        public static float Vel = 1;
        public static float Jumppower = -17.5f;
        public Level.Waypoint[] Waypoints;
        public bool Attacked;
        public bool AtEnd;
        public static float MaxHealth = 20;
        public float Health = MaxHealth;
        public bool Dead;
        public static int Reward = 10;
        public Enemy(Point pt)
        {
            PosX = pt.X;
            PosY = pt.Y;
        }
        public Enemy(int X, int Y)
        {
            PosX = X;
            PosY = Y;
        }


        public int Direction;
    }
}
