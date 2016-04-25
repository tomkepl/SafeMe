using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SaveMe
{
    public class GpsHelper
    {
        string _locationProvider;
        Location _currentLocation;
        LocationManager _locationManager;

        private TextView _addressText;
        private Context _context;
        private TextView _locationText;
        private MainActivity _activity;

        public GpsHelper(Context _context, TextView _locationText, TextView _addressText, MainActivity activity)
        {
            this._context = _context;
            this._locationText = _locationText;
            this._addressText = _addressText;
            this._activity = activity;
        }

        public void InitializeLocationManager()
        {
            //TODO zrobic poziom sygnalu i czy jest juz ustalona pozycja
            _locationManager = (LocationManager)_context.GetSystemService(Context.LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            _locationProvider = acceptableLocationProviders.Any() ? acceptableLocationProviders.First() : string.Empty;

            _locationManager?.RequestLocationUpdates(_locationProvider, 0, 0, _activity);
        }

        public void DisableLocationManager()
        {
            _locationManager.RemoveUpdates(_activity);
        }

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(_context);
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
    }
}