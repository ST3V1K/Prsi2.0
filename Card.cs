using System;
using System.Windows.Media.Imaging;

using static Prsi.Server.Card.Types.Color;

namespace Prsi
{
    public class Card
    {
        public string Name { get; init; }

        public BitmapImage BitmapImage { get; init; }

        public char ColorCode { get; init; }

        public Server.Card.Types.Color Color
        {
            get => ColorCode switch
            {
                'l' => Spades,
                's' => Hearts,
                'k' => Diamonds,
                'ž' => Clubs,
                _ => throw new NotImplementedException()
            };
        }

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

            bool color = lastPlayed.ColorCode == ColorCode && Values.Game.ChangeTo == null && (lastPlayed.Number != 14 && lastPlayed.Number != 7 || lastPlayed.CanPlay);
            bool number = lastPlayed.Number == Number && Values.Game.ChangeTo == null;
            bool changeColor = Number == 12 && (lastPlayed.Number != 14 && lastPlayed.Number != 7 || lastPlayed.CanPlay);
            bool wasChanged = lastPlayed.Number == 12 && (Values.Game.ChangeTo == ColorCode);

            return color || number || changeColor || wasChanged;
        }
    }
}