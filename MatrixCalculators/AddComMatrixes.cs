using System;
using System.Threading;
using System.Windows.Controls;

namespace Multiprocessing
{
    internal class AddComMatrixes : ProcessMatrixes
    {
        //private readonly object[] _lockObjs;
        private readonly IntPoint[] _tasksForSpecialProcessors;

        public AddComMatrixes(Slider slider, Matrix a, Matrix b, int proc) : base(slider, a, b, proc + 1)
        {
            _tasksForSpecialProcessors = new IntPoint[proc];
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
                ProcessorsThreads[i] = new Thread(CombinedControlAdditionThread) { Name = "Thread" + i };
                ProcessorsThreads[i].Start();
            }

            int j = -1;

            while (true)
            {
                j++;
                if (j == Processors - 1)
                    j = 0;
                
                if (NotDone.Count == 0)
                    break;

                if (_tasksForSpecialProcessors[j].X == -1)
                {
                    _tasksForSpecialProcessors[j] = NotDone[0];
                    NotDone.RemoveAt(0);
                }
                
            }
        }

        private void CombinedControlAdditionThread()
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

                C.Matr[myTask.X, myTask.Y] = A.Matr[myTask.X, myTask.Y] + B.Matr[myTask.X, myTask.Y];
                
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
