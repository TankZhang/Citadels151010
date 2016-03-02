using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.View
{
    /// <summary>
    /// LoginV.xaml 的交互逻辑,
    /// </summary>
    public partial class LoginV : Window
    {
        public LoginV()
        {
            InitializeComponent();
        }

        private void pwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordtext = (PasswordBox)sender;
            SetPasswordBoxSelection(passwordtext, passwordtext.Password.Length + 1, passwordtext.Password.Length + 1);
        }
        private static void SetPasswordBoxSelection(PasswordBox passwordBox, int start, int length)
        {
            var select = passwordBox.GetType().GetMethod("Select",
                            BindingFlags.Instance | BindingFlags.NonPublic);

            select.Invoke(passwordBox, new object[] { start, length });
        }

        private void Window_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            if ((bool)e.NewValue == false)
            {
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsEnabled == false)
            {
                LobbyV lobbyV = new LobbyV();
                lobbyV.Show();
            }
        }
    }
}
