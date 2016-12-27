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
        private readonly int[] _dimensions = { 2, 3, 5, 10, 15, 20, 30, 50, 100, 200, 300, 400, 500, 600, 700 };
        private Matrix _a;
        private Matrix _b;
        private int _nOfProc = 2;

        private ProcessMatrixes _matrices;

        private readonly Random _rnd = new Random();

        private int _chosenAction = 0;

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
            ComboAx.IsEnabled = ComboAy.IsEnabled = ComboNofProc.IsEnabled = enabled;
            RadioM1.IsEnabled = RadioM2.IsEnabled = enabled;
            ComboBx.IsEnabled = ComboBy.IsEnabled = _chosenAction == 2 ? false : enabled;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!_sw.IsRunning)
            {
                if (
                    ((_a.X != _b.X || _b.Y != _a.Y) && _chosenAction == 0)
                    || (_a.Y != _b.X && _chosenAction == 1)
                    )
                {
                    TextBoxC.Text = "";
                    LabelStopWatch.Content = "00:00.00";
                    return;
                }

                TextBoxC.Text = "";
                LockControls(false);
                button.Content = "Abort";

                switch (_chosenAction)
                {
                    case 0:
                        if ((bool)RadioM1.IsChecked)
                            _matrices = new AddDivMatrixes(Slider, _a, _b, _nOfProc);
                        else
                            _matrices = new AddComMatrixes(Slider, _a, _b, _nOfProc);
                        break;
                    case 1:
                        if ((bool)RadioM1.IsChecked)
                            _matrices = new MultDivMatrices(Slider, _a, _b, _nOfProc);
                        else
                            _matrices = new MultComMatrices(Slider, _a, _b, _nOfProc);
                        break;
                    default:
                        if ((bool)RadioM1.IsChecked)
                            _matrices = new TransDivMatrixes(Slider, _a, _nOfProc);
                        else
                            _matrices = new TransComMatrixes(Slider, _a, _nOfProc);
                        break;
                }
                
                _matrices.StartProccessing();
                StopWatchAction(true);
            }
            else
            {
                StopWatchAction(false);
                _matrices.Abort();
                
                button.Content = "Start";
                LockControls(true);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            label1.Content = Slider.Value;
            if ((int)Slider.Value != 100)
                return;

            StopWatchAction(false);
            
            string[] lines;

            _matrices.Abort();
            lines = _matrices.C.MatrixToString();

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
            _matrices?.Abort();
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
                _stopWatch?.Abort();
                _sw.Stop();
            }
        }

        private void StopWatchThread()
        {
            while (true)
            {
                var ts = _sw.Elapsed;
                var currentTime = $"{ts.Minutes:00}:{ts.Seconds:00}.{(double)ts.Milliseconds / 10:00}";
                ChangeLabel(currentTime);
                Thread.Sleep(100);
            }
        }

        private void ChangeSlider(int value)
        {
            if (Dispatcher.CheckAccess())
                Slider.Value = value;
            else
            {
                Dispatcher.BeginInvoke(new Action(() => { Slider.Value = value; }));
            }
        }

        private void ChangeLabel(string value)
        {
            if (Dispatcher.CheckAccess())
                LabelStopWatch.Content = value;
            else
            {
                Dispatcher.BeginInvoke(new Action(() => { LabelStopWatch.Content = value; }));
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

        private void ActiveMatrixB(bool active)
        {
            ComboBx.IsEnabled = ComboBy.IsEnabled = TextBoxB.IsEnabled = active;
        }

        private void RadioAddition_Checked(object sender, RoutedEventArgs e)
        {
            ActiveMatrixB(true);
            _chosenAction = 0;
        }

        private void RadioMult_Checked(object sender, RoutedEventArgs e)
        {
            ActiveMatrixB(true);
            _chosenAction = 1;
        }

        private void RadioTransp_Checked(object sender, RoutedEventArgs e)
        {
            ActiveMatrixB(false);
            _chosenAction = 2;
        }
    }
}
