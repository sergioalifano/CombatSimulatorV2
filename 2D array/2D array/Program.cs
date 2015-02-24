using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2D_array
{
    class Program
    {
        static void Main(string[] args)
        {
            int counter = 1;
            int[,] my2DArray = new int[3, 3];

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    my2DArray[x, y] = counter;
                    counter++;
                }
            }

            //2D array of points
            Point[,] pointArray = new Point[10, 10];

            
            for (int y = 0; y < pointArray.GetLength(1); y++)
            {
                //for each row
                for (int x = 0; x < pointArray.GetLength(0); x++)
                {
                    //for each column
                    pointArray[x, y] = new Point(x, y);
                }
            }
           
            //using arrows to movement
            ConsoleKeyInfo input = Console.ReadKey();
            switch (input.Key)
            {
                case ConsoleKey.LeftArrow:
                    break;


            }
        }
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x,int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
