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
        private double _driftXOffset;
        private SolidColorBrush _marksColor = Brushes.Black;
        private bool _pitchExcess;
        private double _pitchYOffset;
        private bool _rollExcess;
        private double _rollScaleRadius;

        public GyroHorizonUI()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;
        }


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

        public double PitchYOffset
        {
            get => _pitchYOffset;
            set
            {
                if (value.Equals(_pitchYOffset)) return;
                _pitchYOffset = value;
                OnPropertyChanged();
            }
        }

        public double DriftXOffset
        {
            get => _driftXOffset;
            set
            {
                if (value.Equals(_driftXOffset)) return;
                _driftXOffset = value;
                OnPropertyChanged();
            }
        }

        public bool RollExcess
        {
            get => _rollExcess;
            set
            {
                if (value.Equals(_rollExcess)) return;
                _rollExcess = value;
                OnPropertyChanged();
            }
        }

        public bool PitchExcess
        {
            get => _pitchExcess;
            private set
            {
                if (value.Equals(_pitchExcess)) return;
                _pitchExcess = value;
                OnPropertyChanged();
            }
        }

        public double RollScaleRadius
        {
            get => _rollScaleRadius;
            set
            {
                if (value.Equals(_rollScaleRadius)) return;
                _rollScaleRadius = value;
                OnPropertyChanged();
            }
        }

        private double LineThickness => 3;


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawPitchScale();
            DrawRollScale();
            DrawRollIndicator();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            PitchYOffset = ConvertPitchToYOffset(Pitch);
            DriftXOffset = ConvertDriftToXOffset(Drift);
            DrawPitchScale();
            DrawRollScale();
            DrawRollIndicator();
        }

        private void DrawBackground()
        {
            var pitchHeightCenter = ThePitchScale.ActualHeight / 2;
            var pitchWidthCenter = ThePitchScale.ActualWidth / 2;

            // Draw Background
            var skyRectHeight = 2 * pitchHeightCenter;
            var skyRect = new Rectangle
            {
                Fill = Brushes.SkyBlue, Height = skyRectHeight, Width = ActualWidth
            };
            Canvas.SetTop(skyRect, -skyRectHeight / 2 + PitchYOffset);
            Canvas.SetLeft(skyRect, pitchWidthCenter - ActualWidth / 2);

            var groundRect = new Rectangle
            {
                Fill = Brushes.SaddleBrown, Height = skyRectHeight, Width = ActualWidth
            };
            Canvas.SetTop(groundRect, skyRectHeight / 2 + PitchYOffset);
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
            var lineWidth = ThePitchScale.ActualWidth; // From XAML

            var scaleStartAt = -90;
            var scaleEndAt = 90;
            var scaleStep = 10;
            var marksQuantity = (scaleEndAt - scaleStartAt) / scaleStep + 1;

            DrawBackground();

            var textXOffset = lineWidth;
            // Distance between scale marks * coefficient
            var textSize = Convert.ToInt32(ThePitchScale.ActualHeight / marksQuantity * 0.5);
            textSize = textSize == 0 ? 12 : textSize; // if ActualHeight == 0 (on init) set some default size


            // Draw Scale Marking
            for (var i = scaleStartAt; i <= scaleEndAt; i += scaleStep)
            {
                var rectDozenLine = new Rectangle
                {
                    Fill = MarksColor, Height = LineThickness, Width = lineWidth
                };

                var yOffset = scaleHeightCenter + ConvertPitchToYOffset(i) + PitchYOffset;

                Canvas.SetTop(rectDozenLine, yOffset - LineThickness / 2);
                ThePitchScale.Children.Add(rectDozenLine);

                // Numbering of marks
                var textR = new TextBlock
                {
                    // Canvas doesn't support TextAlignment -> space or minus
                    Text = -i < 0 ? (-i).ToString() : " " + -i, // and inverting i
                    Foreground = MarksColor,
                    FontSize = textSize,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                };

                Canvas.SetTop(textR, yOffset - textSize);
                Canvas.SetLeft(textR, scaleWidthCenter + textXOffset);
                ThePitchScale.Children.Add(textR);


                if (i <= scaleStartAt) continue;
                var rectHalfLine = new Rectangle
                {
                    Fill = MarksColor, Height = LineThickness, Width = lineWidth / 2
                };
                yOffset -= ConvertPitchToYOffset(scaleStep / 2.0);
                Canvas.SetTop(rectHalfLine, yOffset - LineThickness / 2);
                Canvas.SetLeft(rectHalfLine, lineWidth / 4); // to horizontal center
                ThePitchScale.Children.Add(rectHalfLine);
            }

            var centerLine = new Rectangle
            {
                Fill = MarksColor, Height = 1, Width = 3 * lineWidth
            };
            Canvas.SetTop(centerLine, scaleHeightCenter);
            Canvas.SetLeft(centerLine, scaleWidthCenter - centerLine.Width / 2);
            ThePitchScale.Children.Add(centerLine);

            ThePitchScale.InvalidateVisual(); // Force the canvas to refresh
        }

        private void DrawRollScale()
        {
            TheRollScale.Children.Clear();
            // TODO doesnt clear but iterate and update

            var scaleCenter = new Point(TheRollScale.ActualWidth / 2, TheRollScale.ActualHeight / 2);
            var radius = Math.Min(TheRollScale.ActualWidth, TheRollScale.ActualHeight) / 3;
            RollScaleRadius = radius;

            var scaleStartAt = -90;
            var scaleEndAt = 90;
            var scaleStep = 10;

            var startPoint = new Point(scaleCenter.X, scaleCenter.Y - radius);
            var endPoint = new Point(scaleCenter.X, scaleCenter.Y + radius);
            // Semicircle
            var arcSegment = new ArcSegment(endPoint, new Size(radius, radius), 0, false,
                SweepDirection.Counterclockwise, true);
            var geometry =
                new PathGeometry(new[] { new PathFigure(startPoint, new[] { arcSegment }, false) });
            var path = new Path
            {
                Data = geometry,
                Stroke = MarksColor,
                StrokeThickness = LineThickness
            };
            TheRollScale.Children.Add(path);

            // Arc length
            var textSize = Math.PI * radius / 18 * 0.7;
            textSize = textSize == 0 ? 12 : textSize; // if ActualHeight == 0 (on init) set some default size

            var lineLength = radius / 6.0;
            // Draw Scale Marking
            for (var i = scaleStartAt; i <= scaleEndAt; i += scaleStep)
            {
                var angle = i + 180.0; // Rotate
                angle = Math.PI * angle / 180.0; // To radians

                var xCoord = Math.Cos(angle) * radius;
                var yCoord = Math.Sin(angle) * radius;
                var xCoordEnd = Math.Cos(angle) * (radius - lineLength);
                var yCoordEnd = Math.Sin(angle) * (radius - lineLength);

                var line = new Line
                {
                    X1 = xCoord, Y1 = yCoord, X2 = xCoordEnd, Y2 = yCoordEnd,
                    StrokeThickness = LineThickness, Stroke = MarksColor
                };
                Canvas.SetTop(line, scaleCenter.Y);
                Canvas.SetLeft(line, scaleCenter.X);
                TheRollScale.Children.Add(line);

                // Text
                xCoordEnd = Math.Cos(angle) * (radius + lineLength / 2);
                yCoordEnd = Math.Sin(angle) * (radius + lineLength / 2);
                var text = new TextBlock
                {
                    Text = i < 0 ? i.ToString() : " " + i, // minus or space
                    FontSize = textSize
                };
                Canvas.SetTop(text, scaleCenter.Y + yCoordEnd - textSize / 1.5);
                Canvas.SetLeft(text, scaleCenter.X + xCoordEnd - textSize);
                TheRollScale.Children.Add(text);
            }

            TheRollScale.InvalidateVisual();
        }

        private void DrawRollIndicator()
        {
            TheRollIndicatorCanvas.Children.Clear();
            var center = RollScaleRadius;

            var smallCircleRadius = 0.2 * RollScaleRadius;

            var rollIndicatorColor = Brushes.Gold;
            var rollIndicatorThickness = LineThickness * 1.5;

            var line = new Line
            {
                X1 = 0, Y1 = center, X2 = center - smallCircleRadius + rollIndicatorThickness / 2, Y2 = center,
                Stroke = rollIndicatorColor, StrokeThickness = rollIndicatorThickness
            };
            TheRollIndicatorCanvas.Children.Add(line);

            var startPoint = new Point(center - smallCircleRadius, center);
            var endPoint = new Point(center + smallCircleRadius, center);

            var arcSegment = new ArcSegment(endPoint, new Size(smallCircleRadius, smallCircleRadius), 0, false,
                SweepDirection.Counterclockwise, true);
            var geometry =
                new PathGeometry(new[] { new PathFigure(startPoint, new[] { arcSegment }, false) });
            var path = new Path
            {
                Data = geometry,
                Stroke = rollIndicatorColor,
                StrokeThickness = rollIndicatorThickness
            };
            TheRollIndicatorCanvas.Children.Add(path);

            line = new Line
            {
                X1 = center + smallCircleRadius - rollIndicatorThickness / 2, Y1 = center, X2 = center * 2, Y2 = center,
                Stroke = rollIndicatorColor, StrokeThickness = rollIndicatorThickness
            };
            TheRollIndicatorCanvas.Children.Add(line);

            TheRollIndicatorCanvas.InvalidateVisual();
        }


        #region Dependency Properties

        #region DependencyProperty Roll

        public static readonly DependencyProperty RollProperty = RollProperty = DependencyProperty.Register("Roll",
            typeof(double), typeof(GyroHorizonUI),
            new FrameworkPropertyMetadata(RollChangedCallback, CoerceRollCallback));

        private static object CoerceRollCallback(DependencyObject d, object basevalue)
        {
            if (!double.TryParse(basevalue.ToString(), out var value)) return 0.0;
            return Math.Max(-90.0, Math.Min(90.0, value));
        }

        private static void RollChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GyroHorizonUI gyro)) return;
            if (!double.TryParse(e.NewValue.ToString(), out var value)) return;

            // TODO extract to method
            if (Math.Abs(value) > gyro.ValidRoll && gyro.RollExcess == false)
                gyro.RollExcess = true;
            else if (gyro.RollExcess && Math.Abs(value) <= gyro.ValidRoll) gyro.RollExcess = false;
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
            new FrameworkPropertyMetadata(PitchChangedCallback, CoercePitchCallback));

        private static object CoercePitchCallback(DependencyObject d, object basevalue)
        {
            if (!double.TryParse(basevalue.ToString(), out var value)) return 0.0;
            return Math.Max(-90, Math.Min(90, value));
        }

        private static void PitchChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GyroHorizonUI gyro)) return;
            if (!double.TryParse(e.NewValue.ToString(), out var value)) return;

            if (Math.Abs(value) > gyro.ValidPitch && gyro.PitchExcess == false)
                gyro.PitchExcess = true;
            else if (gyro.PitchExcess && Math.Abs(value) <= gyro.ValidPitch) gyro.PitchExcess = false;

            gyro.PitchYOffset = gyro.ConvertPitchToYOffset(value);

            gyro.DrawPitchScale(); // Redraw pitch scale
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
            new FrameworkPropertyMetadata(DriftChangedCallback, CoerceValueCallback));

        private static object CoerceValueCallback(DependencyObject d, object basevalue)
        {
            if (!double.TryParse(basevalue.ToString(), out var value)) return 0.0;
            return Math.Max(-1, Math.Min(1, value));
        }

        private static void DriftChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GyroHorizonUI gyro)) return;
            if (!double.TryParse(e.NewValue.ToString(), out var value)) return;

            gyro.DriftXOffset = gyro.ConvertDriftToXOffset(value);
        }

        private double ConvertDriftToXOffset(double drift)
        {
            var centerX = TheDriftScale.ActualWidth / 2;
            return drift * (centerX - TheDriftIndicator.ActualWidth / 2);
        }

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
