using Client.Command;
using Client.Model.Datas;
using Client.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.ViewModel
{
    public class LobbyVM : NotificationObject
    {
        ObservableCollection<LobbyRoom> _roomList;
        public ObservableCollection<LobbyRoom> RoomList
        {
            get
            {
                return _roomList;
            }

            set
            {
                _roomList = value;
                RaisePropertyChanged("RoomList");
            }
        }

        ObservableCollection<LobbyPlayer> _playerList;
        public ObservableCollection<LobbyPlayer> PlayerList
        {
            get
            {
                return _playerList;
            }

            set
            {
                _playerList = value;
                RaisePropertyChanged("PlayerList");
            }
        }

        bool _canJoin;
        public bool CanJoin
        {
            get
            {
                return _canJoin;
            }

            set
            {
                _canJoin = value;
                RaisePropertyChanged("CanJoin");
            }
        }

        bool _canCreat;
        public bool CanCreat
        {
            get
            {
                return _canCreat;
            }

            set
            {
                _canCreat = value;
                RaisePropertyChanged("CanCreat");
            }
        }

        bool _canStart;
        public bool CanStart
        {
            get
            {
                return _canStart;
            }

            set
            {
                _canStart = value;
                RaisePropertyChanged("CanStart");
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

        int _index;
        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                try
                {
                    if (RoomList[value].Num < 4 & RoomNum == 0 & RoomList[value].Status == "未开始")
                    {
                        CanJoin = true;
                    }
                    else { CanJoin = false; }
                }
                catch { }
                _index = value;
                RaisePropertyChanged("Index");
            }
        }

        int _roomNum;
        public int RoomNum
        {
            get
            {
                return _roomNum;
            }

            set
            {
                if (value != 0)
                { CanCreat = false; }
                else
                { CanCreat = true; }
                _roomNum = value;
                RaisePropertyChanged("RoomNum");
            }
        }

        int _seatNum;
        public int SeatNum
        {
            get
            {
                return _seatNum;
            }

            set
            {
                _seatNum = value;
            }
        }

        ICommand _btnJoinCmd;
        public ICommand BtnJoinCmd
        {
            get
            {
                return _btnJoinCmd;
            }

            set
            {
                _btnJoinCmd = value;
            }
        }

        ICommand _btnCreatCmd;
        public ICommand BtnCreatCmd
        {
            get
            {
                return _btnCreatCmd;
            }

            set
            {
                _btnCreatCmd = value;
            }
        }

        ICommand _btnStartCmd;
        public ICommand BtnStartCmd
        {
            get
            {
                return _btnStartCmd;
            }

            set
            {
                _btnStartCmd = value;
            }
        }


        public void Join()
        {
            App.NetCtrl.Send("2|2|" + RoomList[Index].RNum + "|" + App.UserInfo.Mail + "|");
            SeatNum = RoomList[Index].Num + 1;
        }

        public void Creat()
        {
            App.NetCtrl.Send("2|1|" + App.UserInfo.Mail + "|");
            SeatNum = 1;
        }

        public void Start()
        {
            App.NetCtrl.Send("2|4|" + RoomNum + "|");
        }

        Thread ThReceive;

        public delegate void Del(string a);
        Del del;
        
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
                    //DealReceivePre(str);
                    Console.WriteLine("Lobby收到了：" + str);
                    string[] strs = str.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.Contains("2|4|1|"))
                    {
                        IsEnable = false;
                        return;
                    }
                    Application.Current.Dispatcher.Invoke(del, str);
                }//try结束
                catch (Exception ex)
                {
                    //抛出异常信息
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }

        //预处理，防止收到两个连续数据包
        void DealReceivePre(string s)
        {
            string[] ss = s.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in ss)
            {
                DealReceive(item);
            }
        }

        //处理lobby收到的信息
        private void DealReceive(string s)
        {
            string[] strs = s.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs[0] != "2") { return; }
            switch (strs[1])
            {
                //返回的创建房间信息
                case "1":
                    if (strs[2] == "1")
                    {
                        RoomNum = int.Parse(strs[3]);
                        CanJoin = false;
                        CanCreat = false;
                    }
                    break;
                //返回的加入房间信息
                case "2":
                    if (strs[2] == "1")
                    {
                        RoomNum = int.Parse(strs[3]);
                        CanJoin = false;
                        CanCreat = false;
                    }
                    break;
                //返回的房间内部信息
                case "3":
                    DealRoomData(strs);
                    break;
                //返回与开始游戏相关的数据
                case "4":
                    DealGameData(strs);
                    break;

            }
        }

        //处理游戏数据
        private void DealGameData(string[] strs)
        {
            switch (strs[2])
            {
                //可以开始游戏
                case "2":
                    CanStart = true; break;
            }
        }

        //处理房间内部信息
        private void DealRoomData(string[] strs)
        {
            switch (strs[2])
            {
                //处理粗略信息
                case "1":
                    RoomList = new ObservableCollection<LobbyRoom>();
                    for (int i = 0; i < (strs.Length - 3) / 4; i++)
                    {
                        LobbyRoom lr = new LobbyRoom();
                        lr.RNum = int.Parse(strs[4 * i + 3]);
                        lr.Num = int.Parse(strs[4 * i + 4]);
                        lr.Creater = strs[4 * i + 5];
                        lr.Status = strs[4 * i + 6];
                        RoomList.Add(lr);
                    }
                    break;
                //处理详细信息
                case "2":
                    PlayerList = new ObservableCollection<LobbyPlayer>();
                    for (int i = 0; i < (strs.Length - 4) / 3; i++)
                    {
                        LobbyPlayer lp = new LobbyPlayer();
                        lp.SNum = int.Parse(strs[3 * i + 4]);
                        lp.Nick = strs[3 * i + 5];
                        lp.Exp = int.Parse(strs[3 * i + 6]);
                        PlayerList.Add(lp);
                    }
                    break;
            }
        }

        public LobbyVM()
        {
            del = new Del(DealReceivePre);
            RoomNum = 0;
            SeatNum = 0;
            CanJoin = false;
            CanCreat = true;
            CanStart = false;
            IsEnable = true;
            RoomList = new ObservableCollection<LobbyRoom>();
            PlayerList = new ObservableCollection<LobbyPlayer>();
            ThReceive = new Thread(ReceiveSocket);
            ThReceive.IsBackground = true;
            ThReceive.Start(App.NetCtrl.SocketClient);
            App.NetCtrl.Send("2|3|1|");
            BtnJoinCmd = new RelayCommand(new Action(Join));
            BtnCreatCmd = new RelayCommand(new Action(Creat));
            BtnStartCmd = new RelayCommand(new Action(Start));
        }
    }
}
