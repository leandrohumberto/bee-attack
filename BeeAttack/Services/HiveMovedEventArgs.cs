namespace BeeAttack.Services
{
    public class HiveMovedEventArgs : System.EventArgs
    {
        public float X { get; private set; }

        public HiveMovedEventArgs(float x)
        {
            X = x;
        }
    }
}