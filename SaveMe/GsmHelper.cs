using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;

namespace SaveMe
{
    public class GsmHelper
    {
        TelephonyManager _telephonyManager;
        GsmSignalStrengthListener _signalStrengthListener;
        TextView _gsmStrengthTextView;
        ImageView _gsmStrengthImageView;

        public GsmHelper(Context context, ImageView image, TextView text)
        {
            _gsmStrengthImageView = image;
            _gsmStrengthTextView = text;
            _telephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
        }

        public void Start()
        {
            _signalStrengthListener = new GsmSignalStrengthListener();
            _telephonyManager.Listen(_signalStrengthListener, PhoneStateListenerFlags.SignalStrengths);
            _signalStrengthListener.SignalStrengthChanged += HandleSignalStrengthChanged;
        }

        public void Stop()
        {
            _signalStrengthListener = new GsmSignalStrengthListener();
            _signalStrengthListener.SignalStrengthChanged -= HandleSignalStrengthChanged;
            _telephonyManager.Listen(_signalStrengthListener, PhoneStateListenerFlags.None);            
        }

        void HandleSignalStrengthChanged(int strength, int level)
        {
            //unhook everything, maybe add it in timer
            //_signalStrengthListener.SignalStrengthChanged -= HandleSignalStrengthChanged;
            //_telephonyManager.Listen(_signalStrengthListener, PhoneStateListenerFlags.None);

            //update the UI with text and an image.
            var temp = strength;
            if (strength == 99)
            {
                if (level < 1)
                    temp = 0;
                if (level >= 1 && level < 3)
                    temp = 1;
                if (level >= 3 && level < 5)
                    temp = 10;
                if (level >= 5)
                    temp = 20;
            }
            _gsmStrengthImageView.SetImageLevel(temp);
            _gsmStrengthTextView.Text = $"GSM Signal Strength ({temp}):";
        }
    }
}