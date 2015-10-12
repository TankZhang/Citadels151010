using Client.Command;
using Client.Model.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.ViewModel
{
   public class SignVM:NotificationObject
    {
        UserInfo _userInfo;
        public UserInfo UserInfo
        {
            get
            {
                return _userInfo;
            }

            set
            {
                _userInfo = value;
                RaisePropertyChanged("UserInfo");
            }
        }

        ICommand _btnSignCmd;
        public ICommand BtnSignCmd
        {
            get
            {
                return _btnSignCmd;
            }

            set
            {
                _btnSignCmd = value;
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

        public void Sign(object obj)
        {
            UserInfo userInfo = obj as UserInfo;
            App.NetCtrl.Send("1|2|"+userInfo.Mail+"|"+userInfo.Nick+"|"+userInfo.Pwd+"|"+userInfo.Real+"|");
            thReceive = new Thread(ReceiveSocket);
            thReceive.IsBackground = true;
            thReceive.Start(App.NetCtrl.SocketClient);
        }

        public void ReceiveSocket(object obj)
        {
            Socket s = obj as Socket;
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024 * 5];
                try
                {
                    int r = s.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    string str = Encoding.UTF8.GetString(buffer, 0, r);
                    Console.WriteLine("SignV收到了" + str);
                    DealPre(str);
                    return;
                }//try结束
                catch (Exception ex)
                {
                    //抛出异常信息
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }

        private void DealPre(string str)
        {
            string[] strs = str.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in strs)
            {
                Deal(item);
            }
        }

        private void Deal(string str)
        {
            string[] strs = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if((strs[0]=="1")&(strs[1]=="2"))
            {
                switch(strs[2])
                {
                    case "-1": MessageBox.Show(strs[3]); break;
                    case "1":MessageBox.Show("注册成功");IsEnable = false;break;
                }
            }
        }

        Thread thReceive;
        //构造函数
        public SignVM()
        {
            IsEnable = true;
            UserInfo = new UserInfo();
            BtnSignCmd = new RelayCommand(new Action<object>(Sign));
        }
    }
}
