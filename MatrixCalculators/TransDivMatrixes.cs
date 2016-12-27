using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace Multiprocessing
{
    class TransDivMatrixes : ProcessMatrixes
    {
        private readonly object _lockObj = new object();

        public TransDivMatrixes(Slider slider, Matrix a, int proc) : base(slider, a, proc)
        {

        }

        private void DividedControlTransposeThread()
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

                C.Matr[myTask.Y, myTask.X] = A.Matr[myTask.X, myTask.Y];

                ChangeDoneWork(100 - NotDone.Count * 100 / Xa / Ya);
            }
        }

        public override void StartProccessing()
        {
            for (var i = 0; i < Processors; i++)
            {
                ProcessorsThreads[i] = new Thread(DividedControlTransposeThread) { Name = i.ToString() };
                ProcessorsThreads[i].Start();
            }
        }
    }
}
