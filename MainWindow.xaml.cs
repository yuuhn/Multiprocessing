using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Multiprocessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly int[] _dimensions = { 2, 3, 5, 10, 15, 20, 30, 50, 100, 200, 300, 400, 500, 600, 700};
        private Matrix _a;
        private Matrix _b;
        private int _nOfProc = 2;
        private AddDivMatrixes _addDiv;
        private AddComMatrixes _addCom;
        private readonly Random _rnd = new Random();

        // S T O P W A T C H
        private readonly Stopwatch _sw = new Stopwatch();
        private Thread _stopWatch;
        

        public MainWindow()
        {
            InitializeComponent();
            SetComboSource();
        }

        private void SetComboSource()
        {
            ComboAx.ItemsSource = ComboAy.ItemsSource = ComboBx.ItemsSource = ComboBy.ItemsSource = _dimensions;
            ComboAx.SelectedIndex = ComboAy.SelectedIndex = ComboBx.SelectedIndex = ComboBy.SelectedIndex = 1;
        }

        private void BuildMatrixA()
        {
            if (ComboAx.SelectedIndex <= -1) return;
            Slider.Value = 0;
            _a = new Matrix(_dimensions[ComboAx.SelectedIndex], _dimensions[ComboAy.SelectedIndex], _rnd);

            
            var lines = _a.MatrixToString();

            TextBoxA.Text = "";
            foreach (var line in lines)
            {
                TextBoxA.Text += line;
            }
            
        }

        private void BuildMatrixB()
        {
            if (ComboBx.SelectedIndex <= -1) return;
            Slider.Value = 0;
            _b = new Matrix(_dimensions[ComboBx.SelectedIndex], _dimensions[ComboBy.SelectedIndex], _rnd);

            
            var lines = _b.MatrixToString();

            TextBoxB.Text = "";
            foreach (var line in lines)
            {
                TextBoxB.Text += line;
            }
            
        }

        private void ComboAx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BuildMatrixA();
        }

        private void ComboAy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BuildMatrixA();
        }

        private void ComboBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BuildMatrixB();
        }

        private void ComboBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BuildMatrixB();
        }

        private void LockControls(bool enabled)
        {
            ComboAx.IsEnabled = ComboAy.IsEnabled = ComboBx.IsEnabled = ComboBy.IsEnabled = ComboNofProc.IsEnabled = enabled;
            RadioM1.IsEnabled = RadioM2.IsEnabled = enabled;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if ((string)button.Content == "Start")
            {
                if (_a.X != _b.X || _b.Y != _a.Y)
                {
                    TextBoxC.Text = "";
                    LabelStopWatch.Content = "00:00:00";
                    return;
                }

                TextBoxC.Text = "";
                LockControls(false);
                button.Content = "Abort";
                StopWatchAction(true);

                if (RadioM1.IsChecked == true)
                {
                    _addDiv = new AddDivMatrixes(Slider, _a, _b, _nOfProc);
                    _addDiv.DividedAddition();
                }
                else
                {
                    _addCom = new AddComMatrixes(Slider, _a, _b, _nOfProc);
                    _addCom.CommonAddition();
                }
            }
            else
            {
                StopWatchAction(false);
                if (RadioM1.IsChecked == true)
                    _addDiv.Abort();
                else
                    _addCom.Abort();
                button.Content = "Start";
                LockControls(true);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            label1.Content = Slider.Value;
            if ((int)Slider.Value != 100) return;
            StopWatchAction(false);
            string[] lines;
            if (RadioM1.IsChecked == true)
            {
                _addDiv.Abort();
                lines = _addDiv.C.MatrixToString();
            }
            else
            {
                _addCom.Abort();
                lines = _addCom.C.MatrixToString();
            }

            
            TextBoxC.Text = "";
            foreach (var line in lines)
            {
                TextBoxC.Text += line;
            }
            

            button.Content = "Start";
            LockControls(true);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _addDiv?.Abort();
        }

        private void StopWatchAction(bool on)
        {
            if (on)
            {
                _sw.Reset();
                _sw.Start();
                _stopWatch = new Thread(StopWatchThread);
                _stopWatch.Start();
            }
            else
            {
                _stopWatch.Abort();
                _sw.Stop();
            }
        }

        private void StopWatchThread()
        {
            while (true)
            {
                var ts = _sw.Elapsed;
                var currentTime = $"{ts.Minutes:00}:{ts.Seconds:00}:{(double)ts.Milliseconds / 10:00}";
                ChangeLabel(currentTime);
                Thread.Sleep(100);
            }
        }

        private void ChangeLabel(string value)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                LabelStopWatch.Content = value;
            else
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() => { LabelStopWatch.Content = value; }));
            }
        }

        private void ComboNofProc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RadioM1 == null || RadioM2 == null)
                return;

            if (ComboNofProc.SelectedIndex == 0)
            {
                RadioM1.IsChecked = true;
                RadioM1.Content = "Han Solo";
                RadioM2.Visibility = Visibility.Hidden;
            }
            else
            {
                RadioM1.Content = "M1";
                RadioM2.Visibility = Visibility.Visible;
            }
            _nOfProc = Convert.ToInt16(((Button)ComboNofProc.SelectedValue).Content);
        }
    }
}
