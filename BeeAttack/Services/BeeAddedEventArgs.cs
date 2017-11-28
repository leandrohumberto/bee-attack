namespace BeeAttack.Services
{
    public class BeeAddedEventArgs
    {
        public float TranslationX { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public BeeAddedEventArgs(float translationX, float width, float height)
        {
            TranslationX = translationX;
            Width = width;
            Height = height;
        }
    }
}