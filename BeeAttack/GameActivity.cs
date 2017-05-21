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
        private Services.BeeAttackService _service;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Game);

            // Create your application here
            Flower = FindViewById<ImageView>(Resource.Id.flowerView);
            Hive = FindViewById<ImageView>(Resource.Id.hiveView);
            GameArea = FindViewById<RelativeLayout>(Resource.Id.gameArea);

            Flower.Touch += Flower_Touch;
        }

        private void Flower_Touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Move)
            {
                MoveFlower(e.Event.GetX());
            }
        }

        private void MoveFlower(float newX)
        {
            _service.MoveFlower(newX);

            if (Flower.TranslationX + Flower.Width + newX <= GameArea.Width || Flower.TranslationX + newX < 0)
            {
                Flower.TranslationX += newX;
            }
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnResume();

            if (_service == null)
            {
                _service = new Services.BeeAttackService(this);
                _service.HiveMoved += _service_HiveMoved;
                _service.BeeAdded += _service_BeeAdded;
                _service.StartGame(new System.Drawing.Size(Flower.Width, Flower.Height),
                    new System.Drawing.Size(Hive.Width, Hive.Height),
                    new System.Drawing.Size(GameArea.Width, GameArea.Height));
                MoveFlower((GameArea.Width - Flower.Width) / 2);
            }
        }

        private void _service_BeeAdded(object sender, ImageView bee)
        {
            RunOnUiThread(() => GameArea.AddView(bee));
        }

        private void _service_HiveMoved(object sender, float x)
        {
            RunOnUiThread(() => Hive.TranslationX = x);
        }
    }
}