using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Multiprocessing
{
    internal struct IntPoint
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
        protected int Xa, Ya, Xb, Yb;
        protected readonly int Processors;
        protected List<IntPoint> NotDone = new List<IntPoint>();

        public int DoneOfWork = 0;
        private object _objAccess;
        public int counter = 0;

        public ProcessMatrixes(Slider slider, Matrix a, Matrix b, int proc)
        {
            _objAccess = new object();
            Processors = proc;
            ProcessorsThreads = new Thread[Processors];
            Slider = slider;
            ChangeSlider(0);
            A = a;
            B = b;
            Xa = a.X;
            Ya = a.Y;
            Xb = b.X;
            Yb = b.Y;

            C = new Matrix(Xa, Yb);
            FillNotDone();
        }

        protected void ChangeDoneWork(int newDone)
        {
            lock (_objAccess)
            {
                if (DoneOfWork < newDone)
                {
                    DoneOfWork = newDone;
                }
            }
        }

        public virtual void StartProccessing() { }

        public ProcessMatrixes(Slider slider, Matrix a, int proc)
        {
            _objAccess = new object();
            Processors = proc;
            ProcessorsThreads = new Thread[Processors];
            Slider = slider;
            ChangeSlider(0);
            A = a;
            B = a;
            Xa = a.X;
            Ya = a.Y;
            Xb = a.X;
            Yb = a.Y;

            C = new Matrix(Ya, Xa);
            FillNotDone();
        }

        private void FillNotDone()
        {
            for (var i = 0; i < Xa; i++)
                for (var j = 0; j < Yb; j++)
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
                thread?.Abort();
        }
    }
}
