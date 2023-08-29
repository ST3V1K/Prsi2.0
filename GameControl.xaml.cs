using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Prsi
{
    /// <summary>
    /// Interakční logika pro GameControl.xaml
    /// </summary>
    public partial class GameControl : UserControl
    {
        private readonly string hostingPlayerName;

        public GameControl(string hostingPlayerName)
        {
            InitializeComponent();
            this.hostingPlayerName = hostingPlayerName;
            lbHostingPlayerName.Content = $"● {hostingPlayerName}";
        }

        private async void BtnJoin_Click(object sender, RoutedEventArgs e)
        {
            //using NpgsqlCommand cmd = new("select pripoj(@jmenoin, @hesloin, @jmenocil, @seedin)", Values.Connection);
            //cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            //cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            //cmd.Parameters.AddWithValue("@jmenocil", hostingPlayerName);
            //cmd.Parameters.AddWithValue("@seedin", new Random().Next(int.MaxValue / 2));
            //int seed = (int?)await cmd.ExecuteScalarAsync() ?? -1;
            
            //if (seed == -1)
            //    return;

            //Values.Game.StartGame(seed, hostingPlayerName, false);
            //Switcher.Switch(Values.Game);
        }
    }
}
