using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Task2;

namespace izci
{
    [Service]
    public class BaslatServis : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            baslat();
            return StartCommandResult.Sticky;
        }
        public void baslat()
        {       
            Intent start = new Intent(this, typeof(MainActivity));
            start.AddFlags(ActivityFlags.NewTask);
            StartActivity(start);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }      
    }
}