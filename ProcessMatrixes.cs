using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Multiprocessing
{
    public struct IntPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    internal class ProcessMatrixes
    {
        protected readonly Slider Slider;
        protected readonly Thread[] ProcessorsThreads;
        protected readonly Matrix A, B;
        public Matrix C;
        protected int X, Y;
        protected readonly int Processors;
        protected List<IntPoint> NotDone = new List<IntPoint>();

        public ProcessMatrixes(Slider slider, Matrix a, Matrix b, int proc)
        {
            Processors = proc;
            ProcessorsThreads = new Thread[Processors];
            Slider = slider;
            ChangeSlider(0);
            A = a;
            B = b;
            X = a.X;
            Y = a.Y;
            C = new Matrix(X, Y);
            FillNotDone();
        }

        private void FillNotDone()
        {
            for (var i = 0; i < X; i++)
                for (var j = 0; j < Y; j++)
                    NotDone.Add(new IntPoint() { X = i, Y = j });
        }

        protected void ChangeSlider(int value)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                Slider.Value = value;
            else
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() => { Slider.Value = value; }));
            }
        }

        public void Abort()
        {
            foreach (var thread in ProcessorsThreads.Where(thread => thread != null && thread.ThreadState != ThreadState.Stopped))
                thread.Abort();
        }
    }
}
