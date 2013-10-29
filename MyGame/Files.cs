using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using TowerDefense.Properties;

namespace TowerDefense
{
    class Files
    {
        public static int[,] ParseLevel(ref Point en)
        {
            var str = Resources.level;
            var strings = str.Split('\n');
            var len = strings[0].Length-1;
            var i = strings.Count()-1;
            var result = new int[i, len];
            for(var x = 0; x < i; x++)
                for (var y = 0; y < len; y++)
                {
                    if (strings[x][y] == '-')
                    {
                        result[x,y] = 1002;
                        continue;
                    }
                    if (strings[x][y] == 'X')
                        //st = new Point(x,y);
                    if (strings[x][y] == 'Y')
                        en = new Point(x, y);
                    result[x, y] = 1000;
                }
            return result;
        }

        public static void PrintLevel(int[,] arr, Level.Waypoint[] waypts)
        {
            var sw = new StreamWriter(@"D:\level.txt");
            for (var i = 0; i < arr.GetLength(0); i++)
            {
                var sb = new StringBuilder();
                for (var j = 0; j < arr.GetLength(1); j++)
                {
                    if (arr[i, j] == 1002)
                    {
                        sb.Append("X");
                        continue;
                    }
                    if (waypts.Any(x => x.Pt == new Point(i,j)))
                    {
                        sb.Append("-");
                        continue;
                    }
                    sb.Append(" ");

                }
                sw.WriteLine(sb.ToString());
            }
            sw.Close();
        }
    }
}
