using System;

namespace BeeAttack.Model
{
    class BeeAttackModel
    {
        public int MissesLeft { get; private set; }
        public int Score { get; private set; }

        public TimeSpan TimeBetweenBees
        {
            get
            {
                double milleseconds = 500;
                milleseconds = Math.Max(milleseconds - Score * 2.5, 100);
                return TimeSpan.FromMilliseconds(milleseconds);
            }
        }

        private double _flowerWidth;
        private double _beeWidth;
        private double _flowerLeft;
        private float _playAreaWidth;
        private double _hiveWidth;
        private float _lastHiveLocation;
        private bool _gameOver;
        private readonly Random _random = new Random();

        public void StartGame(double flowerWidth, double beeWidth, float playAreaWidth, double hiveWidth)
        {
            _flowerWidth = flowerWidth;
            _beeWidth = beeWidth;
            _playAreaWidth = playAreaWidth;
            _hiveWidth = hiveWidth;
            _lastHiveLocation = playAreaWidth / 2;
            MissesLeft = 5;
            Score = 0;
            _gameOver = false;
            OnPlayerScored();
        }

        public void MoveFlower(double flowerLeft)
        {
            _flowerLeft = flowerLeft;
        }

        public void BeeLanded(double beeLeft)
        {
            if ((beeLeft + _beeWidth < _flowerLeft) || ((beeLeft) > _flowerLeft + _flowerWidth))
            {
                if (MissesLeft > 0)
                {
                    MissesLeft--;
                    OnMissed();
                }
                else
                {
                    _gameOver = true;
                    OnGameOver();
                }
            }
            else if (!_gameOver)
            {
                Score++;
                OnPlayerScored();
            }
        }

        public float NextHiveLocation()
        {
            float delta = 10 + Math.Max(1, Score * 2.5f);

            if (_lastHiveLocation <= delta)
                _lastHiveLocation += delta;
            else if (_lastHiveLocation >= _playAreaWidth - _hiveWidth * 2)
                _lastHiveLocation -= delta;
            else
                _lastHiveLocation += delta * (_random.Next(2) == 0 ? 1 : -1);

            return _lastHiveLocation;
        }

        public event EventHandler GameOver;
        private void OnGameOver()
        {
            GameOver?.Invoke(this, new EventArgs());
        }

        public event EventHandler Missed;
        private void OnMissed()
        {
            Missed?.Invoke(this, new EventArgs());
        }

        public event EventHandler PlayerScored;
        private void OnPlayerScored()
        {
            PlayerScored?.Invoke(this, new EventArgs());
        }
    }
}