using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace BeeAttack
{
    [Activity(Label = "Bee Attack", MainLauncher = true, Icon = "@drawable/bee")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var absGameArea = FindViewById<AbsoluteLayout>(Resource.Id.absGameArea);
            var linStart = FindViewById<LinearLayout>(Resource.Id.linStart);
            //linStart.SetX(200);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.button1);
            
            button.Click += delegate 
            {
                var dialog = new AlertDialog.Builder(this);
                dialog.SetMessage("// TODO");
                dialog.SetPositiveButton("OK", delegate { });
                dialog.Show();
            };
        }
    }
}

