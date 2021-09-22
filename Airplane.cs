namespace GyroHorizon
{
    public struct RotationData
    {
        public RotationData(double roll, double pitch, double yaw)
        {
            Roll = roll;
            Pitch = pitch;
            Yaw = yaw;
        }

        public double Roll;
        public double Pitch;
        public double Yaw;
    }

    public class Airplane
    {
        public RotationData Rotation;

        public double Drift;
        public double ValidRoll;
        public double ValidPitch;

        // Some other properties
    }
}
