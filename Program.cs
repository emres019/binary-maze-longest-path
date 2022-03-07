using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleAppFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            int row, col;
            Matrix matrix = new Matrix(12, 9);

            matrix.Fill();
            matrix.Display();

            Console.Write("Başlangıç noktasının satırı: ");
            row = Convert.ToInt32(Console.ReadLine());

            Console.Write("Başlangıç noktasının sütunu: ");
            col = Convert.ToInt32(Console.ReadLine());

            Point startPoint = matrix.points[row, col];

            Console.Write("Bitiş noktasının satırı: ");
            row = Convert.ToInt32(Console.ReadLine());

            Console.Write("Bitiş noktasının sütunu: ");
            col = Convert.ToInt32(Console.ReadLine());

            Point endPoint = matrix.points[row, col];

            new Path(startPoint, endPoint, matrix).Next();

            Console.WriteLine("\n");
            matrix.Display();

            Console.ReadKey();
        }
    }
    
    /// <summary>
    /// Matristeki her bir nokta
    /// </summary>
    class Point
    {
        internal short col, row;
        internal byte value;
        internal ConsoleColor color;

        internal Point(short row, short col, byte value)
        {
            this.row = row;
            this.col = col;
            this.value = value;
            this.color = (value == 0) ? ConsoleColor.Yellow : ConsoleColor.Blue;
        }
    }

    class Path
    {
        static Path LongestPath = null;
        static Point TargetPoint;
        static Matrix matrix;

        /// <summary>
        /// Yolun geçtiği noktaları depolayan set.
        /// </summary>
        HashSet<Point> points;

        /// <summary>
        /// Mevcut bulunulan nokta.
        /// </summary>
        Point curPoint;

        internal Path(Point startPoint)
        {
            if (startPoint.value == 0)
            {
                throw new ArgumentException("Başlangıç noktasının değeri 1 olmalıdır.");
            }

            this.curPoint = startPoint;
            this.points = new HashSet<Point>();
            this.points.Add(curPoint);
        }

        internal Path(Point startPoint, Point TargetPoint, Matrix matrix) : this(startPoint)
        {
            if (TargetPoint.value == 0)
            {
                throw new ArgumentException("Bitiş noktasının değeri 1 olmalıdır.");
            }

            Path.TargetPoint = TargetPoint;
            Path.matrix = matrix;
        }

        /// <summary>
        /// Yan yol oluşturan constructor. Ana yolun geçtiği noktaları kendi set'ine ekler.
        /// </summary>
        /// <param name="startPoint">Yolun başlangıç noktası</param>
        /// <param name="prevPath">Ana yol</param>
        internal Path(Point startPoint, Path prevPath) : this(startPoint)
        {
            this.points.UnionWith(prevPath.points);
        }

        /// <summary>
        /// Yolun ilerlemesini veya yan yollara ayrılmasını sağlayan recursive fonksiyon.
        /// </summary>
        internal void Next()
        {
            if (curPoint == TargetPoint)
            {
                points.Add(TargetPoint);

                if (LongestPath == null || points.Count > LongestPath.points.Count)
                {
                    if (LongestPath != null)
                    {
                        ClearMark();
                    }

                    Mark();
                    LongestPath = this;
                }

                return;
            }

            // curPoint'in komşu noktaları
            Point belowPoint = (curPoint.row < matrix.height - 1) ? matrix.points[curPoint.row + 1, curPoint.col] : null;
            Point abovePoint = (curPoint.row > 0) ? matrix.points[curPoint.row - 1, curPoint.col] : null;
            Point rightPoint = (curPoint.col < matrix.width - 1) ? matrix.points[curPoint.row, curPoint.col + 1] : null;
            Point leftPoint = (curPoint.col > 0) ? matrix.points[curPoint.row, curPoint.col - 1] : null;

            Point nextPoint = null;
            bool isFirstJunction = true;

            if (rightPoint != null && rightPoint.value != 0 && !points.Contains(rightPoint))
            {
                nextPoint = rightPoint;
                isFirstJunction = false;
            }

            if (leftPoint != null && leftPoint.value != 0 && !points.Contains(leftPoint))
            {
                if (isFirstJunction)
                {
                    nextPoint = leftPoint;
                    isFirstJunction = false;
                }
                else
                {
                    new Path(leftPoint, this).Next();
                }
            }

            if (abovePoint != null && abovePoint.value != 0 && !points.Contains(abovePoint))
            {
                if (isFirstJunction)
                {
                    nextPoint = abovePoint;
                    isFirstJunction = false;
                }
                else
                {
                    new Path(abovePoint, this).Next();
                }
            }

            if (belowPoint != null && belowPoint.value != 0 && !points.Contains(belowPoint))
            {
                if (isFirstJunction)
                {
                    nextPoint = belowPoint;
                    isFirstJunction = false;
                }
                else
                {
                    new Path(belowPoint, this).Next();
                }
            }

            if (nextPoint != null)
            {
                curPoint = nextPoint;
                points.Add(nextPoint);
                Next();
            }
        }

        /// <summary>
        /// Yolun geçtiği noktaları işaretleyen fonksiyon.
        /// </summary>
        internal void Mark()
        {
            foreach (Point p in points)
            {
                p.color = ConsoleColor.Red;
            }
        }

        /// <summary>
        /// İşaretlenmiş noktaların işaretlerini kaldıran fonksiyon.
        /// </summary>
        internal static void ClearMark()
        {
            foreach (Point p in LongestPath.points)
            {
                p.color = ConsoleColor.Blue;
            }
        }
    }

    class Matrix
    {
        static Random random = new Random();

        internal short height, width;
        internal Point[,] points;

        internal Matrix(short height, short width)
        {
            this.height = height;
            this.width = width;
            this.points = new Point[height, width];
        }

        /// <summary>
        /// Matrisi rastgele bitlerle dolduran fonksiyon.
        /// </summary>
        internal void Fill()
        {
            // Bu fonksiyon yaklaşık nokta_sayısı / 4 saniye sürecek yani yükseklik * genişlik / 4
            for (short row = 0; row < height; row++)
            {
                for (short column = 0; column < width; column++)
                {
                    points[row, column] = new Point(row, column, (byte)random.Next(2));
                    Thread.Sleep(250);
                }
            }
        }

        /// <summary>
        /// Matrisi ekrana yazdıran fonksiyon.
        /// </summary>
        internal void Display()
        {
            for (short row = 0; row < height; row++)
            {
                Console.ResetColor();
                Console.Write("[");

                for (short column = 0; column < width - 1; column++)
                {
                    Point p = points[row, column];

                    Console.ForegroundColor = p.color;
                    Console.Write(p.value);

                    Console.ResetColor();
                    Console.Write(", ");
                }

                Console.ForegroundColor = points[row, width - 1].color;
                Console.Write(points[row, width - 1].value);

                Console.ResetColor();
                Console.WriteLine("]");
            }
        }
    }
}