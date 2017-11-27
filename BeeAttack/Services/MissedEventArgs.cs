using System;

namespace BeeAttack.Services
{
    public class MissedEventArgs : EventArgs
    {
        public int MissesLeft { get; private set; }

        public MissedEventArgs(int missesLeft)
        {
            MissesLeft = missesLeft;
        }
    }
}