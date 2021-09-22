using System.ComponentModel;
using System.Runtime.CompilerServices;
using GyroHorizon.Annotations;

namespace GyroHorizon
{
    public class GyroHorizonViewModel : INotifyPropertyChanged
    {
        // TODO сделать тип для обмена данными между моделью и VM
        public double Roll;
        public double Pitch;
        public double Drift;
        public double ValidRoll;
        public double ValidPitch;

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
