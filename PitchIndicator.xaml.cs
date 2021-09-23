using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GyroHorizon
{
    public partial class PitchIndicator : UserControl
    {
        public PitchIndicator()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            TheScrollViewer.PreviewMouseWheel +=
                (object sender, MouseWheelEventArgs e) => e.Handled = true; // Disable mouse wheel for ScrollViewer
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            ScrollProportionallyPitch(Pitch);
        }

        public static readonly DependencyProperty ScaleIntermediateHeightProperty = DependencyProperty.Register(
            "ScaleIntermediateHeight", typeof(double), typeof(PitchIndicator), new PropertyMetadata(default(double)));

        public double ScaleIntermediateHeight
        {
            get { return (double)GetValue(ScaleIntermediateHeightProperty); }
            set { SetValue(ScaleIntermediateHeightProperty, value); }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TheScrollViewer.ScrollToVerticalOffset(TheScrollViewer.ScrollableHeight / 2);
            TheScrollViewer.UpdateLayout();
        }

        private void ScrollProportionallyPitch(double pitchValue)
        {
            // from - 90 to 90
            var maxValue = 90.0d;
            var pitchPercent = pitchValue / maxValue;

            var scrollableHeightHalf = TheScrollViewer.ScrollableHeight / 2;
            var scrollOffset = scrollableHeightHalf - pitchPercent * scrollableHeightHalf;
            TheScrollViewer.ScrollToVerticalOffset(scrollOffset);
            TheScrollViewer.UpdateLayout();
        }

        #region DependencyProperty Pitch

        public static readonly DependencyProperty PitchProperty = DependencyProperty.Register("Pitch", typeof(double),
            typeof(PitchIndicator),
            new FrameworkPropertyMetadata(PitchChangedCallback));

        private static void PitchChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PitchIndicator pitchIndicator)) return;

            pitchIndicator.ScrollProportionallyPitch((double)e.NewValue);
        }


        public double Pitch
        {
            get => (double)GetValue(PitchProperty);
            set => SetValue(PitchProperty, value);
        }

        #endregion
    }

    class HalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0.0;
            if (parameter == null) parameter = 1;

            if (double.TryParse(value.ToString(), out var number) &&
                double.TryParse(parameter.ToString(), out double coefficient)) //TODO clear
            {
                return number * 0.5;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0.0;
            if (parameter == null) parameter = 1;

            if (double.TryParse(value.ToString(), out var number) &&
                double.TryParse(parameter.ToString(), out double coefficient)) //TODO clear
            {
                return number * 2;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
