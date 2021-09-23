using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GyroHorizon.Annotations;

namespace GyroHorizon
{
    public partial class PitchIndicator : UserControl, INotifyPropertyChanged
    {
        public PitchIndicator()
        {
            InitializeComponent();
        }


        private double _yOffset = 0;

        public double YOffset
        {
            get => _yOffset;
            set
            {
                _yOffset = value;
                OnPropertyChanged();
            }
        }


        #region DependencyProperty Pitch

        public static readonly DependencyProperty PitchProperty = DependencyProperty.Register("Pitch", typeof(double),
            typeof(PitchIndicator),
            new FrameworkPropertyMetadata(PitchChangedCallback, CoerceValueCallback));

        private static object CoerceValueCallback(DependencyObject d, object basevalue)
        {
            double minValue = -90;
            double maxValue = 90;
            return Math.Max(minValue, Math.Min(maxValue, (double)basevalue));
        }

        private static void PitchChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PitchIndicator pitchIndicator)) return;
            // -90 to 90 = 180
            // pitchIndicator.YOffset = pitchIndicator.ThePitchScale.ActualHeight / 180 * (double)e.NewValue;
        }


        public double Pitch
        {
            get => (double)GetValue(PitchProperty);
            set => SetValue(PitchProperty, value);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
