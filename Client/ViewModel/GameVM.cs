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
        public int SNum { get; set; }
        public int RNum { get; set; }
        public CardRes CardRes { get; set; }
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
                IsBlacksmithExist = false;
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

        #region 流程控制相关的Index，Step，IsStepFinished。
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
        int _step;
        public int Step
        {
            get
            {
                return _step;
            }

            set
            {
                _step = value;
                RaisePropertyChanged("Step");
            }
        }

        //每一步是否完成的bool表示    
        ObservableCollection<bool> _isStepFinished;
        public ObservableCollection<bool> IsStepFinished
        {
            get
            {
                return _isStepFinished;
            }

            set
            {
                _isStepFinished = value;
                RaisePropertyChanged("IsStepFinished");
            }
        }
        #endregion

        #region 右侧聊天控制
        //窗口中输入的text
        string _chatText;
        public string ChatText
        {
            get
            {
                return _chatText;
            }

            set
            {
                _chatText = value;
                RaisePropertyChanged("ChatText");
            }
        }

        //textlog绑定的
        string _chatLog;
        public string ChatLog
        {
            get
            {
                return _chatLog;
            }

            set
            {
                _chatLog = value;
                RaisePropertyChanged("ChatLog");
            }
        }

        //发送命令
        ICommand _chatCmd;
        public ICommand ChatCmd
        {
            get
            {
                return _chatCmd;
            }

            set
            {
                _chatCmd = value;
            }
        }

        //发送操作
        public void Chat()
        {
            if (ChatText == "") { return; }
            Send("3|2|" + RNum + "|" + SNum + "|" + GamePlayerList[SNum - 1].Nick+"|"+ChatText+"|");
            //ChatLog += (GamePlayerList[SNum - 1].Nick+":"+ChatText+"\n");
            ChatText = "";
        }

        //窗口中显示的战报
        string _battleLog;
        public string BattleLog
        {
            get
            {
                return _battleLog;
            }

            set
            {
                _battleLog = value;
                RaisePropertyChanged("BattleLog");
            }
        }

        #endregion

        #region 特殊建筑处理
        //检查列表中有没有特定的建筑牌
        public bool IsExist(ObservableCollection<Building> buildings, int id)
        {
            List<Building> Bs = buildings.ToList<Building>();
            if (Bs.FindIndex(s => s.Id == id) > -1)
            { return true; }
            else { return false; }
        }

        //铁匠铺可不可以用
        bool _isBlacksmithExist;
        public bool IsBlacksmithExist
        {
            get
            {
                return _isBlacksmithExist;
            }

            set
            {
                _isBlacksmithExist = value;
                RaisePropertyChanged("IsBlacksmithExist");
            }
        }

        #endregion

        #region 处理发送接收的部分
        //发送消息的函数
        public void Send(string s)
        {
            BattleLog += ("C2S：" + s+ "*\n");
            //App.NetCtrl.Send(s);
        }

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
            BattleLog += ("S2C：" + s + "\n");
            string[] ss = s.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in ss)
            {
                DealReceive(item);
            }
        }

        //处理game收到的信息
        private void DealReceive(string item)
        {
            string[] strs = item.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs[0] != "3") { return; }
            switch(strs[1])
            {
                //回复数据
                case "1":DealData(strs);
                    break;
                //聊天数据
                case "2":DealChat(strs);
                    break;

            }
        }

        //处理聊天数据
        private void DealChat(string[] strs)
        {
            if(int.Parse(strs[2])==SNum)
            {
                ChatLog += ("我：" + strs[3] + "\n");
            }
            else
            {
                ChatLog += (GamePlayerList[int.Parse(strs[2]) - 1].Nick + ":" + strs[3] + "\n");
            }
        }

        //处理返回的数据
        private void DealData(string[] strs)
        {
            switch(strs[2])
            {
                //此处为回复的是初始数据
                case "1":
                    PocketBuildings.Add(CardRes.Buildings[int.Parse(strs[3])]);
                    PocketBuildings.Add(CardRes.Buildings[int.Parse(strs[4])]);
                    foreach (var item in GamePlayerList)
                    {
                        item.Money = int.Parse(strs[5]);
                    }
                    for (int i = 6; i < strs.Length; i++)
                    {
                        GamePlayerList[i - 6].Nick = strs[i];
                    }
                    break;
            }
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
            if (IsStepFinished[Step]) { return; }
            IsStepFinished[Step] = true;
            if (Step == 14) IsStepFinished[5] = true;
            RaisePropertyChanged("IsStepFinished");
            IEnumerable a = (IEnumerable)o;
            foreach (var item in a)
            {
                Building b = item as Building;
                Console.Write(b.Name);
                Console.WriteLine("");
            }
        }

        //中间list单选点击时的命令
        ICommand _selectCmd;
        public ICommand SelectCmd
        {
            get
            {
                return _selectCmd;
            }

            set
            {
                _selectCmd = value;
            }
        }
        //中间list单选点击时的操作
        public void Select()
        {
            switch (Step)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    if (Index < 0) { return; }
                    if (IsStepFinished[Step]) { return; }
                    IsStepFinished[Step] = true;
                    Console.WriteLine("您选择的角色为" + CenterHeros[Index].Name);
                    CancelSelect();
                    break;
                case 5:
                case 6:
                    if (Index < 0) { return; }
                    if (IsStepFinished[Step]) { return; }
                    IsStepFinished[Step] = true;
                    Console.WriteLine("您选择的玩家为" + CenterPlayer[Index].Nick);
                    CancelSelect();
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    if (Index < 0) { return; }
                    if (IsStepFinished[Step]) { return; }
                    IsStepFinished[Step] = true;
                    RaisePropertyChanged("IsStepFinished");
                    Console.WriteLine("您单选的建筑为" + CenterBuildings[Index].Name);
                    CancelSelect();
                    break;
                default: return;
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
            switch (Step)
            {
                case 1:
                case 2:
                case 3:
                case 4: IsCenterHeroVisable = false; break;
                case 5:
                case 6: IsCenterPlayerVisable = false; break;
                case 7:
                case 8:
                case 9:
                case 10:
                case 11: IsCenterBuildingVisable = false; break;
                case 12:
                case 13: IsCenterBuildingMultiVisable = false; break;
                default: Console.WriteLine("中间点击取消时出现了意外的Step!!"); break;
            }
            Index = -1;
        }

        #endregion

        #region 控制台按下的Cmd
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

        //控制台我要建设命令
        ICommand _buildCmd;
        public ICommand BuildCmd
        {
            get
            {
                return _buildCmd;
            }

            set
            {
                _buildCmd = value;
            }
        }
        //我要建设函数
        public void Build()
        {
            if (!IsStepFinished[8])
            {
                Step = 8;
                CenterBuildings = PocketBuildings;
                IsCenterBuildingVisable = true;
            }
            if (!IsStepFinished[12])
            {
                Step = 12;
                CenterBuildings = PocketBuildings;
                IsCenterBuildingMultiVisable = true;
            }
        }

        //我要刺杀命令
        ICommand _killCmd;
        public ICommand KillCmd
        {
            get
            {
                return _killCmd;
            }

            set
            {
                _killCmd = value;
            }
        }
        //我要刺杀函数
        public void Kill()
        {
            Step = 3;
            IsCenterHeroVisable = true;
            CenterHeros = new ObservableCollection<Hero>();
            CardRes.Heros.ForEach(x => CenterHeros.Add(x));
            CenterHeros.RemoveAt(0);
            CenterHeros.RemoveAt(0);
        }

        //我要偷取命令
        ICommand _stoleCmd;
        public ICommand StoleCmd
        {
            get
            {
                return _stoleCmd;
            }

            set
            {
                _stoleCmd = value;
            }
        }
        //我要偷取函数
        public void Stole()
        {
            Step = 4;
            IsCenterHeroVisable = true;
            CenterHeros = new ObservableCollection<Hero>();
            CardRes.Heros.ForEach(x => CenterHeros.Add(x));
            CenterHeros.RemoveAt(0);
            CenterHeros.RemoveAt(0);
            CenterHeros.RemoveAt(0);
        }

        //我要与玩家交换命令
        ICommand _swapWithPlayerCmd;
        public ICommand SwapWithPlayerCmd
        {
            get
            {
                return _swapWithPlayerCmd;
            }

            set
            {
                _swapWithPlayerCmd = value;
            }
        }
        //我要与玩家交换函数
        public void SwapWithPlayer()
        {
            Step = 5;
            IsCenterPlayerVisable = true;
            CenterPlayer = new ObservableCollection<GamePlayer>();
            GamePlayerList.ToList<GamePlayer>().ForEach(x => CenterPlayer.Add(x));
            CenterPlayer.RemoveAt(SNum - 1);
        }

        //我要与牌堆交换命令
        ICommand _swapWithCardsCmd;
        public ICommand SwapWithCardsCmd
        {
            get
            {
                return _swapWithCardsCmd;
            }

            set
            {
                _swapWithCardsCmd = value;
            }
        }
        //与牌堆交换函数
        public void SwapWithCards()
        {
            Step = 14;
            IsCenterBuildingMultiVisable = true;
            CenterBuildings = PocketBuildings;
        }

        //我要摧毁命令
        ICommand _destroyCmd;
        public ICommand DestroyCmd
        {
            get
            {
                return _destroyCmd;
            }

            set
            {
                _destroyCmd = value;
            }
        }
        //我要摧毁操作
        public void Destroy()
        {
            Step = 6;
            IsCenterPlayerVisable = true;
            CenterPlayer = new ObservableCollection<GamePlayer>();
            GamePlayerList.ToList<GamePlayer>().ForEach(x => CenterPlayer.Add(x));
            CenterPlayer.RemoveAt(SNum - 1);
        }

        //发动铁匠铺命令
        ICommand _blacksmithCmd;
        public ICommand BlacksmithCmd
        {
            get
            {
                return _blacksmithCmd;
            }

            set
            {
                _blacksmithCmd = value;
            }
        }

        //发动铁匠铺函数
        public void Blacksmith()
        {
            IsBlacksmithExist = false;
            Console.WriteLine("你发动了铁匠铺");
        }

        //发动实验室命令
        ICommand _laboratoryCmd;
        public ICommand LaboratoryCmd
        {
            get
            {
                return _laboratoryCmd;
            }

            set
            {
                _laboratoryCmd = value;
            }
        }

        //发动实验室函数
        public void Laboratory()
        {
            CenterBuildings = PocketBuildings;
            Step = 11;
            IsCenterBuildingVisable = true;
        }
        #endregion

        #region 测试

        #region 测试的Cmd
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
        #endregion

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
        
        string _testText;
        public string TestText
        {
            get
            {
                return _testText;
            }

            set
            {
                _testText = value;
                RaisePropertyChanged("TestText");
            }
        }

        public void Test1()
        {
            DealReceivePre(TestText);
            TestText = "";
        }

        public void Test2()
        {
        }

        public void Test3()
        {
        }
        #endregion

        
        //构造函数
        public GameVM(int num, int rNum, int sNum)
        {
            SNum = sNum;
            RNum = rNum;
            SelectMultiCmd = new RelayCommand(new Action<object>(SelectMulti));
            CancelSelectCmd = new RelayCommand(new Action(CancelSelect));
            ShowHandCardsCmd = new RelayCommand(new Action(ShowHandCards));
            SelectCmd = new RelayCommand(new Action(Select));
            BuildCmd = new RelayCommand(new Action(Build));
            KillCmd = new RelayCommand(new Action(Kill));
            StoleCmd = new RelayCommand(new Action(Stole));
            SwapWithPlayerCmd = new RelayCommand(new Action(SwapWithPlayer));
            SwapWithCardsCmd = new RelayCommand(new Action(SwapWithCards));
            DestroyCmd = new RelayCommand(new Action(Destroy));
            BlacksmithCmd = new RelayCommand(new Action(Blacksmith));
            LaboratoryCmd = new RelayCommand(new Action(Laboratory));
            ChatCmd = new RelayCommand(new Action(Chat));
            del = new Del(DealReceivePre);
            ThReceive = new Thread(ReceiveSocket);
            ThReceive.IsBackground = true;
            //ThReceive.Start(App.NetCtrl.SocketClient);
            CardRes = new CardRes();
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
            ChatText = "";
            ChatLog = "";
            BattleLog = "\n\n\n\n\n\n\n\n这里是战报实时显示：\n";
            Index = -1;
            Step = 8;
            IsStepFinished = new ObservableCollection<bool>(new List<bool> { true,false,false,false,false, false,
                false, false, false, false, false, false, true, false,false });

            Send("3|1|1|"+RNum+"|"+SNum+"|");

            #region 测试
            Test1Text = "测试接收";
            Test2Text = "测试按钮2";
            Test3Text = "测试按钮3";
            Test1Cmd = new RelayCommand(new Action(Test1));
            Test2Cmd = new RelayCommand(new Action(Test2));
            Test3Cmd = new RelayCommand(new Action(Test3));
            #endregion
        }
    }
}
