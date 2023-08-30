using System;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static Prsi.Values;

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
            lbName.Content = PlayerName;
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(GetMainMenu());
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadGames();
        }

        private async void BtnVyhledat_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilter.Text))
                await LoadGames();
            else
                await LoadGames(txtFilter.Text);
        }

        private async Task LoadGames()
        {
            gamesBox.Children.Clear();

            var response = await GameClient.ListGamesAsync(ServerPlayer, deadline: Deadline);

            foreach (var (gameId, playerName) in Enumerable.Zip(response.GameUuids, response.PlayerNames))
            {
                gamesBox.Children.Add(new GameControl(playerName, gameId));
            }

            if (gamesBox.Children.Count == 0)
                gamesBox.Children.Add(new TextBlock()
                {
                    Text = "Nebyla nalezena žádná hra.\nZkuste to poději.",
                    TextAlignment = TextAlignment.Center
                });
        }

        private async Task LoadGames(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                await LoadGames();
                return;
            }

            gamesBox.Children.Clear();

            var response = await GameClient.ListGamesFilteredAsync(new()
            {
                Player = ServerPlayer,
                Filter = filter
            }, deadline: Deadline);

            foreach (var (gameId, playerName) in Enumerable.Zip(response.GameUuids, response.PlayerNames))
            {
                gamesBox.Children.Add(new GameControl(playerName, gameId));
            }

            if (gamesBox.Children.Count == 0)
                gamesBox.Children.Add(new TextBlock()
                {
                    Text = "Nebyla nalezena žádná hra.\nZkuste to poději.",
                    TextAlignment = TextAlignment.Center
                });
        }
    }
}
