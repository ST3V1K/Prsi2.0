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

        public bool CanBePlayed(Card? card)
        {
            if (card == null) return false;
            return card.Name[0] == Name[0] || card.Name[1..] == Name[1..] || Name[1..] == "12" || !card.Name.Contains("14");
        }
    }
}