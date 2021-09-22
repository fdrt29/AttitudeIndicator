using System.Windows;
using System.Windows.Controls;

namespace GyroHorizon
{
    public partial class GyroHorizonView : UserControl
    {
        public GyroHorizonView()
        {
            InitializeComponent();
            ViewModel = new GyroHorizonViewModel();
        }

        private GyroHorizonViewModel ViewModel
        {
            get => Root.DataContext as GyroHorizonViewModel;
            set => Root.DataContext = value;
        }


        #region Dependency Properties // TODO add coerce or validate and check other

        public static readonly DependencyProperty GyroHorizonDataProperty = DependencyProperty.Register(
            "GyroHorizonData",
            typeof(GyroHorizonData), typeof(GyroHorizonView));

        public GyroHorizonData GyroHorizonData
        {
            get => (GyroHorizonData)GetValue(RollProperty);
            set => SetValue(RollProperty, value);
        }

        #region DependencyProperty Roll

        public static readonly DependencyProperty RollProperty = RollProperty = DependencyProperty.Register("Roll",
            typeof(double), typeof(GyroHorizonView));

        public double Roll
        {
            get => (double)GetValue(RollProperty);
            set => SetValue(RollProperty, value);
        }

        #endregion

        #region DependencyProperty Pitch

        public static readonly DependencyProperty PitchProperty = DependencyProperty.Register("Pitch", typeof(double),
            typeof(GyroHorizonView),
            new FrameworkPropertyMetadata());

        public double Pitch
        {
            get => (double)GetValue(PitchProperty);
            set => SetValue(PitchProperty, value);
        }

        #endregion

        #region DependencyProperty Drift

        public static readonly DependencyProperty DriftProperty = DependencyProperty.Register("Drift", typeof(double),
            typeof(GyroHorizonView),
            new FrameworkPropertyMetadata());

        public double Drift
        {
            get => (double)GetValue(DriftProperty);
            set => SetValue(DriftProperty, value);
        }

        #endregion

        #region DependencyProperty ValidRoll

        public static readonly DependencyProperty ValidRollProperty = DependencyProperty.Register("ValidRoll",
            typeof(double), typeof(GyroHorizonView),
            new FrameworkPropertyMetadata());

        public double ValidRoll
        {
            get => (double)GetValue(ValidRollProperty);
            set => SetValue(ValidRollProperty, value);
        }

        #endregion

        #region DependencyProperty ValidPitch

        public static readonly DependencyProperty ValidPitchProperty = DependencyProperty.Register("ValidPitch",
            typeof(double), typeof(GyroHorizonView),
            new FrameworkPropertyMetadata());

        public double ValidPitch
        {
            get => (double)GetValue(ValidPitchProperty);
            set => SetValue(ValidPitchProperty, value);
        }

        #endregion

        #endregion
    }
}
