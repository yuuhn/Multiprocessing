using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace Multiprocessing
{
    class TransComMatrixes : ProcessMatrixes
    {
        //private readonly object[] _lockObjs;
        private readonly IntPoint[] _tasksForSpecialProcessors;

        public TransComMatrixes(Slider slider, Matrix a, int proc) : base(slider, a, proc + 1)
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
                ProcessorsThreads[i] = new Thread(CombinedControlTransposeThread) { Name = "Thread" + i };
                ProcessorsThreads[i].Start();
            }


            while (true)
            {
                for (var i = 0; i < _tasksForSpecialProcessors.Length; i++)
                {
                    if (NotDone.Count == 0)
                    {
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

        private void CombinedControlTransposeThread()
        {
            int myId;
            if (Thread.CurrentThread.Name != null)
                myId = Convert.ToInt16(Thread.CurrentThread.Name.Substring(6, Thread.CurrentThread.Name.Length - 6));
            else
                return;
            while (true)
            {
                IntPoint myTask;

                if (_tasksForSpecialProcessors[myId].X == -1)
                {
                    continue;
                }

                myTask = _tasksForSpecialProcessors[myId];
                _tasksForSpecialProcessors[myId].X = -1;

                C.Matr[myTask.Y, myTask.X] = A.Matr[myTask.X, myTask.Y];

                ChangeDoneWork(100 - NotDone.Count * 100 / Xa / Ya);
            }
        }

        public override void StartProccessing()
        {
            ProcessorsThreads[Processors - 1] = new Thread(ControlThread) { Name = "ThreadControl" };
            ProcessorsThreads[Processors - 1].Start();
        }
    }
}
