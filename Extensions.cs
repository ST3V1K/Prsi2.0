using System;
using System.IO;
using System.Windows.Media.Imaging;
using static Prsi.Server.Card.Types.Color;

namespace Prsi
{
    static class Extensions
    {
        public static BitmapImage ToBitmapImage(this byte[] data)
        {
            using MemoryStream ms = new(data);
            BitmapImage img = new();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = ms;
            img.EndInit();

            if (img.CanFreeze)
                img.Freeze();

            return img;
        }

        public static string GetCardName(this Server.Card card) => 
            card.Color switch
            {
                Spades => "l",
                Hearts => "s",
                Diamonds => "k",
                Clubs => "z",
                _ => throw new ArgumentException()
            } + card.Value;

        public static Server.Card.Types.Color ToColor(this char c) =>
            c switch
            {
                'l' => Spades,
                's' => Hearts,
                'k' => Diamonds,
                'z' => Clubs,
                _ => throw new ArgumentException()
            };

        public static char ToChar(this Server.Card.Types.Color color) =>
            color switch
            {
                Spades => 'l',
                Hearts => 's',
                Diamonds => 'k',
                Clubs => 'z',
                _ => throw new ArgumentException()
            };
    }
}