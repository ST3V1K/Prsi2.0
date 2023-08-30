using System.Windows;
using System.Windows.Controls;
using static Prsi.Values;

namespace Prsi
{
    /// <summary>
    /// Interakční logika pro CreateGame.xaml
    /// </summary>
    public partial class CreateGame : UserControl
    {
        public CreateGame()
        {
            InitializeComponent();
            lbName.Content = PlayerName;
            // TODO: listen if someone joins
        }

        private async void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            var response = await GameClient.RemoveGameAsync(new()
            {
                Game = ServerGame,
                Player = ServerPlayer
            }, deadline: Deadline);

            if (response.Success)
                Switcher.Switch(GetMainMenu());
        }
    }
}
