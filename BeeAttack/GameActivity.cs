using System;

using Android.App;
using Android.Content;
using Android.OS;
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
        private TextView _misses;
        private TextView _score;
        private RelativeLayout _gameArea;
        private Button _restart;
        private bool _running;
        private LinearLayout _gameOverLayout;
        private Fragments.BeeAttackServiceFragment _fragment;
        private Services.BeeAttackService _service;

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
                _fragment = new Fragments.BeeAttackServiceFragment(new Services.BeeAttackService(this));
                var fragmentTransaction = FragmentManager.BeginTransaction();
                fragmentTransaction.Add(_fragment, "GameService");
                fragmentTransaction.Commit();
            }
            _service = _fragment.Service;

            _service.HiveMoved += _service_HiveMoved;
            _service.BeeAdded += _service_BeeAdded;
            _service.GameOver += _service_GameOver;
            _service.Missed += _service_Missed;
            _service.Scored += _service_Scored;
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
            if (_service.IsGameOver)
                return;

            if (_flower.TranslationX + _flower.Width + newX <= _gameArea.Width || _flower.TranslationX + newX < 0)
            {
                _flower.TranslationX += newX - _flower.Width / 2;
                _service.MoveFlower(_flower.TranslationX);
            }
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (!_running && _gameOverLayout.Visibility == ViewStates.Invisible)
            {
                StartGame();
            }
        }

        private void StartGame()
        {            
            _service.StartGame(new System.Drawing.Size(_flower.Width, _flower.Height),
                new System.Drawing.Size(_hive.Width, _hive.Height),
                new System.Drawing.Size(_gameArea.Width, _gameArea.Height));
            MoveFlower((_gameArea.Width - _flower.Width) / 2);
            _gameOverLayout.Visibility = ViewStates.Invisible;
            _running = true;
        }

        private void _service_BeeAdded(object sender, ImageView bee)
        {
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

        private void _service_HiveMoved(object sender, float x)
        {
            RunOnUiThread(() => _hive.TranslationX = x);
        }

        private void _service_Scored(object sender, int score)
        {
            _score.Text = score.ToString();
        }

        private void _service_Missed(object sender, int missesLeft)
        {
            _misses.Text = missesLeft.ToString();
        }

        private void _service_GameOver(object sender, EventArgs e)
        {
            _running = false;
            _gameOverLayout.Visibility = ViewStates.Visible;
        }
    }
}