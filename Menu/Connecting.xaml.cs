using Google.Protobuf.Reflection;
using Grpc.Core;
using Grpc.Net.Client;
using Prsi.Server;
using System;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using static Prsi.Values;

namespace Prsi
{
    /// <summary>
    /// Interakční logika pro Connecting.xaml
    /// </summary>
    public partial class Connecting : UserControl
    {
        public Connecting()
        {
            InitializeComponent();
        }

        private void Connecting_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Creates client connections to server
                var channel = GrpcChannel.ForAddress(SERVER_ADDRESS);

                GameClient = new GameService.GameServiceClient(channel);
                PlayerClient = new PlayerService.PlayerServiceClient(channel);
                CardClient = new CardService.CardServiceClient(channel);
            }
            catch
            {
                Switcher.Switch(new FailedToConnect());
                return;
            }

            ChangeName changeName = new() { Owner = Switcher.PageSwitcher };
            changeName.ShowDialog();
        }
    }
}
