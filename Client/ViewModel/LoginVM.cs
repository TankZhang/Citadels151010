using Client.Command;
using Client.Model.Datas;
using Client.View;
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
    public class LoginVM : NotificationObject
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

        ICommand _btnLoginCmd;
        public ICommand BtnLoginCmd
        {
            get
            {
                return _btnLoginCmd;
            }

            set
            {
                _btnLoginCmd = value;
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

        Thread thReceive;
        //构造函数
        public LoginVM()
        {
            IsEnable = true;
            UserInfo = new UserInfo();
            BtnLoginCmd = new RelayCommand(new Action<object>(Login));
            BtnSignCmd = new RelayCommand(new Action<object>(Sign));
        }
        //点击登陆，进行登陆
        public void Login(object obj)
        {
            thReceive = new Thread(ReceiveSocket);
            thReceive.IsBackground = true;
            thReceive.Start(App.NetCtrl.SocketClient);
            UserInfo userInfo = obj as UserInfo;
            App.NetCtrl.Send("1|3|" + userInfo.Mail + "|" + userInfo.Pwd + "|");
        }
        //点击注册，进入注册界面
        public void Sign(object obj)
        {
            string mailTemp = obj as string;
            SignV signV = new SignV();
            signV.ShowDialog();
        }

        //对socket进行接收
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
                    Console.WriteLine("loginV收到了" + str);
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
            //如果收到登陆成功的消息，关闭本窗口，并return该线程
            if (strs[0] == "1")
            {
                switch (strs[1])
                {
                    case "1":
                        if (strs[2] == "-1")
                        { MessageBox.Show(strs[3]); }
                        break;
                    case "2":
                        MessageBox.Show("服务器混乱发错信息了,这是注册类型消息。。所以。。请重启"); break;
                    case "3":
                        if (strs[2] == "1")
                        {
                            App.UserInfo = new UserInfo(strs[3], strs[4], strs[5], strs[6], int.Parse(strs[7]));
                            IsEnable = false;
                        }
                        else
                        { MessageBox.Show(strs[3]); }
                        break;
                }
            }
        }
    }
}
