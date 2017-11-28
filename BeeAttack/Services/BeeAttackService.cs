using System;
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

        private readonly Model.BeeAttackModel _model = new Model.BeeAttackModel();
        private float _hiveTranslation;
        private float _beeWidth;
        private float _beeHeight;
        private float _playAreaWidth;
        private Timer _timer;
        
        public BeeAttackService()
        {
            _model.Missed += MissedEventHandler;
            _model.GameOver += GameOverEventHandler;
            _model.PlayerScored += PlayerScoredEventHandler;
            IsGameOver = false;
        }

        private void HiveTimerTick(object sender)
        {
            if (_playAreaWidth <= 0 || IsGameOver || Paused)
                return;

            _hiveTranslation = _model.NextHiveLocation();
            OnHiveMoved(_hiveTranslation);
            OnBeeAdded(_hiveTranslation, _beeWidth, _beeHeight);
        }

        private void OnBeeAdded(float translationX, float width, float height) 
            => BeeAdded?.Invoke(this, new BeeAddedEventArgs(translationX, width, height));

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

        public void StartGame(float flowerWidth, float hiveWidth, float playAreaWidth)
        {
            _timer?.Dispose();
            _timer = new Timer(new TimerCallback(HiveTimerTick), null, _model.TimeBetweenBees, _model.TimeBetweenBees);
            _playAreaWidth = playAreaWidth;
            _beeWidth = playAreaWidth / 10;
            _beeHeight = playAreaWidth / 10;
            _model.StartGame(flowerWidth, _beeWidth, _playAreaWidth, hiveWidth);
            OnMissed();

            IsGameOver = false;
        }

        public void BeeLanded(double beeTranslationX)
        {
            _model.BeeLanded(beeTranslationX);
        }

        public void MoveFlower(float newX)
        {
            _model.MoveFlower(newX);
        }
    }
}
