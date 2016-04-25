using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.Locations;
using Android.Net;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Telephony;
using Android.Util;
using Mono.Data.Sqlite;
using Environment = System.Environment;

namespace SaveMe
{
    [Activity(Label = "SaveMe", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : Activity, ILocationListener, ISensorEventListener
    {
        #region private fields

        private Context _context;

        static readonly object _syncLock = new object();

        private DoubleSensorHelper _sensorHelper;
        private GpsHelper _gpsHelper;
        private GsmHelper _gsmHelper;
        
        TextView _sensorTextViewAcc;
        TextView _sensorTextViewGir;
        TextView _locationText;
        TextView _addressText;
        TextView _gsmStrengthTextView;
        ImageView _gsmStrengthImageView;

        TextView _isOnline;
        TextView _roaming;
        TextView _wifi;
        TextView _connectionType;

        private string _lastDateTime;
        private OkFlag _dbok = new OkFlag() {Ok = false};
        #endregion

        #region Activity ovverride
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += delegate { Start(); };
            _sensorTextViewAcc = FindViewById<TextView>(Resource.Id.textViewAcc);
            _sensorTextViewGir = FindViewById<TextView>(Resource.Id.textViewGyr);
            _locationText = FindViewById<TextView>(Resource.Id.textViewGps);
            _addressText = FindViewById<TextView>(Resource.Id.textViewGpsAdress);
            _gsmStrengthTextView = FindViewById<TextView>(Resource.Id.textViewGsm);
            _gsmStrengthImageView = FindViewById<ImageView>(Resource.Id.imageView1);
            _wifi = FindViewById<TextView>(Resource.Id.textViewWiFi);
            _roaming = FindViewById<TextView>(Resource.Id.textViewRoaming);
            _isOnline = FindViewById<TextView>(Resource.Id.textViewIsOnline);
            _connectionType = FindViewById<TextView>(Resource.Id.textViewConnectionType);
            _context = FindViewById<Button>(Resource.Id.MyButton).Context;

            _sensorHelper = new DoubleSensorHelper(_context, this);
            _gsmHelper = new GsmHelper(_context, _gsmStrengthImageView, _gsmStrengthTextView);
            _gpsHelper = new GpsHelper(_context, _locationText, _addressText, this);

            AdoFunctionsHelper.CreateDatabase(_context);
            NetworkHelper.DetectNetwork(_context, _isOnline, _connectionType, _wifi, _roaming);
        }

        protected override void OnResume()
        {
            base.OnResume();

            //NetworkHelper.DetectNetwork(_context, _isOnline, _connectionType, _wifi, _roaming);
            //_sensorHelper.Resume();
        }

        protected override void OnPause()
        {
            base.OnPause();

            //_sensorHelper.Pause();
        }
        #endregion

        private void Start()
        {            
            _sensorHelper.Start();
            _gpsHelper.InitializeLocationManager();            
            _gsmHelper.Start();            
            //TODO stop if started (toggle)
        }

        #region ISensorEventListener
        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            // We don't want to do anything here.
        }

        public async void OnSensorChanged(SensorEvent e)
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

            if (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") != _lastDateTime)
            {
                var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _lastDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                await AdoFunctionsHelper.InsertIntoDb(_sensorTextViewAcc.Text, time, "ACC", _dbok);
                await AdoFunctionsHelper.InsertIntoDb(_sensorTextViewGir.Text, time, "GYR", _dbok);
                await AdoFunctionsHelper.InsertIntoDb(_locationText.Text, time, "GPS", _dbok);               
            }
        }
        #endregion

        #region ILocationListener
        public void OnLocationChanged(Location location)
        {
            ;
        }

        public void OnProviderDisabled(string provider)
        {
            ;
        }

        public void OnProviderEnabled(string provider)
        {
            ;
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            ;
        }
        #endregion        
    }
}

