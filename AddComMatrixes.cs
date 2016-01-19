using System;
using System.Threading;
using System.Windows.Controls;

namespace Multiprocessing
{
    internal class AddComMatrixes : ProcessMatrixes
    {
        private readonly object _lockObj = new object();
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
            while (true)
            {
                for (var i = 0; i < _tasksForSpecialProcessors.Length; i++)
                    lock (_lockObj)
                    {
                        if (NotDone.Count == 0)
                        {
                            ChangeSlider(100);
                            break;
                        }
                        if (_tasksForSpecialProcessors[i].X != -1) continue;
                        _tasksForSpecialProcessors[i] = NotDone[0];
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
                lock (_lockObj)
                    if (_tasksForSpecialProcessors[myId].X != -1)
                    {
                        myTask = _tasksForSpecialProcessors[myId];
                        _tasksForSpecialProcessors[myId].X = -1;
                    }
                    else
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                C.Matr[myTask.X, myTask.Y] = A.Matr[myTask.X, myTask.Y] + B.Matr[myTask.X, myTask.Y];

                ChangeSlider(100 - NotDone.Count * 100 / X / Y);
                Thread.Sleep(1);
            }
        }

        public void CommonAddition()
        {
            ProcessorsThreads[Processors - 1] = new Thread(ControlThread) { Name = "ThreadControl" };
            ProcessorsThreads[Processors - 1].Start();
            for (var i = 0; i < Processors - 1; i++)
            {
                ProcessorsThreads[i] = new Thread(CombinedControlAdditionThread) { Name = "Thread" + i };
                ProcessorsThreads[i].Start();
            }
        }
    }
}
