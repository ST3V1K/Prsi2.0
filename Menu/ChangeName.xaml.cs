using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Prsi
{
    /// <summary>
    /// Interakční logika pro ChangeName.xaml
    /// </summary>
    public partial class ChangeName : Window
    {
        public ChangeName()
        {
            InitializeComponent();
            txtName.Focus();
        }

        [GeneratedRegex("^[a-zA-Z0-9]+$")]
        private partial Regex NameRegex();

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var name = txtName.Text.Trim();

            if (name.Length == 0)
                return;
            if (name.Length > 32)
            {
                MessageBox.Show("Maximální délka jména je 32 znaků.\nVyberte si jiné.");
                return;
            }
            if (!NameRegex().IsMatch(name))
            {
                MessageBox.Show("Jméno smí obsahovat pouze písmena a číslice.\nVyberte si jiné.");
                return;
            }

            var password = GeneratePassword();

            using NpgsqlCommand cmd = new("select prihlasit(@jmenoin, @hesloin)", Values.Connection);
            //cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@jmenoin", name);
            cmd.Parameters.AddWithValue("@hesloin", password);
            bool? logged_in = (bool?)cmd.ExecuteScalar();

            if (logged_in == false)
            {
                MessageBox.Show("Jméno je již zabrané.\nVyberte si jiné.");
                return;
            }

            Values.PlayerName = name;
            Values.PlayerPassword = password;

            Switcher.PageSwitcher?.Timer.Start();

            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Values.IsFormClosed = true;
            Application.Current.Shutdown();
        }

        private void TxtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbNameLength.Content = txtName.Text.Length;
        }

        private static string GeneratePassword()
        {
            string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "0123456789";
            string specialChars = "+/*_=.";

            Random rnd = new();
            string password = "";

            for (int i = 0; i < 3; i++)
            {
                // Execute 3 times
                password += upperCase[rnd.Next(upperCase.Length)];
                password += lowerCase[rnd.Next(lowerCase.Length)];

                // Execute 2 times
                if (i == 0)
                    continue;

                password += numbers[rnd.Next(numbers.Length)];
                password += specialChars[rnd.Next(specialChars.Length)];
            }

            return password;
        }
    }
}
