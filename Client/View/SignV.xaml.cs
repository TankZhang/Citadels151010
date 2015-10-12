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
    /// SignV.xaml 的交互逻辑
    /// </summary>
    public partial class SignV : Window
    {
        public SignV()
        {
            InitializeComponent();
        }

        private void Window_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                Close();
            }
        }

        private void pwdbx1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pwdbx1.Password != pwdbx2.Password)
            {
                btnSign.IsEnabled = false;
            }
            else
            {
                btnSign.IsEnabled = true;
            }
            PasswordBox passwordtext = (PasswordBox)sender;
            SetPasswordBoxSelection(passwordtext, passwordtext.Password.Length + 1, passwordtext.Password.Length + 1);

        }

        private void pwdbx2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pwdbx1.Password != pwdbx2.Password)
            {
                btnSign.IsEnabled = false;
            }
            else
            {
                btnSign.IsEnabled = true;
            }
        }
        private static void SetPasswordBoxSelection(PasswordBox passwordBox, int start, int length)
        {
            var select = passwordBox.GetType().GetMethod("Select",
                            BindingFlags.Instance | BindingFlags.NonPublic);

            select.Invoke(passwordBox, new object[] { start, length });
        }
    }
}
