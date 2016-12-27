using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace Multiprocessing
{
    internal class MultComMatrices : ProcessMatrixes
    {
        //private readonly object[] _lockObjs;
        private readonly IntPoint[] _tasksForSpecialProcessors;

        public MultComMatrices(Slider slider, Matrix a, Matrix b, int proc) : base(slider, a, b, proc + 1)
        {
            _tasksForSpecialProcessors = new IntPoint[proc];
            /*
            _lockObjs = new object[proc];
            for (int i = 0; i < proc; i++)
            {
                _lockObjs[i] = new object();
            }
            */
        }

        private void ControlThread()
        {
            for (var i = 0; i < Processors - 1; i++)
            {
                if (NotDone.Count == 0)
                    _tasksForSpecialProcessors[i] = new IntPoint() { X = -1, Y = -1 };
                else
                {
                    _tasksForSpecialProcessors[i] = NotDone[0];
                    NotDone.RemoveAt(0);
                }
            }


            for (var i = 0; i < Processors - 1; i++)
            {
                ProcessorsThreads[i] = new Thread(CombinedControlMultiplicationThread) { Name = "Thread" + i };
                ProcessorsThreads[i].Start();
            }


            while (true)
            {
                for (var i = 0; i < _tasksForSpecialProcessors.Length; i++)
                {
                    if (NotDone.Count == 0)
                    {
                        //ChangeSlider(100);
                        break;
                    }

                    if (_tasksForSpecialProcessors[i].X != -1) continue;
                    /*
                    lock (_lockObjs[i])
                    {
                        _tasksForSpecialProcessors[i] = NotDone[0];
                    }
                    */

                    _tasksForSpecialProcessors[i] = NotDone[0];
                    NotDone.RemoveAt(0);
                }
            }
        }

        private void CombinedControlMultiplicationThread()
        {
            int myId;
            if (Thread.CurrentThread.Name != null)
                myId = Convert.ToInt16(Thread.CurrentThread.Name.Substring(6, Thread.CurrentThread.Name.Length - 6));
            else
                return;

            IntPoint myTask;

            while (true)
            {
                if (_tasksForSpecialProcessors[myId].X == -1)
                {
                    continue;
                }

                myTask = _tasksForSpecialProcessors[myId];
                _tasksForSpecialProcessors[myId].X = -1;

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
            ProcessorsThreads[Processors - 1] = new Thread(ControlThread) { Name = "ThreadControl" };
            ProcessorsThreads[Processors - 1].Start();
        }
    }
}
