using System;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SaveMe
{
    [Activity(Label = "SaveMe", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISensorEventListener
    {
        static readonly object _syncLock = new object();
        SensorManager _sensorManagerAcc;
        SensorManager _sensorManagerGir;
        TextView _sensorTextViewAcc;
        TextView _sensorTextViewGir;        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += delegate { StartSensors(); };
            _sensorTextViewAcc = FindViewById<TextView>(Resource.Id.textView2);
            _sensorTextViewGir = FindViewById<TextView>(Resource.Id.textView4);
        }

        private void StartSensors()
        {
            _sensorManagerAcc = (SensorManager)GetSystemService(SensorService);
            _sensorManagerGir = (SensorManager)GetSystemService(SensorService);
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            // We don't want to do anything here.
        }

        public void OnSensorChanged(SensorEvent e)
        {
            lock (_syncLock)
            {
                if (e.Sensor.Type == SensorType.Accelerometer)
                {
                    _sensorTextViewAcc.Text = $"x={e.Values[0]:f}, y={e.Values[1]:f}, y={e.Values[2]:f}";
                }
                if (e.Sensor.Type == SensorType.Gyroscope)
                {
                    _sensorTextViewGir.Text = $"x={e.Values[0]:f}, y={e.Values[1]:f}, y={e.Values[2]:f}";
                }

            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            _sensorManagerAcc.RegisterListener(this, _sensorManagerAcc.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Fastest);
            _sensorManagerGir.RegisterListener(this, _sensorManagerGir.GetDefaultSensor(SensorType.Gyroscope), SensorDelay.Fastest);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _sensorManagerAcc.UnregisterListener(this);
            _sensorManagerGir.UnregisterListener(this);
        }
    }
}

