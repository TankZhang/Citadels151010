using Client.Model.Datas;
using Client.Model.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static NetCtrl NetCtrl;
        public static UserInfo UserInfo;

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //NetCtrl.SocketClient.Disconnect(false);
            //NetCtrl.SocketClient.Shutdown(System.Net.Sockets.SocketShutdown.Both);
            //NetCtrl.SocketClient.Close();
        }
    }
}
