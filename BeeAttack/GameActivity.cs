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
using Android.Views.Animations;

namespace BeeAttack
{
    [Activity(Label = "GameActivity", Theme = "@style/StartUpAppTheme", 
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class GameActivity : Activity
    {
        private ImageView _flower;
        private ImageView _hive;
        private RelativeLayout _gameArea;
        private Button _restart;
        private Services.BeeAttackService _service;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Game);

            _flower = FindViewById<ImageView>(Resource.Id.flowerView);
            _hive = FindViewById<ImageView>(Resource.Id.hiveView);
            _gameArea = FindViewById<RelativeLayout>(Resource.Id.gameArea);
            _restart = FindViewById<Button>(Resource.Id.restartButton);
            _service = new Services.BeeAttackService(this);

            _flower.Touch += Flower_Touch;
            _restart.Click += delegate 
            {
                Intent intent = Intent;
                Finish();
                StartActivity(intent);
            };
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

            if (_flower.TranslationX + _flower.Width + newX <= _gameArea.Width || _flower.TranslationX + newX < 0)
            {
                _flower.TranslationX += newX;
            }
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnResume();

            if (!_service.GameStarted)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            _service.HiveMoved += _service_HiveMoved;
            _service.BeeAdded += _service_BeeAdded;
            _service.StartGame(new System.Drawing.Size(_flower.Width, _flower.Height),
                new System.Drawing.Size(_hive.Width, _hive.Height),
                new System.Drawing.Size(_gameArea.Width, _gameArea.Height));
            MoveFlower((_gameArea.Width - _flower.Width) / 2);
        }

        private void _service_BeeAdded(object sender, ImageView bee)
        {
            bee.StartAnimation(AnimationUtils.LoadAnimation(this, Resource.Animation.beeview_animation));
            RunOnUiThread(() => _gameArea.AddView(bee));

            bee.Animation.AnimationEnd += delegate
            {
                try
                {
                    bee.Animation.Cancel();
                    bee.ClearAnimation();

                    // Caution: Gambiarra!!!
                    // TODO: There must be a better way to remove the views... 
                    // RunOnUiThread(() => GameArea.RemoveView(bee)) causes an exception
                    bee.Visibility = ViewStates.Invisible;
                    bee.Dispose();
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }
            };
        }

        private void _service_HiveMoved(object sender, float x)
        {
            RunOnUiThread(() => _hive.TranslationX = x);
        }
    }
}