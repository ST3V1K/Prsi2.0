using System.IO;
using System.Windows.Media.Imaging;

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
    }
}