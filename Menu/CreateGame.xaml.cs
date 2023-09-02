using Grpc.Core;
using System;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using static Prsi.Values;
using static Prsi.Listener;

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
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (call is not null)
                call.Dispose();

            call = GameClient.NewGame(ServerPlayer);

            await call.ResponseStream.MoveNext();
            GameUuid = Guid.Parse(call.ResponseStream.Current.Uuid);
            var seed = call.ResponseStream.Current.Seed;

            await call.ResponseStream.MoveNext();
            Values.Game.StartGame(call.ResponseStream.Current.OpponentName, true, seed);
            Switcher.Switch(Values.Game);

            backgroundWorker.RunWorkerAsync();
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

            backgroundWorker.CancelAsync();
        }
    }
}
