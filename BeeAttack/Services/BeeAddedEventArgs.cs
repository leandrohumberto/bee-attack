using Android.Widget;

namespace BeeAttack.Services
{
    public class BeeAddedEventArgs
    {
        public ImageView Bee { get; private set; }

        public BeeAddedEventArgs(ImageView bee)
        {
            Bee = bee;
        }
    }
}