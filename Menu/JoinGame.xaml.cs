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
using Npgsql;

namespace Prsi
{
    /// <summary>
    /// Interakční logika pro JoinGame.xaml
    /// </summary>
    public partial class JoinGame : UserControl
    {
        public JoinGame()
        {
            InitializeComponent();
            lbName.Content = Values.PlayerName;
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadGames();
        }

        private async void BtnVyhledat_Click(object sender, RoutedEventArgs e)
        {
            await LoadGames(txtFilter.Text);
        }

        private async Task LoadGames()
        {
            gamesBox.Children.Clear();
            using NpgsqlCommand cmd = new("select zobraz_hrace(@jmenoin, @hesloin)", Values.Connection);
            cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
                gamesBox.Children.Add(new GameControl(reader.GetString(0)));
        }
        private async Task LoadGames(string filter)
        {
            gamesBox.Children.Clear();
            using NpgsqlCommand cmd = new("select * from zobraz_hrace(@jmenoin, @hesloin) where jmena like @filter", Values.Connection);
            cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            cmd.Parameters.AddWithValue("@filter", filter + '%');
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
                gamesBox.Children.Add(new GameControl(reader.GetString(0)));
        }
    }
}
