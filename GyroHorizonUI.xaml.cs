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
        private SolidColorBrush _marksColor = Brushes.Black;

        public SolidColorBrush MarksColor
        {
            get => _marksColor;
            private set
            {
                if (Equals(value, _marksColor)) return;
                _marksColor = value;
                OnPropertyChanged();
            }
        }


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
            private set
            {
                _pitchExcess = value;
                OnPropertyChanged();
            }
        }

        private double LineThickness => 3;


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawPitchScale();
            DrawRollScale();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            YOffset = ConvertPitchToYOffset(Pitch);
            DrawPitchScale();
            DrawRollScale();
        }

        private void DrawBackground()
        {
            var pitchHeightCenter = ThePitchScale.ActualHeight / 2;
            var pitchWidthCenter = ThePitchScale.ActualWidth / 2;

            // Draw Background
            var skyRectHeight = 2 * pitchHeightCenter;
            Rectangle skyRect = new Rectangle
            {
                Fill = Brushes.SkyBlue, Height = skyRectHeight, Width = ActualWidth
            };
            Canvas.SetTop(skyRect, -skyRectHeight / 2 + YOffset);
            Canvas.SetLeft(skyRect, pitchWidthCenter - ActualWidth / 2);

            Rectangle groundRect = new Rectangle
            {
                Fill = Brushes.SaddleBrown, Height = skyRectHeight, Width = ActualWidth
            };
            Canvas.SetTop(groundRect, skyRectHeight / 2 + YOffset);
            Canvas.SetLeft(groundRect, pitchWidthCenter - ActualWidth / 2);

            ThePitchScale.Children.Add(skyRect);
            ThePitchScale.Children.Add(groundRect);
        }

        private void DrawPitchScale()
        {
            ThePitchScale.Children.Clear();
            // TODO doesnt clear but iterate and update

            var scaleHeightCenter = ThePitchScale.ActualHeight / 2;
            var scaleWidthCenter = ThePitchScale.ActualWidth / 2;
            double lineWidth = ThePitchScale.ActualWidth; // From XAML

            int scaleStartAt = -90;
            int scaleEndAt = 90;
            int scaleStep = 10;
            int marksQuantity = (scaleEndAt - scaleStartAt) / scaleStep + 1;

            DrawBackground();

            double textXOffset = lineWidth;
            // Distance between scale marks * coefficient
            int textSize = Convert.ToInt32(ThePitchScale.ActualHeight / marksQuantity * 0.6);
            textSize = textSize == 0 ? 12 : textSize; // if ActualHeight == 0 (on init) set some default size


            // Draw Scale Marking
            for (int i = scaleStartAt; i <= scaleEndAt; i += scaleStep)
            {
                Rectangle rectDozenLine = new Rectangle
                {
                    Fill = MarksColor, Height = LineThickness, Width = lineWidth
                };

                var yOffset = scaleHeightCenter + ConvertPitchToYOffset(i) + YOffset;

                Canvas.SetTop(rectDozenLine, yOffset - LineThickness / 2);
                ThePitchScale.Children.Add(rectDozenLine);

                // Numbering of marks
                TextBlock textR = new TextBlock
                {
                    // Canvas doesn't support TextAlignment -> space or minus
                    Text = -i < 0 ? (-i).ToString() : " " + (-i), // and inverting i
                    Foreground = MarksColor,
                    FontSize = textSize,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                };

                Canvas.SetTop(textR, yOffset - textSize);
                Canvas.SetLeft(textR, scaleWidthCenter + textXOffset);
                ThePitchScale.Children.Add(textR);


                if (i <= scaleStartAt) continue;
                Rectangle rectHalfLine = new Rectangle
                {
                    Fill = MarksColor, Height = LineThickness, Width = lineWidth / 2
                };
                yOffset -= ConvertPitchToYOffset(scaleStep / 2.0);
                Canvas.SetTop(rectHalfLine, yOffset - LineThickness / 2);
                Canvas.SetLeft(rectHalfLine, lineWidth / 4); // to horizontal center
                ThePitchScale.Children.Add(rectHalfLine);
            }

            Rectangle centerLine = new Rectangle
            {
                Fill = MarksColor, Height = 1, Width = 3 * lineWidth
            };
            Canvas.SetTop(centerLine, scaleHeightCenter);
            Canvas.SetLeft(centerLine, scaleWidthCenter - centerLine.Width / 2);
            ThePitchScale.Children.Add(centerLine);

            ThePitchScale.InvalidateVisual(); // Force the canvas to refresh
        }

        void DrawRollScale()
        {
            TheRollScale.Children.Clear();
            // TODO doesnt clear but iterate and update

            var scaleCenter = new Point(TheRollScale.ActualWidth / 2, TheRollScale.ActualHeight / 2);
            var radius = Math.Max(TheRollScale.ActualWidth, TheRollScale.ActualHeight) / 4;

            int scaleStartAt = -90;
            int scaleEndAt = 90;
            int scaleStep = 10;

            Point startPoint = new Point(scaleCenter.X - radius, scaleCenter.Y);
            Point endPoint = new Point(scaleCenter.X + radius, scaleCenter.Y);
            // Semicircle
            ArcSegment arcSegment = new ArcSegment(endPoint, new Size(radius, radius), 0, false,
                SweepDirection.Clockwise, true);
            PathGeometry geometry =
                new PathGeometry(new[] { new PathFigure(startPoint, new[] { arcSegment }, false) });
            Path path = new Path
            {
                Data = geometry,
                Stroke = MarksColor,
                StrokeThickness = LineThickness,
            };
            TheRollScale.Children.Add(path);

            var textSize = Math.PI * radius / 18 / 2;
            textSize = textSize == 0 ? 12 : textSize; // if ActualHeight == 0 (on init) set some default size

            // Arc length
            double lineLength = radius / 6.0;
            // Draw Scale Marking
            for (int i = scaleStartAt; i <= scaleEndAt; i += scaleStep)
            {
                double angle = i - 90.0; // Rotate
                angle = Math.PI * angle / 180.0; // To radians

                var xCoord = Math.Cos(angle) * radius;
                var yCoord = Math.Sin(angle) * radius;
                var xCoordEnd = Math.Cos(angle) * (radius - lineLength);
                var yCoordEnd = Math.Sin(angle) * (radius - lineLength);

                Line line = new Line
                {
                    X1 = xCoord, Y1 = yCoord, X2 = xCoordEnd, Y2 = yCoordEnd,
                    StrokeThickness = LineThickness, Stroke = MarksColor,
                };
                Canvas.SetTop(line, scaleCenter.Y);
                Canvas.SetLeft(line, scaleCenter.X);
                TheRollScale.Children.Add(line);


                xCoordEnd = Math.Cos(angle) * (radius + lineLength / 2);
                yCoordEnd = Math.Sin(angle) * (radius + lineLength / 2);
                TextBlock text = new TextBlock
                {
                    Text = i < 0 ? i.ToString() : " " + i, // minus or space
                    FontSize = textSize,
                };
                Canvas.SetTop(text, scaleCenter.Y + yCoordEnd - textSize);
                Canvas.SetLeft(text, scaleCenter.X + xCoordEnd - textSize);
                TheRollScale.Children.Add(text);
            }

            TheRollScale.InvalidateVisual();
        }


        #region Dependency Properties

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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
