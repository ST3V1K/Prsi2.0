using System.Windows.Media.Imaging;

namespace Prsi
{
    public class Card
    {
        public string Name { get; init; }

        public BitmapImage BitmapImage { get; init; }

        public Card(string name, byte[] image)
        {
            Name = name;
            BitmapImage = image.ToBitmapImage();
        }
    }
}