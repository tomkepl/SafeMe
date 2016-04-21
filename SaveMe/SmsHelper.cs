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
    public static class SmsHelper
    {
        public static void SendSms(string number, string content, bool intent = false, Context context = null)
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
                context?.StartActivity(smsIntent);
            }
        }
    }
}