namespace TowerDefense
{
    class Tower
    {
        public int X;
        public int Y;
        public static int FireRadius = 100;
        public bool Attacking;
        public static float Power = 0.1f;
        public static int Price = 30;
        

        public Tower(int x, int y)
        {
            X = x;
            Y = y;
        }

    }
}
