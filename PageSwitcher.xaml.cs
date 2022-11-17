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
using Npgsql;

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
            await using NpgsqlCommand cmd = new("select zaktivovat(@jmenoin, @hesloin)", Values.Connection);
            cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            await cmd.ExecuteNonQueryAsync();
            await cmd.DisposeAsync();
        }

        private async void Prsi_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Values.IsFormClosed = true;
            if (Values.Connection?.State == ConnectionState.Open)
                await Values.Connection.CloseAsync();
        }
    }
}
