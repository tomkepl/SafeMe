using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Locations;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Telephony;

namespace SaveMe
{
    [Activity(Label = "SaveMe", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISensorEventListener, ILocationListener
    {
        static readonly object _syncLock = new object();
        Location _currentLocation;
        LocationManager _locationManager;
        string _locationProvider;

        SensorManager _sensorManagerAcc;
        SensorManager _sensorManagerGir;

        TextView _sensorTextViewAcc;
        TextView _sensorTextViewGir;
        TextView _locationText;
        TextView _addressText;

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
            _locationText = FindViewById<TextView>(Resource.Id.textView6);
            _addressText = FindViewById<TextView>(Resource.Id.textView7);
        }

        private void StartSensors()
        {
            if (_sensorManagerAcc == null && _sensorManagerGir == null)
            {
                _sensorManagerAcc = (SensorManager) GetSystemService(SensorService);
                _sensorManagerGir = (SensorManager) GetSystemService(SensorService);
                if (_sensorManagerAcc != null && _sensorManagerGir != null)
                {
                    _sensorManagerAcc.RegisterListener(this,
                        _sensorManagerAcc.GetDefaultSensor(SensorType.Accelerometer),
                        SensorDelay.Fastest);
                    _sensorManagerGir.RegisterListener(this, _sensorManagerGir.GetDefaultSensor(SensorType.Gyroscope),
                        SensorDelay.Fastest);
                }
            }

            InitializeLocationManager();
            _locationManager?.RequestLocationUpdates(_locationProvider, 0, 0, this);
            //TODO stop if started (toggle)
        }

        void InitializeLocationManager()
        {
            //TODO zrobic poziom sygnalu i czy jest juz ustalona pozycja
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            _locationProvider = acceptableLocationProviders.Any() ? acceptableLocationProviders.First() : string.Empty;
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
            if (_sensorManagerAcc != null && _sensorManagerGir != null && _locationManager != null)
            {
                //TODO sprawdzac czy dzialaja na wzbudzeniu, jesli nie obudzic
                _sensorManagerAcc.RegisterListener(this, _sensorManagerAcc.GetDefaultSensor(SensorType.Accelerometer),
                    SensorDelay.Fastest);
                _sensorManagerGir.RegisterListener(this, _sensorManagerGir.GetDefaultSensor(SensorType.Gyroscope),
                    SensorDelay.Fastest);

                _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (_sensorManagerAcc != null && _sensorManagerGir != null && _locationManager != null)
            {
                //TODO to chyba spowoduje w uspieniu brak dzialania to teoretycznie do usuniecia
                _sensorManagerAcc.UnregisterListener(this);
                _sensorManagerGir.UnregisterListener(this);
                _locationManager.RemoveUpdates(this);
            }
        }

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                // Remove the last comma from the end of the address.
                _addressText.Text = deviceAddress.ToString();                
            }
            else
            {
                _addressText.Text = "Unable to determine the address. Try again in a few minutes.";
            }
        }

        public async void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _locationText.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
                _locationText.Text = $"{_currentLocation.Latitude:f6},{_currentLocation.Longitude:f6}";
                Address address = await ReverseGeocodeCurrentLocation();
                DisplayAddress(address);                
            }
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

        private void SendSms(string number, string content, bool intent = false)
        {
            if (intent == false)
            {
                SmsManager.Default.SendTextMessage(number, null, content, null, null);
            }
            else
            {
                var smsUri = Android.Net.Uri.Parse("smsto:" + number);
                var smsIntent = new Intent(Intent.ActionSendto, smsUri);
                smsIntent.PutExtra("sms_body", content);
                StartActivity(smsIntent);
            }
        }
    }
}

