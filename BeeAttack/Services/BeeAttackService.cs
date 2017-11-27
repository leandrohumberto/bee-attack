using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace BeeAttack.Services
{
    public class BeeAttackService
    {
        private bool _isGameOver;

        public bool IsGameOver
        {
            get { return _isGameOver; }
            private set
            {
                _isGameOver = value;
                OnGameOver();
            }
        }

        public bool Paused { get; set; }

        public event EventHandler<HiveMovedEventArgs> HiveMoved;
        public event EventHandler<BeeAddedEventArgs> BeeAdded;
        public event EventHandler GameOver;
        public event EventHandler<MissedEventArgs> Missed;
        public event EventHandler<ScoredEventArgs> Scored;

        private readonly ObservableCollection<ImageView> _beeControls = new ObservableCollection<ImageView>();
        private readonly Model.BeeAttackModel _model = new Model.BeeAttackModel();
        private float _hiveTranslation;
        private SizeF _beeSize;
        private Timer _timer;
        private SizeF _playAreaSize;
        private SizeF _hiveSize;
        private SizeF _flowerSize;
        private GameActivity _gameActivity;
        
        public BeeAttackService(GameActivity gameActivity)
        {
            _gameActivity = gameActivity;
            _model.Missed += MissedEventHandler;
            _model.GameOver += GameOverEventHandler;
            _model.PlayerScored += PlayerScoredEventHandler;
            IsGameOver = false;
        }

        private void HiveTimerTick(object sender)
        {
            if (_playAreaSize.Width <= 0 || IsGameOver || Paused)
                return;

            _hiveTranslation = _model.NextHiveLocation();
            OnHiveMoved(_hiveTranslation);

            ImageView bee = new ImageView(_gameActivity);
            bee.TranslationX = _hiveTranslation + _hiveSize.Width / 2;
            bee.TranslationY = _playAreaSize.Height + _flowerSize.Height / 3;
            var layoutParams = new RelativeLayout.LayoutParams((int)_beeSize.Width, (int)_beeSize.Height);
            layoutParams.AddRule(LayoutRules.AlignParentTop);
            bee.LayoutParameters = layoutParams;
            bee.SetImageResource(Resource.Drawable.bee);
            bee.SetY(_hiveSize.Height);
            bee.Visibility = ViewStates.Visible;

            OnBeeAdded(bee);
        }

        private void OnBeeAdded(ImageView bee)
        {
            _beeControls.Add(bee);
            BeeAdded?.Invoke(this, new BeeAddedEventArgs(bee));
        }

        private void OnHiveMoved(float _hiveTranslation)
        {
            HiveMoved?.Invoke(this, new HiveMovedEventArgs(_hiveTranslation));
        }

        private void OnGameOver()
        {
            GameOver?.Invoke(this, new EventArgs());
        }

        private void OnMissed()
        {
            Missed?.Invoke(this, new MissedEventArgs(_model.MissesLeft));
        }

        private void OnScored()
        {
            Scored?.Invoke(this, new ScoredEventArgs(_model.Score));
        }

        void PlayerScoredEventHandler(object sender, EventArgs e)
        {
            _timer.Change(_model.TimeBetweenBees, _model.TimeBetweenBees);
            OnScored();
        }

        void GameOverEventHandler(object sender, EventArgs e)
        {
            IsGameOver = true;
        }

        void MissedEventHandler(object sender, EventArgs e)
        {
            OnMissed();
        }

        public void StartGame(SizeF flowerSize, SizeF hiveSize, SizeF playAreaSize)
        {
            _timer?.Dispose();
            _timer = new Timer(new TimerCallback(HiveTimerTick), null, _model.TimeBetweenBees, _model.TimeBetweenBees);
            _flowerSize = flowerSize;
            _hiveSize = hiveSize;
            _playAreaSize = playAreaSize;
            _beeSize = new SizeF(playAreaSize.Width / 10, playAreaSize.Width / 10);
            _model.StartGame(_flowerSize.Width, _beeSize.Width, playAreaSize.Width, hiveSize.Width);
            OnMissed();

            IsGameOver = false;
        }

        public void BeeLanded(object sender, EventArgs e)
        {
            List<ImageView> landedBees = new List<ImageView>(_beeControls);
            landedBees = (from control in landedBees
                          where control.Animation == sender
                          select control).ToList();

            if (landedBees.Count > 0)
            {
                var landedBee = landedBees.First();
                _model.BeeLanded(landedBee.TranslationX);
                _beeControls.Remove(landedBee);
            }          
        }

        public void MoveFlower(float newX)
        {
            _model.MoveFlower(newX);
        }
    }
}
