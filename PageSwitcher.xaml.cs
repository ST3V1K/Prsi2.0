using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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
            Values.IsFormClosed = true;

            if (Values.Game.IsRunning)
                await Values.Game.Surrender();

            //try
            //{
            //    using var cmdLogOut = new NpgsqlCommand("select odhlasit(@jmenoin, @hesloin)", Values.Connection);
            //    cmdLogOut.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            //    cmdLogOut.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            //    cmdLogOut.ExecuteNonQuery();
            //}
            //catch (NoNameException) { }

            //try
            //{
            //    if (Values.Connection != null)
            //        await Values.Connection.CloseAsync();
            //}
            //catch (NpgsqlOperationInProgressException) { }
            //try
            //{
            //    if (Values.Connection_Listen != null)
            //        await Values.Connection_Listen.CloseAsync();
            //}
            //catch (NpgsqlOperationInProgressException) { }
        }
    }
}
