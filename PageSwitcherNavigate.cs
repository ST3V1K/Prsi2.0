using System.Windows.Controls;
using System;

namespace Prsi
{
    public partial class PageSwitcher
    {
        public void Navigate(UserControl nextPage)
        {
            Content = nextPage;
        }
    }
}