using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SaveMe
{
    public static class NetworkHelper
    {
        public static void DetectNetwork(Context context, TextView _isOnline, TextView _connectionType, TextView _wifi, TextView _roaming)
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;

            bool isOnline = (activeConnection != null) && activeConnection.IsConnected;

            if (isOnline)
            {
                _isOnline.Text = "Yes";

                NetworkInfo.State activeState = activeConnection.GetState();
                _connectionType.Text = activeConnection.TypeName;

                NetworkInfo wifiInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi);
                _wifi.Text = wifiInfo.IsConnected ? "Yes" : "No";

                NetworkInfo mobileInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Mobile);
                _roaming.Text = mobileInfo.IsRoaming && mobileInfo.IsConnected ? "Yes" : "No";
            }
            else
            {
                _connectionType.Text = "N/A";
                _wifi.Text = "No";
                _roaming.Text = "No";
                _isOnline.Text = "No";
            }
        }
    }
}