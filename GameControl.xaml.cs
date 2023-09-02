using Grpc.Core;
using System;
using System.Windows;
using System.Windows.Controls;
using static Prsi.Listener;
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
            if (call is not null)
                call.Dispose();

            GameUuid = Guid.Parse(gameId);

            call = GameClient.Join(new()
            {
                Game = ServerGame,
                Player = ServerPlayer
            });

            await call.ResponseStream.MoveNext();
            Values.Game.StartGame(call.ResponseStream.Current.OpponentName, false, call.ResponseStream.Current.Seed);
            Switcher.Switch(Values.Game);

            backgroundWorker.RunWorkerAsync();
        }
    }
}
