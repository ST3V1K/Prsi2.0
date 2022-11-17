using Npgsql;
using Prsi.Menu;
using System;
using System.Collections.Generic;
using System.Data;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Prsi
{
    /// <summary>
    /// Interakční logika pro MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
            try
            {
                lbName.Content = Values.PlayerName;
            }
            catch (NoNameException) { }
        }

        private async void BtnJoinQueue_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new CreateGame());
            using NpgsqlCommand cmd = new("select toggle_queue(@jmenoin, @hesloin, true, @seedin)", Values.Connection);
            cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            cmd.Parameters.AddWithValue("@seedin", new Random().Next(int.MaxValue / 2));
            await cmd.ExecuteNonQueryAsync();
        }

        private void BtnJoinGame_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new JoinGame());
        }
    }
}
