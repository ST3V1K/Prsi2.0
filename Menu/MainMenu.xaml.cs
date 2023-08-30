using System;
using System.Windows;
using System.Windows.Controls;
using static Prsi.Values;

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
        }

        public void SetName()
        {
            if (!string.IsNullOrEmpty(PlayerName))
                Dispatcher.Invoke(() => lbName.Content = PlayerName);
        }

        private async void BtnJoinQueue_Click(object sender, RoutedEventArgs e)
        {
            var response = await GameClient.NewGameAsync(ServerPlayer, deadline: Deadline);

            GameUuid = Guid.Parse(response.Uuid);
            Values.Game.SetSeed(response.Seed);

            Switcher.Switch(new CreateGame());
        }

        private void BtnJoinGame_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new JoinGame());
        }
    }
}
