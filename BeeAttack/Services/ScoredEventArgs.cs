using System;

namespace BeeAttack.Services
{
    public class ScoredEventArgs : EventArgs
    {
        public int Score { get; private set; }

        public ScoredEventArgs(int score)
        {
            Score = score;
        }
    }
}