using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using GyroHorizon.Annotations;

namespace GyroHorizon
{
    public delegate void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);

    public partial class GyroHorizonUI : UserControl, INotifyPropertyChanged
    {
        public GyroHorizonUI()
        {
            InitializeComponent();
            _vm = new GyroHorizonVM(this);
            SizeChanged += (sender, args) => YOffset = ConvertPitchToYOffset(Pitch);
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


        #region Dependency Properties // TODO add coerce or validate and check other

        #region DependencyProperty Roll

        public static readonly DependencyProperty RollProperty = RollProperty = DependencyProperty.Register("Roll",
            typeof(double), typeof(GyroHorizonUI), new FrameworkPropertyMetadata(RollChangedCallback));

        private static void RollChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //TODO value if > ValidRoll call some
            if (!(d is GyroHorizonUI gyroHorizonUi)) return;
            if (!double.TryParse(e.NewValue.ToString(), out double value)) return;

            if (Math.Abs(value) > gyroHorizonUi.ValidRoll && gyroHorizonUi.RollExcess == false)
            {
                gyroHorizonUi.RollExcess = true;
            }
            else if (gyroHorizonUi.RollExcess == true && Math.Abs(value) <= gyroHorizonUi.ValidRoll)
            {
                gyroHorizonUi.RollExcess = false;
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
            if (!(d is GyroHorizonUI gyroHorizonUi)) return;
            if (!double.TryParse(e.NewValue.ToString(), out double value)) return;

            if (Math.Abs(value) > gyroHorizonUi.ValidPitch && gyroHorizonUi.PitchExcess == false)
            {
                gyroHorizonUi.PitchExcess = true;
            }
            else if (gyroHorizonUi.PitchExcess == true && Math.Abs(value) <= gyroHorizonUi.ValidPitch)
            {
                gyroHorizonUi.PitchExcess = false;
            }

            gyroHorizonUi.YOffset = gyroHorizonUi.ConvertPitchToYOffset(gyroHorizonUi.Pitch);
        }

        private double ConvertPitchToYOffset(double pitch)
        {
            return ThePitchScale.ActualHeight / 180 * pitch;
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
