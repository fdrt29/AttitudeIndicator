using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GyroHorizon.Annotations;

namespace GyroHorizon
{
    public partial class GyroHorizonUI : UserControl, INotifyPropertyChanged
    {
        public GyroHorizonUI()
        {
            InitializeComponent();
            _vm = new GyroHorizonVM(this);
            SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;
        }


        private GyroHorizonVM _vm;
        private double _yOffset;
        private bool _rollExcess;
        private bool _pitchExcess;

        public double YOffset
        {
            get => _yOffset;
            set
            {
                _yOffset = value;
                OnPropertyChanged();
            }
        }

        public bool RollExcess
        {
            get => _rollExcess;
            set
            {
                _rollExcess = value;
                OnPropertyChanged();
            }
        }

        public bool PitchExcess
        {
            get => _pitchExcess;
            set
            {
                _pitchExcess = value;
                OnPropertyChanged();
            }
        }

        private double LineThickness => 3;


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawPitchScale();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            YOffset = ConvertPitchToYOffset(Pitch);
            DrawPitchScale();
        }

        private void DrawPitchScale()
        {
            var pitchHeightCenter = ThePitchScale.ActualHeight / 2;
            var pitchWidthCenter = ThePitchScale.ActualWidth / 2;
            double lineWidth = ThePitchScale.ActualWidth;

            int scaleStartFrom = -90;
            int scaleEndAt = 90;
            int scaleStep = 10;

            ThePitchScale.Children.Clear();
            for (int i = scaleStartFrom; i <= scaleEndAt; i += scaleStep)
            {
                Rectangle rectDozenLine = new Rectangle
                {
                    Fill = Brushes.Black, Height = LineThickness, Width = lineWidth
                };

                var yOffset = pitchHeightCenter + ConvertPitchToYOffset(i) + YOffset;

                Canvas.SetTop(rectDozenLine, yOffset - LineThickness / 2);
                ThePitchScale.Children.Add(rectDozenLine);

                if (i <= scaleStartFrom) continue;
                Rectangle rectHalfLine = new Rectangle
                {
                    Fill = Brushes.Black, Height = LineThickness, Width = lineWidth / 2
                };
                yOffset -= ConvertPitchToYOffset(scaleStep / 2.0);
                Canvas.SetTop(rectHalfLine, yOffset - LineThickness / 2);
                Canvas.SetLeft(rectHalfLine, lineWidth / 4); // to horizontal center
                ThePitchScale.Children.Add(rectHalfLine);
            }

            Rectangle centerLine = new Rectangle
            {
                Fill = Brushes.Black, Height = 1, Width = 3 * lineWidth
            };
            Canvas.SetTop(centerLine, pitchHeightCenter);
            Canvas.SetLeft(centerLine, pitchWidthCenter - centerLine.Width / 2);
            ThePitchScale.Children.Add(centerLine);
            ThePitchScale.InvalidateVisual(); // Force the canvas to refresh
        }


        #region Dependency Properties // TODO add coerce or validate and check other

        #region DependencyProperty Roll

        public static readonly DependencyProperty RollProperty = RollProperty = DependencyProperty.Register("Roll",
            typeof(double), typeof(GyroHorizonUI), new FrameworkPropertyMetadata(RollChangedCallback));

        private static void RollChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GyroHorizonUI gyro)) return;
            if (!double.TryParse(e.NewValue.ToString(), out double value)) return;

            // TODO extract to method
            if (Math.Abs(value) > gyro.ValidRoll && gyro.RollExcess == false)
            {
                gyro.RollExcess = true;
            }
            else if (gyro.RollExcess == true && Math.Abs(value) <= gyro.ValidRoll)
            {
                gyro.RollExcess = false;
            }
        }

        public double Roll
        {
            get => (double)GetValue(RollProperty);
            set => SetValue(RollProperty, value);
        }

        #endregion

        #region DependencyProperty Pitch

        public static readonly DependencyProperty PitchProperty = DependencyProperty.Register("Pitch", typeof(double),
            typeof(GyroHorizonUI),
            new FrameworkPropertyMetadata(PitchChangedCallback));

        private static void PitchChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GyroHorizonUI gyro)) return;
            if (!double.TryParse(e.NewValue.ToString(), out double value)) return;

            if (Math.Abs(value) > gyro.ValidPitch && gyro.PitchExcess == false)
            {
                gyro.PitchExcess = true;
            }
            else if (gyro.PitchExcess == true && Math.Abs(value) <= gyro.ValidPitch)
            {
                gyro.PitchExcess = false;
            }

            gyro.YOffset = gyro.ConvertPitchToYOffset(gyro.Pitch);

            gyro.DrawPitchScale();
        }

        private double ConvertPitchToYOffset(double pitch)
        {
            return ThePitchScale.ActualHeight / 180 * pitch; // from -90 to +90 = 180
        }

        public double Pitch
        {
            get => (double)GetValue(PitchProperty);
            set => SetValue(PitchProperty, value);
        }

        #endregion

        #region DependencyProperty Drift

        public static readonly DependencyProperty DriftProperty = DependencyProperty.Register("Drift", typeof(double),
            typeof(GyroHorizonUI),
            new FrameworkPropertyMetadata(0.0));

        public double Drift
        {
            get => (double)GetValue(DriftProperty);
            set => SetValue(DriftProperty, value);
        }

        #endregion

        #region DependencyProperty ValidRoll

        public static readonly DependencyProperty ValidRollProperty = DependencyProperty.Register("ValidRoll",
            typeof(double), typeof(GyroHorizonUI),
            new FrameworkPropertyMetadata());

        public double ValidRoll
        {
            get => (double)GetValue(ValidRollProperty);
            set => SetValue(ValidRollProperty, value);
        }

        #endregion

        #region DependencyProperty ValidPitch

        public static readonly DependencyProperty ValidPitchProperty = DependencyProperty.Register("ValidPitch",
            typeof(double), typeof(GyroHorizonUI),
            new FrameworkPropertyMetadata());

        public double ValidPitch
        {
            get => (double)GetValue(ValidPitchProperty);
            set => SetValue(ValidPitchProperty, value);
        }

        #endregion

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
