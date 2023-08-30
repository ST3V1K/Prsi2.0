using System;
using System.Windows;
using System.Windows.Threading;
using static Prsi.Values;

namespace Prsi
{
    /// <summary>
    /// Interaction logic for PageSwitcher.xaml
    /// </summary>
    public partial class PageSwitcher : Window
    {
        public PageSwitcher()
        {
            InitializeComponent();

            Switcher.PageSwitcher = this;
            Switcher.Switch(new Connecting());
        }

        private async void Prsi_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsFormClosed = true;

            if (Values.Game.IsRunning)
                await Values.Game.Surrender();

            await PlayerClient.LogoutAsync(new()
            {
                Game = ServerGame,
                Player = ServerPlayer
            }, deadline: Deadline);
        }
    }
}
