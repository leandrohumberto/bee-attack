using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;
using BeeAttack.Services;

namespace BeeAttack
{
    [Activity(Label = "GameActivity", Theme = "@style/StartUpAppTheme", 
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class GameActivity : Activity
    {
        private ImageView _flower;
        private ImageView _hive;
        private TextView _misses;
        private TextView _score;
        private RelativeLayout _gameArea;
        private Button _restart;
        private bool _running;
        private LinearLayout _gameOverLayout;
        private Fragments.BeeAttackServiceFragment _fragment;
        private BeeAttackService _service;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Game);

            _flower = FindViewById<ImageView>(Resource.Id.flowerView);
            _hive = FindViewById<ImageView>(Resource.Id.hiveView);
            _gameArea = FindViewById<RelativeLayout>(Resource.Id.gameArea);
            _misses = FindViewById<TextView>(Resource.Id.missesLeftText);
            _score = FindViewById<TextView>(Resource.Id.scoreText);
            _restart = FindViewById<Button>(Resource.Id.restartButton);
            _gameOverLayout = FindViewById<LinearLayout>(Resource.Id.gameOverLayout);
            _fragment = (Fragments.BeeAttackServiceFragment)FragmentManager.FindFragmentByTag("GameService");

            if (_fragment == null)
            {
                _fragment = new Fragments.BeeAttackServiceFragment(new BeeAttackService(this));
                var fragmentTransaction = FragmentManager.BeginTransaction();
                fragmentTransaction.Add(_fragment, "GameService");
                fragmentTransaction.Commit();
            }
            _service = _fragment.Service;

            _service.HiveMoved += Service_HiveMoved;
            _service.BeeAdded += Service_BeeAdded;
            _service.GameOver += Service_GameOver;
            _service.Missed += Service_Missed;
            _service.Scored += Service_Scored;
            _flower.Touch += Flower_Touch;
            _restart.Click += delegate 
            {
                Intent intent = Intent;
                Finish();
                StartActivity(intent);
            };
        }

        protected override void OnPause()
        {
            base.OnPause();
            PauseGame();
        }

        protected override void OnStop()
        {
            base.OnStop();
            PauseGame();
        }

        protected override void OnResume()
        {
            base.OnResume();
            PauseGame(false);
        }

        private void PauseGame(bool pause = true)
        {
            _service.Paused = pause;
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (!_running && _gameOverLayout.Visibility == ViewStates.Invisible)
            {
                StartGame();
            }
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
            if (_service.IsGameOver)
                return;

            if (_flower.TranslationX + _flower.Width + newX <= _gameArea.Width || _flower.TranslationX + newX < 0)
            {
                _flower.TranslationX += newX - _flower.Width / 2;
                _service.MoveFlower(_flower.TranslationX);
            }
        }

        private void StartGame()
        {            
            _service.StartGame(new System.Drawing.SizeF(_flower.Width, _flower.Height),
                new System.Drawing.SizeF(_hive.Width, _hive.Height),
                new System.Drawing.SizeF(_gameArea.Width, _gameArea.Height));
            MoveFlower((_gameArea.Width - _flower.Width) / 2);
            _gameOverLayout.Visibility = ViewStates.Invisible;
            _running = true;
        }

        private void Service_BeeAdded(object sender, BeeAddedEventArgs args)
        {
            var bee = args.Bee;
            bee.StartAnimation(AnimationUtils.LoadAnimation(this, Resource.Animation.beeview_animation));
            bee.Animation.AnimationEnd += _service.BeeLanded;
            RunOnUiThread(() => _gameArea.AddView(bee));

            bee.Animation.AnimationEnd += delegate
            {
                bee.ClearAnimation();

                // Caution: Gambiarra!!!
                // TODO: There must be a better way to remove the views... 
                // RunOnUiThread(() => GameArea.RemoveView(bee)) causes an exception
                bee.Visibility = ViewStates.Invisible;
            };
        }

        private void Service_HiveMoved(object sender, HiveMovedEventArgs args)
        {
            RunOnUiThread(() => _hive.TranslationX = args.X);
        }

        private void Service_Scored(object sender, ScoredEventArgs args)
        {
            _score.Text = args.Score.ToString();
        }

        private void Service_Missed(object sender, MissedEventArgs args)
        {
            _misses.Text = args.MissesLeft.ToString();
        }

        private void Service_GameOver(object sender, EventArgs e)
        {
            _running = false;
            _gameOverLayout.Visibility = ViewStates.Visible;
        }
    }
}