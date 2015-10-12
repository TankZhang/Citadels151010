using Client.Command;
using Client.Model.Net;
using Client.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.ViewModel
{
    public class ConnectVM : NotificationObject
    {
        IPAddr _ip;
        public IPAddr Ip
        {
            get
            {
                return _ip;
            }

            set
            {
                _ip = value;
                RaisePropertyChanged("Ip");
            }
        }

        ICommand _btnConnectCmd;
        public ICommand BtnConnectCmd
        {
            get
            {
                return _btnConnectCmd;
            }

            set
            {
                _btnConnectCmd = value;
            }
        }

        bool _isEnable;
        public bool IsEnable
        {
            get
            {
                return _isEnable;
            }

            set
            {
                _isEnable = value;
                RaisePropertyChanged("IsEnable");
            }
        }

        public ConnectVM()
        {
            Ip = new IPAddr();
            Ip.Ip = "192.168.1.102";
            Ip.Port = "31313";
            BtnConnectCmd = new RelayCommand(new Action<object>(Connect));
            IsEnable = true;
        }


        public void Connect(object obj)
        {
            IPAddr ipTemp = obj as IPAddr;
            App.NetCtrl = new NetCtrl(ipTemp.Ip, ipTemp.Port);
            App.NetCtrl.Send("1|1|");
            if (App.NetCtrl.SocketClient.Connected)
            {
                LoginV loginV = new LoginV();
                loginV.Show();
                IsEnable = false;
            }
            else
            {
                MessageBox.Show("连接失败");
            }
        }
    }
}
