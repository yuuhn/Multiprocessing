using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiprocessing
{
    internal class Matrix
    {
        public int X, Y;
        public int[,] Matr;
        private Random _rnd;

        public Matrix(int x, int y, Random rnd)
        {
            X = x;
            Y = y;
            _rnd = rnd;
            Matr = new int[x, y];
            FillMatrix();
        }

        public Matrix(int x, int y)
        {
            X = x;
            Y = y;
            Matr = new int[x, y];
        }

        private void FillMatrix()
        {

            for (var i = 0; i < X; i++)
                for (var j = 0; j < Y; j++)
                    Matr[i, j] = _rnd.Next(-9, 10);
        }

        public string[] MatrixToString()
        {
            string[] lines = new string[X];

            for (var i = 0; i < X; i++)
            {
                var formatMatr = "";
                for (var j = 0; j < Y; j++)
                {
                    formatMatr += $"{Matr[i, j],4}" + ";";
                }
                lines[i] = formatMatr + "\n";
            }

            return lines;
        }
    }
}
