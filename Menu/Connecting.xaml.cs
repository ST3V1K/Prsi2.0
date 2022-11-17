using Npgsql;
using Prsi.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
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
    /// Interakční logika pro Connecting.xaml
    /// </summary>
    public partial class Connecting : UserControl
    {
        public Connecting()
        {
            InitializeComponent();
        }

        bool Server1;
        bool Server2;

        private async void Connecting_Loaded(object sender, RoutedEventArgs e)
        {
            Values.Connection = new(Values.CONNECTION_STRING);
            Values.Connection_Listen = Values.Connection.CloneWith(Values.CONNECTION_STRING);

            await Task.Run(async () =>
            {
                try
                {
                    await Values.Connection.OpenAsync();
                    Server1 = true;
                }
                catch { }
            });
            await Task.Run(async () =>
            {
                try
                {
                    await Values.Connection_Listen.OpenAsync();
                    if (Switcher.PageSwitcher != null)
                    {
                        Values.Connection_Listen.Notification += Listener.HandleListen;
                        Server2 = true;
                    }
                }
                catch { }
            }, new(Values.IsFormClosed == true || !Server1))
                .ContinueWith(t =>
                {
                    if (t.IsCanceled) { }
                });

            if (!Server1 && !Server2)
            {
                Switcher.Switch(new FailedToConnect());
                return;
            }

            if (!Values.IsFormClosed)
            {
                ChangeName changeName = new() { Owner = Switcher.PageSwitcher };
                changeName.ShowDialog();

                Switcher.Switch(new MainMenu());

                if (!Values.IsFormClosed)
                    using (NpgsqlCommand cmd = new($"LISTEN \"{Values.Channel}\";", Values.Connection_Listen))
                        await Task.Run(async () => await cmd.ExecuteNonQueryAsync(Values.FormClosedToken));

                await Task.Run(async () =>
                {
                    while (!Values.IsFormClosed)
                        await Values.Connection_Listen.WaitAsync();
                }, Values.FormClosedToken)
                    .ContinueWith(t => { }, TaskContinuationOptions.OnlyOnCanceled);
            }
        }
    }
}
