using Android.Telephony;

namespace SaveMe
{
    public class GsmSignalStrengthListener : PhoneStateListener
    {
        public delegate void SignalStrengthChangedDelegate(int strength, int level);

        public event SignalStrengthChangedDelegate SignalStrengthChanged;

        public override void OnSignalStrengthsChanged(SignalStrength newSignalStrength)
        {
            if (newSignalStrength.IsGsm)
            {
                SignalStrengthChanged?.Invoke(newSignalStrength.GsmSignalStrength, newSignalStrength.Level);
            }
        }
    }
}