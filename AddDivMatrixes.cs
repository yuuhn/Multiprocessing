using System.Linq;
using System.Threading;
using System.Windows.Controls;

namespace Multiprocessing
{
    //public delegate void AddDelegate();
    
    internal class AddDivMatrixes : ProcessMatrixes
    {
        
        private readonly object _lockObj = new object();

        public AddDivMatrixes(Slider slider, Matrix a, Matrix b, int proc) : base(slider, a, b, proc)
        {
            
        }

        private void DividedControlAdditionThread()
        {
            while (true)
            {
                IntPoint myTask;
                lock (_lockObj)
                    if (NotDone.Count > 0)
                    {
                        myTask = NotDone.First();
                        NotDone.RemoveAt(0);
                    }
                    else
                        return;

                C.Matr[myTask.X, myTask.Y] = A.Matr[myTask.X, myTask.Y] + B.Matr[myTask.X, myTask.Y];

                ChangeSlider(100 - NotDone.Count*100/X/Y);
                Thread.Sleep(1);
            }
        }

        public void DividedAddition()
        {
            for (var i = 0; i < Processors; i++)
            {
                ProcessorsThreads[i] = new Thread(DividedControlAdditionThread) {Name = i.ToString()};
                ProcessorsThreads[i].Start();
            }
        }

        
    }
}
