using Server.Net;
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

namespace Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.vM;
        }

        private void ServerStart_Click(object sender, RoutedEventArgs e)
        {
            ServerIp sip = cmboIP.SelectedItem as ServerIp;
            App.vM.NetCtrl = new NetCtrl(sip.Ip.ToString(), tbxPort.Text);
            gridSuccess.Visibility = Visibility.Visible;
            gridStart.Visibility = Visibility.Collapsed;
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
