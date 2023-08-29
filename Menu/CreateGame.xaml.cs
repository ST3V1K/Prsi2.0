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
            lbName.Content = Values.PlayerName;
            // TODO: listen if someone joins
        }

        private async void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            //using NpgsqlCommand cmd = new("select toggle_queue(@jmenoin, @hesloin, false, NULL)", Values.Connection);
            //cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            //cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            //await cmd.ExecuteNonQueryAsync();
            //Switcher.Switch(Values.GetMainMenu());
        }
    }
}
