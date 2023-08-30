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
        public DispatcherTimer Timer { get; init; }

        public PageSwitcher()
        {
            InitializeComponent();

            Switcher.PageSwitcher = this;
            Switcher.Switch(new Connecting());

            Timer = new() { Interval = new(0, 0, 20) };
            Timer.Tick += MakeUserActive;
        }

        private async void MakeUserActive(object? sender, EventArgs e)
        {
            //var conn = Values.Connection?.CloneWith(Values.CONNECTION_STRING);

            //if (conn == null) return;

            //await using NpgsqlCommand cmd = new("select zaktivovat(@jmenoin, @hesloin)", conn);
            //cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            //cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            //await conn.OpenAsync();
            //await cmd.ExecuteNonQueryAsync();
            //await conn.CloseAsync();
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
