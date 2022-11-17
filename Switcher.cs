using System.Windows.Controls;

namespace Prsi
{
    public static class Switcher
    {
        private static PageSwitcher? pageSwitcher;

        public static PageSwitcher? PageSwitcher { get => pageSwitcher; set => pageSwitcher = value; }

        public static void Switch(UserControl newPage)
        {
            pageSwitcher?.Navigate(newPage);
        }
    }
}