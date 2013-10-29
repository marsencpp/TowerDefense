using System.Drawing;

namespace TowerDefense
{
    class Block
    {
        public Rectangle Rect;
        public bool Crackable;
        public bool Moving;
        public Image Sprite;
        public bool Star;
        public Block(Rectangle rectangle, bool star)
        {
            this.Star = star;
            Rect = rectangle;
        }
    }
}
