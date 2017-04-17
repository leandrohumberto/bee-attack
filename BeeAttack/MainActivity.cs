using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace BeeAttack
{
    [Activity(Label = "Bee Attack", MainLauncher = true, Icon = "@drawable/bee", 
        Theme = "@style/StartUpAppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button button = FindViewById<Button>(Resource.Id.button1);
            
            button.Click += delegate 
            {
                var intent = new Intent(this, typeof(GameActivity));
                StartActivity(intent);
            };
        }
    }
}

