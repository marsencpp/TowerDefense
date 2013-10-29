using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TowerDefense.Properties;
using System.Timers;
using Timer = System.Timers.Timer;

namespace TowerDefense
{
    class Level
    {
        public struct Waypoint
        {
            public Waypoint(Point p, int dir)
            {
                Pt = p;
                NextDir = dir;
            }
            public Point Pt;
            public int NextDir;

        }

        public static int Lives = 3;
        public static List<Enemy> Enemies;
        public static List<Tower> Towers; 
        private static int[,] _globalMap;
        private static Rectangle[] _rects;
        private static Rectangle[] _rectsToDraw;
        public static int LevelWidth;
        public static int LevelHeight;
        public static int WindowWidth;
        public static int WindowHeight;
        private const int BlockSize = 20;
        private static Point _enPoint = new Point(16, 64);
        private static Point _stPoint = new Point(16, 2);
        private static Timer _mainTimer;
        private static Timer _waveTimer;
        private static int Money = 100;

        public Level(int w, int h)
        {
            var maxI = h/BlockSize;
            var maxJ = w/BlockSize;
            WindowWidth = w;
            WindowHeight = h;

            _globalMap = new int[maxI + 1, maxJ + 1];
            _globalMap = Files.ParseLevel(ref _enPoint);
            var globalPoints = new Point[maxI * maxJ * 2];
            for (int i = 0, cntr = 0; i <= maxI; i++)
                for (var j = 0; j <= maxJ; j++, cntr++)
                    globalPoints[cntr] = new Point(i, j);
            _rects = globalPoints.Where(x => _globalMap[x.X, x.Y] == 1002).
                Select(x => new Rectangle(new Point(x.Y * BlockSize, x.X * BlockSize),
                    new Size(BlockSize, BlockSize))).ToArray();
            
            Enemies=new List<Enemy>();
            //Enemies.Insert(0, new Enemy(new Point(15*BlockSize, 4*BlockSize)));
            foreach (var enemy in Enemies)
                CalculatePath(enemy);

            Towers=new List<Tower>();
            
            _mainTimer = new Timer(20);
            _mainTimer.Elapsed += Play;
            _mainTimer.Start();
            _waveTimer = new Timer(3000);
            _waveTimer.Elapsed += NextEnemy;
            _waveTimer.Start();
        }



        private static bool CalculatePath(Enemy enemy)
        {
            var globalMap = new int[_globalMap.GetLength(0), _globalMap.GetLength(1)];
            for (var m = 0; m < _globalMap.GetLength(0); m++)
                for (var j = 0; j < _globalMap.GetLength(1); j++)
                    globalMap[m, j] = _globalMap[m, j];
            var stPoint = GetCell(new Point((int) enemy.PosX, (int) enemy.PosY));
            //Path.GetPathArr(globalMap, globalMap.GetLength(1)+1, stPoint);
            var tmp = Path.GetPath(globalMap, stPoint, _enPoint);
            if (tmp[0].NextDir == 0)
                return false;
            enemy.Waypoints = tmp;
            return true;
        }
        public static void Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255,175,236,231)), 0,0, WindowWidth, WindowHeight );
            var img = Resources.brick;
            //Drawing walls
            foreach (var rect in _rects)
                e.Graphics.DrawImage(img, rect);
            //Drawing enPoint
            e.Graphics.DrawRectangle(new Pen(Color.Blue),
                new Rectangle(_enPoint.Y*BlockSize, _enPoint.X*BlockSize, BlockSize, BlockSize));
            //Drawing info
            e.Graphics.DrawString("Lives: " + Lives, SystemFonts.DefaultFont, new SolidBrush(Color.Black), 50, 50);
            e.Graphics.DrawString("Money: " + Money, SystemFonts.DefaultFont, new SolidBrush(Color.Black), 50, 60);
            //Drawing towers
            foreach (var tower in Towers)
            {
                e.Graphics.DrawImage(img, new Rectangle(tower.Y * BlockSize, tower.X * BlockSize, BlockSize, BlockSize));
                e.Graphics.DrawArc(new Pen(Color.Red), tower.Y*BlockSize - Tower.FireRadius/2 + BlockSize/2,
                    tower.X*BlockSize - Tower.FireRadius/2 + BlockSize/2,
                    Tower.FireRadius, Tower.FireRadius, 0.0f, 360.0f);
                //if(tower.Attacking==null) continue;
                //e.Graphics.DrawLine(new Pen(Color.Green), new Point(tower.Y*BlockSize+BlockSize/2, tower.X*BlockSize+BlockSize/2),
                //    new Point((int) (tower.Attacking.PosX+BlockSize/2), (int) (tower.Attacking.PosY+BlockSize/2)));
            }
            //Drawing enemies
            foreach (var player in Enemies.Where(x => !x.Dead && !x.AtEnd))
            {
                e.Graphics.DrawRectangle(new Pen(Color.Red), new Rectangle((int) player.PosX, (int) player.PosY, BlockSize, BlockSize));
                var pt1 = new Point((int) (player.PosX - 10), (int) (player.PosY - 10));
                var pt2 = new Point(pt1.X + 40, pt1.Y);
                var pt3 = new Point((int)(pt1.X + player.Health / Enemy.MaxHealth * Circle.Distance(pt1, pt2)), pt1.Y);
                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Blue), 2.0f), pt1, pt2);
                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Red), 2.0f), pt1, pt3);
            }
                
        }


        private static void Camera(float x, float y)
        {
            var camera = new Rectangle((int) x-WindowWidth/2+200, (int) y-WindowHeight/2-100, WindowWidth, WindowHeight);
            _rectsToDraw =
                _rects.Select(rect => new Rectangle(rect.X - camera.X, rect.Y - camera.Y, rect.Width, rect.Height))
                    .ToArray();
        }

        //Get number of cell in GlobalMap by given coordinates;
        public static Point GetCell(float x, float y)
        {
            return new Point((int)(y/BlockSize), (int)(x/BlockSize));
        }
        public static Point GetCell(Point pt)
        {
            return new Point(pt.Y/BlockSize, pt.X/BlockSize);
        }
        static void GetSpeed(Enemy enemy)
        {
            switch (enemy.Direction)
            {
                case 1:
                {
                    enemy.VelX = 0;
                    enemy.VelY = -Enemy.Vel;
                    break;
                }
                case 2:
                {
                    enemy.VelX = Enemy.Vel;
                    enemy.VelY = 0;
                    break;
                }
                case 3:
                {
                    enemy.VelX = 0;
                    enemy.VelY = Enemy.Vel;
                    break;
                }
                case 4:
                {
                    enemy.VelX = -Enemy.Vel;
                    enemy.VelY = 0;
                    break;
                }
                case 0:
                {
                    enemy.VelX = 0;
                    enemy.VelY = 0;
                    break;
                }
            }
        }
        //private static bool delete=false;
        public static void MouseClick(object sender, MouseEventArgs e)
        {
            var pt = GetCell(e.Y , e.X);
            if (e.Button==MouseButtons.Left)
            {
                if(Money<Tower.Price) return;
                if (Towers.Any(x => x.Y == pt.X && x.X == pt.Y)) return;
                _globalMap[pt.Y, pt.X] = 1003;
                Towers.Insert(Towers.Count(), new Tower(pt.Y, pt.X));
                Money -= Tower.Price;
            }
            if (e.Button == MouseButtons.Right)
            {
                if (!Towers.Any(x => x.Y == pt.X && x.X == pt.Y)) return;
                _globalMap[pt.Y, pt.X] = 1000;
                Towers.Remove(Towers.Where(x => x.Y == pt.X && x.X == pt.Y).ToList()[0]);
                Money += Tower.Price/2;
            }
            if (e.Button == MouseButtons.Middle)
            {
                _globalMap[pt.Y, pt.X] = 1002;
                _enPoint=new Point(pt.Y, pt.X);
            }
            if (Enemies.All(CalculatePath)) return;
            if (!Towers.Any(x => x.Y == pt.X && x.X == pt.Y)) return;
            _globalMap[pt.Y, pt.X] = 1000;
            Towers.Remove(Towers.Where(x => x.Y == pt.X && x.X == pt.Y).ToList()[0]);
            Money += Tower.Price;
        }
        private static void NextEnemy(object sender, ElapsedEventArgs e)
        {
            if (Enemies.Count() >= 5)
                _waveTimer.Stop();
            var enemy = new Enemy(_stPoint.Y * BlockSize, _stPoint.X * BlockSize);
            CalculatePath(enemy);
            Enemies.Insert(Enemies.Count(), enemy);
        }
        private static void Play(object sender, ElapsedEventArgs e)
        {
            foreach (var player in Enemies.Where(x=>!x.Dead))
            {
                GetDirection(player);
                GetSpeed(player);
                player.PosX += (int)player.VelX;
                player.PosY += (int)player.VelY;
                if (IsAtEnd(player)&&!player.AtEnd)
                {
                    Lives--;
                    player.AtEnd = true;
                }
                foreach (var tower in Towers)
                {
                    if (IsInCircle(tower, player))
                    {
                        if (tower.Attacking&&Enemies.Count(x=>!x.Dead)!=1) continue;
                        tower.Attacking = true;
                        player.Health -= Tower.Power;
                    }
                    else
                        tower.Attacking = false;
                        
                }
                player.Dead = player.Health < 0;
                if (player.Dead)
                    Money += Enemy.Reward;
            }
            if (Enemies.Count(x => !x.Dead) == 1&&Enemies.Count()!=1)
            if (Enemies.Count(x => x.Dead) == Enemies.Count)
                GameOver(1);
            if (Lives == 0)
                GameOver(0);
            Form1.pictureBox1.Invalidate();
            GC.Collect();
        }

        private static bool IsInCircle(Tower tower, Enemy enemy)
        {
            var r = Tower.FireRadius/2;
            var circle = new Circle(tower.Y*BlockSize+BlockSize/2, tower.X*BlockSize+BlockSize/2, r);
            var rectangle = new Rectangle((int) enemy.PosX, (int) enemy.PosY, BlockSize, BlockSize);
            return Circle.Intersect(circle, rectangle);
        }
        private static void GameOver(int result)
        {
            if (result == 0)
                _mainTimer.Stop();
        }

        private static void GetDirection(Enemy enemy)
        {
            var cell = GetCell(new Point((int)enemy.PosX, (int)enemy.PosY));
            enemy.Direction = enemy.Waypoints.Any(x => x.Pt==cell) ? enemy.Waypoints.Where(x => x.Pt == cell).Single().NextDir : 0;
        }

        private static bool IsAtEnd(Enemy enemy)
        {
            return GetCell(new Point((int) enemy.PosX, (int) enemy.PosY)) == _enPoint;
        }
        


        public static void Restart()
        {

        }
    }
}
