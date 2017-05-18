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
        public ImageView Flower;
        public ImageView Hive;
        public RelativeLayout GameArea;
        private Services.BeeAtackService _service;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Game);

            // Create your application here
            Flower = FindViewById<ImageView>(Resource.Id.flowerView);
            Hive = FindViewById<ImageView>(Resource.Id.hiveView);
            GameArea = FindViewById<RelativeLayout>(Resource.Id.gameArea);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnResume();

            if (_service == null)
            {
                _service = new Services.BeeAtackService(this);
                _service.StartGame(new System.Drawing.Size(Flower.Width, Flower.Height),
                    new System.Drawing.Size(Hive.Width, Hive.Height),
                    new System.Drawing.Size(GameArea.Width, GameArea.Height));
            }
        }
    }
}