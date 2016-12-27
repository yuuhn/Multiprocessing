using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace Multiprocessing
{
    internal class MultDivMatrices : ProcessMatrixes
    {
        private readonly object _lockObj = new object();

        public MultDivMatrices(Slider slider, Matrix a, Matrix b, int proc) : base(slider, a, b, proc)
        {
        }

        private void DividedControlMultiplicationThread()
        {
            IntPoint myTask;
            while (true)
            {
                lock (_lockObj)
                {
                    if (NotDone.Count > 0)
                    {
                        myTask = NotDone.First();
                        NotDone.RemoveAt(0);
                    }
                    else
                        return;
                }

                int cell = 0;
                for (int i = 0; i < Ya; i++)
                {
                    cell += A.Matr[myTask.X, i] * B.Matr[i, myTask.Y];
                }

                C.Matr[myTask.X, myTask.Y] = cell;

                ChangeDoneWork(100 - NotDone.Count * 100 / Xa / Yb);
            }
        }

        public override void StartProccessing()
        {
            for (var i = 0; i < Processors; i++)
            {
                ProcessorsThreads[i] = new Thread(DividedControlMultiplicationThread) { Name = i.ToString() };
                ProcessorsThreads[i].Start();
            }
        }

    }
}
