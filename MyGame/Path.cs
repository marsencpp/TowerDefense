using System.Drawing;
using System.Linq;

namespace TowerDefense
{
    class Path
    {
        private static int _qHead;
        private static int _qTail;
        private static int[] _queue=new int[10000];
        private static int _nextX;
        private static int _nextY;
        public static void GetPathArr(int[,] defArr, int len, Point stPoint)
        {
            _queue = new int[10000];
            defArr[stPoint.X, stPoint.Y] = 0;
            _qHead = 0;
            _qTail = 0;
            _queue[_qHead] = GetNum(stPoint.X, stPoint.Y, len);
            while (_qHead <= _qTail)
            {
                var x=0;
                var y=0;
                GetIJ(ref x, ref y, len, _queue[_qHead]);
                Check(defArr, x, y + 1, len, defArr[x,y]);
                Check(defArr, x, y - 1, len, defArr[x, y]);
                Check(defArr, x + 1, y, len, defArr[x, y]);
                Check(defArr, x - 1, y, len, defArr[x, y]);
                _qHead++;
            }
        }

        public static void GetIJ(ref int i, ref int j, int l, int n)
        {
            j = n%l;
            i = n/l;
        }

        public static int GetNum(int i, int j, int l)
        {
            return i*l + j;
        }

        static void Check(int[,] arr, int x, int y, int len, int prev)
        {
            if(arr[x, y]==1002||arr[x, y]==1003)
                return;
            if (arr[x, y] > prev + 1)
                arr[x, y] = prev + 1;
            else return;
            _queue[++_qTail] = GetNum(x, y, len);
        }

        public static Level.Waypoint[] GetPath(int[,] defArr, Point st, Point en)
        {
            GetPathArr(defArr, defArr.GetLength(1)+1, st);
            var counter = 0;
            //PathArr = defArr;
            var res = new Level.Waypoint[10000];
            if (defArr[en.X, en.Y] == 1000) return res;
            _nextX = en.X;
            _nextY = en.Y;
            while (_nextX != st.X || _nextY != st.Y)
            {
                res[counter++].Pt=new Point(_nextX, _nextY);
                CheckAdjacentCells(defArr, _nextX, _nextY);
            }
            res[counter].Pt = new Point(_nextX, _nextY);
            res = res.Where(x => x.Pt!=new Point(0, 0)).Reverse().ToArray();
            for (var i = 0; i < counter; i++)
                res[i].NextDir = GetDir(res[i].Pt, res[i+1].Pt);
            return res;
        }

        static int GetDir(Point pt1, Point pt2)
        {
            if (pt1.X - pt2.X > 0) return 1;
            if (pt1.Y - pt2.Y < 0) return 2;
            if (pt1.X - pt2.X < 0) return 3;
            if (pt1.Y - pt2.Y > 0) return 4;
            return 0;
        }

        static void CheckAdjacentCells(int[,] arr, int x, int y)
        {
            var x1 = arr[x + 1, y];
            var x2 = arr[x - 1, y];
            var x3 = arr[x, y + 1];
            var x4 = arr[x, y - 1];
            if (x1 <= x2 && x1 <= x3 && x1 <= x4) {_nextX = x + 1;return;}
            if (x2 <= x1 && x2 <= x3 && x2 <= x4) {_nextX = x - 1;return;}
            if (x3 <= x2 && x3 <= x1 && x3 <= x4) {_nextY = y + 1;return;}
            if (x4 <= x2 && x4 <= x3 && x4 <= x1) {_nextY = y - 1;return;}
        }
        
    }
}
