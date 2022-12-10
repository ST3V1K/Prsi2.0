using System.Windows.Media.Imaging;

namespace Prsi
{
    public class Card
    {
        public string Name { get; init; }

        public BitmapImage BitmapImage { get; init; }

        public char ColorCode { get; init; }

        public char? ChangeToColor { get; set; }

        public int Number { get; init; }

        public bool CanPlay { get; set; }

        public Card(string name, byte[] image)
        {
            Name = name;
            BitmapImage = image.ToBitmapImage();
            ColorCode = name[0];
            if (name == "zdk")
                Number = -1;
            else
                Number = int.Parse(name[1..].Length <= 2 ? name[1..] : name[1..^1]);
        }

        public bool CanBePlayed(Card? lastPlayed)
        {
            if (lastPlayed == null) return false;

            bool color = lastPlayed.ColorCode == ColorCode && lastPlayed.ChangeToColor == null && (lastPlayed.Number != 14 && lastPlayed.Number != 7 || lastPlayed.CanPlay);
            bool number = lastPlayed.Number == Number && lastPlayed.ChangeToColor == null;
            bool changeColor = Number == 12 && (lastPlayed.Number != 14 && lastPlayed.Number != 7 || lastPlayed.CanPlay);
            bool wasChanged = lastPlayed.Number == 12 && (lastPlayed.ChangeToColor == ColorCode);

            return color || number || changeColor || wasChanged;
        }
    }
}