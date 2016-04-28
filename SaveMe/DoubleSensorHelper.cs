using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SaveMe
{
    public class DoubleSensorHelper
    {
        private MainActivity activity;
        private Context _context;
        SensorManager _sensorManagerAcc;
        SensorManager _sensorManagerGir;

        public DoubleSensorHelper(Context _context, MainActivity activity)
        {
            this._context = _context;
            this.activity = activity;
        }

        public void Start()
        {
            if (_sensorManagerAcc == null && _sensorManagerGir == null)
            {
                _sensorManagerAcc = (SensorManager)_context.GetSystemService(Context.SensorService);
                _sensorManagerGir = (SensorManager)_context.GetSystemService(Context.SensorService);
                if (_sensorManagerAcc != null && _sensorManagerGir != null)
                {
                    _sensorManagerAcc.RegisterListener(activity, _sensorManagerAcc.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Fastest);
                    _sensorManagerGir.RegisterListener(activity, _sensorManagerGir.GetDefaultSensor(SensorType.Gyroscope), SensorDelay.Fastest);
                }
            }
            else
            {
                this.Resume();
            }
        }

        public void Resume()
        {
            if (_sensorManagerAcc != null && _sensorManagerGir != null)
            {
                _sensorManagerAcc.RegisterListener(activity, _sensorManagerAcc.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Fastest);
                _sensorManagerGir.RegisterListener(activity, _sensorManagerGir.GetDefaultSensor(SensorType.Gyroscope), SensorDelay.Fastest);
            }
        }

        public void Pause()
        {
            if (_sensorManagerAcc != null && _sensorManagerGir != null)
            {
                _sensorManagerAcc.UnregisterListener(activity);
                _sensorManagerGir.UnregisterListener(activity);
            }
        }
    }
}