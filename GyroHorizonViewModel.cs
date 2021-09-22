using System.ComponentModel;
using System.Runtime.CompilerServices;
using GyroHorizon.Annotations;

namespace GyroHorizon
{
    public class GyroHorizonData
    {
        public double Roll;
        public double Pitch;
        public double Drift;
        public double ValidRoll;
        public double ValidPitch;

        public GyroHorizonData(double roll, double pitch, double drift, double validRoll, double validPitch)
        {
            Roll = roll;
            Pitch = pitch;
            Drift = drift;
            ValidRoll = validRoll;
            ValidPitch = validPitch;
        }
    }

    public class GyroHorizonViewModel : INotifyPropertyChanged
    {
        // TODO сделать тип для обмена данными между моделью и VM


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
