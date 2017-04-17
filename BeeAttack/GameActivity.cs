using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BeeAttack
{
    [Activity(Label = "GameActivity", Theme = "@style/StartUpAppTheme", 
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class GameActivity : Activity
    {
        private ImageView _flower;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Game);

            // Create your application here
            _flower = FindViewById<ImageView>(Resource.Id.flowerView);
            _flower.Touch += _flower_Touch;
        }

        private void _flower_Touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Move)
            {
                _flower.TranslationX += e.Event.GetX();
            }
        }
    }
}