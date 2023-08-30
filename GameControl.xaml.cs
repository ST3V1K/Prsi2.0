using System.Windows;
using System.Windows.Controls;
using static Prsi.Values;

namespace Prsi
{
    /// <summary>
    /// Interakční logika pro GameControl.xaml
    /// </summary>
    public partial class GameControl : UserControl
    {
        private readonly string gameId;

        public GameControl(string hostingPlayerName, string gameId)
        {
            InitializeComponent();
            this.gameId = gameId;
            lbHostingPlayerName.Content = $"● {hostingPlayerName}";
        }

        private async void BtnJoin_Click(object sender, RoutedEventArgs e)
        {
            var response = await GameClient.JoinAsync(new()
            {
                Game = new()
                {
                    Uuid = gameId
                },
                Player = ServerPlayer
            });

            Values.Game.StartGame(response.OpponentName, false, response.Seed);
            Switcher.Switch(Values.Game);
        }
    }
}
