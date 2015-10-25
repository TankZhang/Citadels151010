using Client.Command;
using Client.Model.Cards;
using Client.Model.Datas;
using System;
using System.Collections;
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
    public class GameVM : NotificationObject
    {
        Thread ThReceive;
        #region 各种列表ObservableCollection
        //玩家列表
        ObservableCollection<GamePlayer> _gamePlayerList;
        public ObservableCollection<GamePlayer> GamePlayerList
        {
            get
            {
                return _gamePlayerList;
            }

            set
            {
                _gamePlayerList = value;
                RaisePropertyChanged("GamePlayerList");
            }
        }

        //中间的玩家列表
        ObservableCollection<GamePlayer> _centerPlayer;
        public ObservableCollection<GamePlayer> CenterPlayer
        {
            get
            {
                return _centerPlayer;
            }

            set
            {
                _centerPlayer = value;
                RaisePropertyChanged("CenterPlayer");
            }
        }

        //中间的建筑列表
        ObservableCollection<Building> _centerBuildings;
        public ObservableCollection<Building> CenterBuildings
        {
            get
            {
                return _centerBuildings;
            }

            set
            {
                _centerBuildings = value;
                RaisePropertyChanged("CenterBuildings");
            }
        }

        //中间的角色列表
        ObservableCollection<Hero> _centerHeros;
        public ObservableCollection<Hero> CenterHeros
        {
            get
            {
                return _centerHeros;
            }

            set
            {
                _centerHeros = value;
                RaisePropertyChanged("CenterHeros");
            }
        }

        //手中的建筑牌
        ObservableCollection<Building> _pocketBuildings;
        public ObservableCollection<Building> PocketBuildings
        {
            get
            {
                return _pocketBuildings;
            }

            set
            {
                _pocketBuildings = value;
                RaisePropertyChanged("PocketBuildings");
            }
        }
        #endregion

        #region 控制中间控件显示的bool
        //中间多选的BUC是否显示
        bool _IsCenterBuildingMultiVisable;
        public bool IsCenterBuildingMultiVisable
        {
            get
            {
                return _IsCenterBuildingMultiVisable;
            }

            set
            {
                _IsCenterBuildingMultiVisable = value;
                RaisePropertyChanged("IsCenterBuildingMultiVisable");
            }
        }

        //中间单选的BUC是否显示
        bool _isCenterBuildingVisable;
        public bool IsCenterBuildingVisable
        {
            get
            {
                return _isCenterBuildingVisable;
            }

            set
            {
                _isCenterBuildingVisable = value;
                RaisePropertyChanged("IsCenterBuildingVisable");
            }
        }

        //中间单选的HUC是否显示
        bool _isCenterHeroVisable;
        public bool IsCenterHeroVisable
        {
            get
            {
                return _isCenterHeroVisable;
            }

            set
            {
                _isCenterHeroVisable = value;
                RaisePropertyChanged("IsCenterHeroVisable");
            }
        }

        //中间单选的PUC是否显示
        bool _isCenterPlayerVisable;
        public bool IsCenterPlayerVisable
        {
            get
            {
                return _isCenterPlayerVisable;
            }

            set
            {
                _isCenterPlayerVisable = value;
                RaisePropertyChanged("IsCenterPlayerVisable");
            }
        }

        //中间用户的BUC是否显示
        bool _isCenterBuildingPocketVisable;
        public bool IsCenterBuildingPocketVisable
        {
            get
            {
                return _isCenterBuildingPocketVisable;
            }

            set
            {
                _isCenterBuildingPocketVisable = value;
                RaisePropertyChanged("IsCenterBuildingPocketVisable");
            }
        }
        #endregion

        //中间单选控件选中的index
        int _index;
        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                _index = value;
                RaisePropertyChanged("Index");
            }
        }

        //当前中央控件式哪一步在显示
        public int Step { get; set; }

        #region 中间控件显示的提示
        //单选角色时显示的提示
        public string[] SelectHeroText { get; set; }
        //单选玩家显示的提示
        public string[] SelectPlayerText { get; set; }
        //中间的提示，用于绑定
        string _centerText;
        public string CenterText
        {
            get
            {
                return _centerText;
            }

            set
            {
                _centerText = value;
                RaisePropertyChanged("CenterText");
            }
        }
        #endregion

        #region 处理接收的部分
        //接收消息的委托
        public delegate void Del(string a);
        Del del;

        //接收消息的函数
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
                    Console.WriteLine("game收到了：" + str);
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

        //处理game收到的信息
        private void DealReceive(string item)
        {

        }
        #endregion

        #region 中间按键按下的Cmd
        //中间选按下确认选中键的命令
        ICommand _selectMultiCmd;
        public ICommand SelectMultiCmd
        {
            get
            {
                return _selectMultiCmd;
            }

            set
            {
                _selectMultiCmd = value;
            }
        }
        //多选对应的操作
        public void SelectMulti(object o)
        {
            IsCenterBuildingMultiVisable = false;
            IEnumerable a = (IEnumerable)o;
            foreach (var item in a)
            {
                Building b = item as Building;
                Console.Write(b.Name);
                Console.WriteLine("");
            }
        }
        //中间取消键按下的命令
        ICommand _cancelSelectCmd;
        public ICommand CancelSelectCmd
        {
            get
            {
                return _cancelSelectCmd;
            }

            set
            {
                _cancelSelectCmd = value;
            }
        }
        //取消对应的操作
        public void CancelSelect()
        {
            switch(Step)
            {
                default:IsCenterBuildingMultiVisable = false;break;
            }
        }
        #endregion

        #region 控制台按下的Cmd
        //显示手牌的flag

        //控制台显示手牌按下
        ICommand _showHandCardsCmd;
        public ICommand ShowHandCardsCmd
        {
            get
            {
                return _showHandCardsCmd;
            }

            set
            {
                _showHandCardsCmd = value;
            }
        }
        //控制台显示手牌的操作
        public void ShowHandCards()
        {
            IsCenterBuildingPocketVisable = !IsCenterBuildingPocketVisable;
        }
        #endregion

        #region 测试
        ICommand _test1Cmd;
        public ICommand Test1Cmd
        {
            get
            {
                return _test1Cmd;
            }

            set
            {
                _test1Cmd = value;
            }
        }

        string _test1Text;
        public string Test1Text
        {
            get
            {
                return _test1Text;
            }

            set
            {
                _test1Text = value;
                RaisePropertyChanged("Test1Text");
            }
        }

        ICommand _test2Cmd;
        public ICommand Test2Cmd
        {
            get
            {
                return _test2Cmd;
            }

            set
            {
                _test2Cmd = value;
            }
        }

        string _test2Text;
        public string Test2Text
        {
            get
            {
                return _test2Text;
            }

            set
            {
                _test2Text = value;
                RaisePropertyChanged("Test2Text");
            }
        }

        ICommand _test3Cmd;
        public ICommand Test3Cmd
        {
            get
            {
                return _test3Cmd;
            }

            set
            {
                _test3Cmd = value;
            }
        }

        string _test3Text;
        public string Test3Text
        {
            get
            {
                return _test3Text;
            }

            set
            {
                _test3Text = value;
                RaisePropertyChanged("Test3Text");
            }
        }


        public void Test1()
        {
            IsCenterBuildingPocketVisable = !IsCenterBuildingPocketVisable;
        }

        public void Test2()
        {
            Index = -1;
        }

        public void Test3()
        {
            IsCenterHeroVisable = !IsCenterHeroVisable;
        }
        #endregion


        //构造函数
        public GameVM(int num)
        {
            #region 测试
            Test1Text = "测试按钮1";
            Test2Text = "测试按钮2";
            Test3Text = "测试按钮3";
            Test1Cmd = new RelayCommand(new Action(Test1));
            Test2Cmd = new RelayCommand(new Action(Test2));
            Test3Cmd = new RelayCommand(new Action(Test3));
            #endregion

            SelectMultiCmd = new RelayCommand(new Action<object>(SelectMulti));
            CancelSelectCmd = new RelayCommand(new Action(CancelSelect));
            ShowHandCardsCmd = new RelayCommand(new Action(ShowHandCards));
            SelectHeroText = new string[] { "", "请选择你想要的角色：", "请选择你要盖下的角色：", "请选择你要刺杀的角色：", "请选择你要偷的角色：" };
            SelectPlayerText = new string[] { "", "请选择换牌的玩家：", "请选择摧毁建筑的玩家：" };
            //del = new Del(DealReceivePre);
            //ThReceive = new Thread(ReceiveSocket);
            //ThReceive.IsBackground = true;
            //ThReceive.Start(App.NetCtrl.SocketClient);
            CenterBuildings = new ObservableCollection<Building>();
            CenterHeros = new ObservableCollection<Hero>();
            CenterPlayer = new ObservableCollection<GamePlayer>();
            PocketBuildings = new ObservableCollection<Building>();
            GamePlayerList = new ObservableCollection<GamePlayer>();
            for (int i = 0; i < num; i++)
            {
                GamePlayer p = new GamePlayer(i + 1, "", 0, 0);
                GamePlayerList.Add(p);
            }
            IsCenterBuildingMultiVisable = false;
            IsCenterBuildingVisable = false;
            IsCenterHeroVisable = false;
            IsCenterPlayerVisable = false;
            IsCenterBuildingPocketVisable = false;
            Index = -1;
            Step = -1;
            CenterText = SelectHeroText[2];
        }
    }
}
