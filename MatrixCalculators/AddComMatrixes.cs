using System;
using System.Threading;
using System.Windows.Controls;

namespace Multiprocessing
{
    internal class AddComMatrixes : ProcessMatrixes
    {
        private readonly object[] _lockObjs;
        private readonly IntPoint[] _tasksForSpecialProcessors;

        public AddComMatrixes(Slider slider, Matrix a, Matrix b, int proc) : base(slider, a, b, proc + 1)
        {
            _tasksForSpecialProcessors = new IntPoint[proc];
            _lockObjs = new object[proc];
            for (int i = 0; i < proc; i++)
            {
                _lockObjs[i] = new object();
            }
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


            while (true)
            {
                for (var i = 0; i < _tasksForSpecialProcessors.Length; i++)
                {
                    counter++;
                    if (NotDone.Count == 0)
                    {
                        //ChangeSlider(100);
                        break;
                    }

                    if (_tasksForSpecialProcessors[i].X != -1) continue;
                    lock (_lockObjs[i])
                    {
                        _tasksForSpecialProcessors[i] = NotDone[0];
                    }
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
            while (true)
            {
                IntPoint myTask;
                lock (_lockObjs[myId])
                {
                    if (_tasksForSpecialProcessors[myId].X == -1)
                    {
                        continue;
                    }

                    myTask = _tasksForSpecialProcessors[myId];
                    _tasksForSpecialProcessors[myId].X = -1;
                }

                C.Matr[myTask.X, myTask.Y] = A.Matr[myTask.X, myTask.Y] + B.Matr[myTask.X, myTask.Y];

                
                if (100 - NotDone.Count * 100 / Xa / Ya == 100)
                {
                    ChangeDoneWork(100);
                }
                

                //ChangeDoneWork(100 - NotDone.Count * 100 / Xa / Ya);
                //ChangeSlider(100 - NotDone.Count * 100 / Xa / Ya);
                //Thread.Sleep(1);
            }
        }

        public override void StartProccessing()
        {
            ProcessorsThreads[Processors - 1] = new Thread(ControlThread) { Name = "ThreadControl" };
            ProcessorsThreads[Processors - 1].Start();
        }
    }
}
