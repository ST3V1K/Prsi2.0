using Grpc.Core;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using static Prsi.Values;

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

        private async void BtnConfirm_Click(object sender, RoutedEventArgs e)
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

            try
            {
                var response = await PlayerClient.LoginAsync(
                    new() { Name = name },
                    deadline: Deadline
                    );
                var password = response?.Password;

                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Jméno je již zabrané.\nVyberte si jiné.");
                    return;
                }

                PlayerName = name;
                PlayerPassword = password;

                Switcher.PageSwitcher?.Timer.Start();
                Switcher.Switch(GetMainMenu());

                Close();
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.DeadlineExceeded)
            {
                Switcher.Switch(new FailedToConnect());
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            IsFormClosed = true;
            Application.Current.Shutdown();
        }

        private void TxtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbNameLength.Content = txtName.Text.Length;
        }
    }
}
