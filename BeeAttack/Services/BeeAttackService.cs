using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading;

namespace BeeAttack.Services
{
    public class BeeAttackService
    {
        public bool GameStarted { get; private set; }
        public event EventHandler<float> HiveMoved;
        public event EventHandler<ImageView> BeeAdded;

        private readonly ObservableCollection<ImageView> _beeControls =
            new ObservableCollection<ImageView>();
        private readonly Model.BeeAttackModel _model = new Model.BeeAttackModel();
        private float _hiveTranslation;
        private ViewStates _gameOver;
        private Size _beeSize;
        private Timer _timer;
        private Size _playAreaSize;
        private Size _hiveSize;
        private Size _flowerSize;
        private GameActivity _gameActivity;
        
        public BeeAttackService(GameActivity gameActivity)
        {
            _gameActivity = gameActivity;
            _model.Missed += MissedEventHandler;
            _model.GameOver += GameOverEventHandler;
            _model.PlayerScored += PlayerScoredEventHandler;
            _gameOver = ViewStates.Visible;
            // OnPropertyChanged("GameOver");
        }

        private void HiveTimerTick(object sender)
        {
            if (_playAreaSize.Width <= 0 || _gameOver != ViewStates.Invisible)
            {
                return;
            }

            _hiveTranslation = _model.NextHiveLocation();
            OnHiveMoved(_hiveTranslation);

            ImageView bee = new ImageView(_gameActivity);
            bee.TranslationX = _hiveTranslation + _hiveSize.Width / 2;
            bee.TranslationY = _playAreaSize.Height + _flowerSize.Height / 3;
            var layoutParams = new RelativeLayout.LayoutParams(_beeSize.Width, _beeSize.Height);
            layoutParams.AddRule(LayoutRules.AlignParentTop);
            bee.LayoutParameters = layoutParams;
            bee.SetImageResource(Resource.Drawable.bee);
            bee.SetY(_hiveSize.Height);
            bee.Visibility = ViewStates.Visible;

            _beeControls.Add(bee);
            OnBeeAdded(bee);
        }

        private void OnBeeAdded(ImageView bee)
        {
            BeeAdded?.Invoke(this, bee);
        }

        private void OnHiveMoved(float _hiveTranslation)
        {
            HiveMoved?.Invoke(this, _hiveTranslation);
        }

        void PlayerScoredEventHandler(object sender, EventArgs e)
        {
            // OnPropertyChanged("Score");
            _timer.Change(_model.TimeBetweenBees, _model.TimeBetweenBees);
        }

        void GameOverEventHandler(object sender, EventArgs e)
        {
            _gameOver = ViewStates.Visible;
        }

        void MissedEventHandler(object sender, EventArgs e)
        {
            // OnPropertyChanged("MissesLeft");
        }

        public void StartGame(Size flowerSize, Size hiveSize, Size playAreaSize)
        {
            _timer = new Timer(new TimerCallback(this.HiveTimerTick));
            _flowerSize = flowerSize;
            _hiveSize = hiveSize;
            _playAreaSize = playAreaSize;
            _beeSize = new Size(playAreaSize.Width / 10, playAreaSize.Width / 10);
            _model.StartGame(_flowerSize.Width, _beeSize.Width, playAreaSize.Width, hiveSize.Width);
            // OnPropertyChanged("MissesLeft");

            _timer.Change(_model.TimeBetweenBees, _model.TimeBetweenBees);

            _gameOver = ViewStates.Invisible;
            // OnPropertyChanged("GameOver");
            GameStarted = true;
        }

        private void BeeLanded(object sender, EventArgs e)
        {
            ImageView landedBee = null;

            foreach (ImageView sprite in _beeControls)
            {
                if (sprite == sender)
                {
                    landedBee = sprite;
                }
            }

            _model.BeeLanded(landedBee.TranslationX);

            if (landedBee != null)
            {
                _beeControls.Remove(landedBee);
            }
        }

        public void MoveFlower(float newX)
        {
            _model.MoveFlower(newX);
        }
    }
}
