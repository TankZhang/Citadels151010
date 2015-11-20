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

        //中间开局显示的CenterRoundStartUC是否显示
        bool _isCenterRoundStartVisible;
        public bool IsCenterRoundStartVisible
        {
            get
            {
                return _isCenterRoundStartVisible;
            }

            set
            {
                _isCenterRoundStartVisible = value;
                RaisePropertyChanged("IsCenterRoundStartVisible");
            }
        }

        //控制台结束当前回合是否显示
        bool _isRoundOver;
        public bool IsRoundOver
        {
            get
            {
                return _isRoundOver;
            }

            set
            {
                _isRoundOver = value;
                RaisePropertyChanged("IsRoundOver");
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
            Send("3|2|" + RNum + "|" + SNum + "|" + GamePlayerList[SNum - 1].Nick + "|" + ChatText + "|");
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
            //测试
            BattleLog += ("C2S：" + s + "*\n");

            App.NetCtrl.Send(s);
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
            //测试
            BattleLog += ("S2C：" + item + "\n");

            string[] strs = item.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs[0] != "3") { return; }
            switch (strs[1])
            {
                //回复数据
                case "1":
                    DealData(strs);
                    break;
                //聊天数据
                case "2":
                    DealChat(strs);
                    break;
                //英雄数据
                case "3":
                    DealHero(strs);
                    break;
                //回合相关
                case "4":
                    DealRound(strs);
                    break;
                //钱相关
                case "5":
                    DealMoney(strs);
                    break;
                //建筑相关
                case "6":
                    DealBuilding(strs);
                    break;
            }
        }

        #region 处理建筑相关的数据
        //处理建筑相关的数据
        private void DealBuilding(string[] strs)
        {
            switch (strs[2])
            {
                //选择拿的建筑牌
                case "1":
                    DealBuilding1(strs);
                    break;
                //建筑师多拿的两张建筑牌
                case "2":
                    DealBuilding2(strs);
                    break;
                //魔术师与牌堆交换得到的牌
                case "3":
                    DealBuilding3(strs);
                    break;
                //更新当前的手牌
                case "4":
                    DealBuilding4(strs);
                    break;
            }
        }

        //收到更新当前手牌的数据
        private void DealBuilding4(string[] strs)
        {
            PocketBuildings.Clear();
            for (int i = 3; i < strs.Length; i++)
            {
                PocketBuildings.Add(CardRes.Buildings[int.Parse(strs[i])]);
            }
        }

        //魔术师与牌堆交换得到的牌
        private void DealBuilding3(string[] strs)
        {
            for (int i = 3; i < strs.Length; i++)
            {
                PocketBuildings.Add(CardRes.Buildings[int.Parse(strs[i])]);
            }
        }

        //处理建筑师多拿的牌
        private void DealBuilding2(string[] strs)
        {
            int id = int.Parse(strs[3]);
            PocketBuildings.Add(CardRes.Buildings[id]);
            id = int.Parse(strs[4]);
            PocketBuildings.Add(CardRes.Buildings[id]);
        }

        //处理选择拿牌数据
        private void DealBuilding1(string[] strs)
        {
            //单选时候
            if (!(IsStepFinished[7] && IsStepFinished[10]))
            {
                if (!IsStepFinished[7])
                { Step = 7; }
                if (!IsStepFinished[10])
                { Step = 10; }
                CenterBuildings.Clear();
                for (int i = 3; i < strs.Length; i++)
                {
                    CenterBuildings.Add(CardRes.Buildings[int.Parse(strs[i])]);
                }
                IsCenterBuildingVisable = true;
            }
            //多选时候
            if (!(IsStepFinished[13] && IsStepFinished[15]))
            {
                if (!IsStepFinished[13])
                { Step = 13; }
                if (!IsStepFinished[15])
                { Step = 15; }
                CenterBuildings.Clear();
                for (int i = 3; i < strs.Length; i++)
                {
                    CenterBuildings.Add(CardRes.Buildings[int.Parse(strs[i])]);
                }
                IsCenterBuildingMultiVisable = true;
            }
        }


        #endregion

        //处理钱相关的数据
        private void DealMoney(string[] strs)
        {
            switch (strs[2])
            {
                //偷成功
                case "1":
                    //GamePlayerList[SNum - 1].Money = int.Parse(strs[3]);
                    break;
                //被偷成功
                case "2":
                    //GamePlayerList[SNum - 1].Money = 0;
                    break;
                //更新钱
                case "3":
                    GamePlayerList[int.Parse(strs[3]) - 1].Money = int.Parse(strs[4]);
                    break;

            }
        }

        #region 处理回合相关的数据

        //处理回合相关的数据。
        private void DealRound(string[] strs)
        {
            switch (strs[2])
            {
                //回合开始
                case "1":
                    DealRound1(strs);
                    break;
            }
        }

        //处理回合开始时
        private void DealRound1(string[] strs)
        {
            //点亮结束回合按键
            IsRoundOver = false;
            IsCenterRoundStartVisible = true;
            //初始化IsStepFinished
            for (int i = 0; i < IsStepFinished.Count; i++)
            {
                IsStepFinished[i] = true;
            }
            //按照关键牌将IsStepFinished置否
            foreach (Building item in GamePlayerList[SNum - 1].Buildings)
            {
                //天文台选择地区时可以从三个里面选
                if (item.Name == "天文台")
                {
                    IsStepFinished[10] = false;
                    continue;
                }
                //实验室可以丢弃一张手牌得到金币
                if (item.Name == "实验室")
                {
                    IsStepFinished[11] = false;
                    continue;
                }
                //图书馆选地区时可以保留两张地区牌
                if (item.Name == "图书馆")
                {
                    IsStepFinished[13] = false;
                    continue;
                }
                //检查可以发动铁匠铺
                if (item.Name == "铁匠铺")
                {
                    IsBlacksmithExist = true;
                    continue;
                }
            }
            if (!(IsStepFinished[10] || IsStepFinished[13]))
            {
                IsStepFinished[10] = true;
                IsStepFinished[13] = true;
                IsStepFinished[15] = false;
            }

            //按照角色将IsStepFinished置否
            switch (strs[3])
            {
                //刺客
                case "1":
                    IsStepFinished[3] = false;
                    if (IsStepFinished[10] && IsStepFinished[13] && IsStepFinished[15])
                    { IsStepFinished[7] = false; }
                    IsStepFinished[8] = false;
                    break;
                //盗贼
                case "2":
                    IsStepFinished[4] = false;
                    if (IsStepFinished[10] && IsStepFinished[13] && IsStepFinished[15])
                    { IsStepFinished[7] = false; }
                    IsStepFinished[8] = false;
                    break;
                //魔术师
                case "3":
                    IsStepFinished[5] = false;
                    if (IsStepFinished[10] && IsStepFinished[13] && IsStepFinished[15])
                    { IsStepFinished[7] = false; }
                    IsStepFinished[8] = false;
                    IsStepFinished[14] = false;
                    break;
                //国王
                case "4":
                    if (IsStepFinished[10] && IsStepFinished[13] && IsStepFinished[15])
                    { IsStepFinished[7] = false; }
                    IsStepFinished[8] = false;
                    break;
                //主教
                case "5":
                    if (IsStepFinished[10] && IsStepFinished[13] && IsStepFinished[15])
                    { IsStepFinished[7] = false; }
                    IsStepFinished[8] = false;
                    break;
                //商人
                case "6":
                    if (IsStepFinished[10] && IsStepFinished[13] && IsStepFinished[15])
                    { IsStepFinished[7] = false; }
                    IsStepFinished[8] = false;
                    break;
                //建筑师
                case "7":
                    if (IsStepFinished[10] && IsStepFinished[13] && IsStepFinished[15])
                    { IsStepFinished[7] = false; }
                    IsStepFinished[12] = false;
                    break;
                //军阀
                case "8":
                    if (IsStepFinished[10] && IsStepFinished[13] && IsStepFinished[15])
                    { IsStepFinished[7] = false; }
                    IsStepFinished[8] = false;
                    IsStepFinished[6] = false;
                    IsStepFinished[9] = false;
                    break;
            }
            RaisePropertyChanged("IsStepFinished");
        }

        #endregion

        //处理得到的英雄相关的数据
        private void DealHero(string[] strs)
        {
            switch (strs[2])
            {
                //选英雄的信息
                case "1":
                    CenterHeros.Clear();
                    for (int i = 3; i < strs.Length; i++)
                    {
                        CenterHeros.Add(CardRes.Heros[int.Parse(strs[i])]);
                    }
                    IsStepFinished[1] = false;
                    Step = 1;
                    IsCenterBuildingPocketVisable = false;
                    IsCenterHeroVisable = true;
                    return;
                //盖下英雄的信息
                case "2":
                    CenterHeros.Clear();
                    IsStepFinished[2] = false;
                    for (int i = 3; i < strs.Length; i++)
                    {
                        CenterHeros.Add(CardRes.Heros[int.Parse(strs[i])]);
                    }
                    Step = 2;
                    IsCenterBuildingPocketVisable = false;
                    Task t = new Task(() =>
                    {
                        Thread.Sleep(200);
                        IsCenterHeroVisable = true;
                    });
                    t.Start();
                    //IsCenterHeroVisable = true;
                    return;
                //被杀害
                case "3":
                    for (int i = 0; i < GamePlayerList[SNum - 1].Roles.Count; i++)
                    {
                        if (GamePlayerList[SNum - 1].Roles[i].Id == int.Parse(strs[3]))
                        {
                            GamePlayerList[SNum - 1].Roles.RemoveAt(i);
                        }
                    }
                    break;
                //返回来的可以偷的角色ID
                case "4":
                    CancelSelect();
                    Step = 4;
                    CenterHeros.Clear();
                    for (int i = 3; i < strs.Length; i++)
                    {
                        CenterHeros.Add(CardRes.Heros[int.Parse(strs[i])]);
                    }
                    IsCenterHeroVisable = true;
                    break;
            }
        }

        //处理聊天数据
        private void DealChat(string[] strs)
        {
            switch (strs[2])
            {
                //战报
                case "1":
                    DealBattleLog(strs);
                    break;
                //聊天信息
                case "2":
                    if (int.Parse(strs[3]) == SNum)
                    {
                        ChatLog += ("我：" + strs[4] + "\n");
                    }
                    else
                    {
                        ChatLog += (GamePlayerList[int.Parse(strs[3]) - 1].Nick + ":" + strs[4] + "\n");
                    }
                    break;
            }
        }

        #region 处理战报的过程
        //处理战报的详细过程
        private void DealBattleLog(string[] strs)
        {
            string s = "";
            switch (strs[4])
            {
                //英雄相关的战报
                case "1":
                    s = DealBattleLog1(strs);
                    break;
                //回合相关的战报
                case "2":
                    s = DealBattleLog2(strs);
                    break;
                //钱相关的战报
                case "3":
                    s = DealBattleLog3(strs);
                    break;
                //被杀害
                case "4":
                    s = "作为" + CardRes.Heros[int.Parse(strs[5])].Name + "被杀！";
                    break;
                //建筑相关
                case "5":
                    s = DealBattleLog5(strs);
                    break;

                default: s = "处理战报时收到了意外的值"; break;
            }
            if (int.Parse(strs[3]) == SNum)
            {
                BattleLog += ("我：" + s + "\n");
            }
            else
            {
                BattleLog += (GamePlayerList[int.Parse(strs[3]) - 1].Nick + ":" + s + "\n");
            }
        }

        //钱相关的战报
        private string DealBattleLog3(string[] strs)
        {
            string s = "";
            switch (strs[5])
            {
                //偷成功
                case "1":
                    s = "作为" + CardRes.Heros[int.Parse(strs[7])].Name + "被" + GamePlayerList[int.Parse(strs[6]) - 1].Nick + "偷了" + strs[8] + "个钱！";
                    GamePlayerList[int.Parse(strs[3]) - 1].Money = 0;
                    GamePlayerList[int.Parse(strs[6]) - 1].Money += (int.Parse(strs[8]));
                    break;
                //选择拿钱
                case "2":
                    s = "作为" + CardRes.Heros[int.Parse(strs[6])].Name + "选择了拿钱";
                    break;
                //通过建筑拿钱
                case "3":
                    int money = int.Parse(strs[6]);
                    GamePlayerList[int.Parse(strs[3]) - 1].Money += money;
                    s = "通过建筑拿了" + money + "个金币";
                    break;
                //作为商人多拿了一个钱
                case "4":
                    GamePlayerList[int.Parse(strs[3]) - 1].Money += 1;
                    s = "作为商人多得了1个金币";
                    break;
            }
            return s;
        }

        //英雄相关的战报
        private string DealBattleLog1(string[] strs)
        {
            int seatNum = int.Parse(strs[3]);
            switch (strs[5])
            {
                case "1":
                    return "正在选择角色";
                case "2":
                    if (seatNum != SNum)
                    { GamePlayerList[seatNum - 1].Roles.Add(CardRes.Heros[0]); }
                    return "选择了一个角色";
                case "3":
                    return "正在选择盖下一个角色";
                case "4":
                    return "盖下了一个角色";
                //杀害一个角色
                case "5":
                    return "作为刺客选择杀害" + CardRes.Heros[int.Parse(strs[6])].Name;
                //某个角色被偷了
                case "6":
                    return "作为盗贼选择偷" + CardRes.Heros[int.Parse(strs[6])].Name;
            }
            return "处理英雄相关的战报时收到了错误的信息！";
        }

        //回合相关的战报
        private string DealBattleLog2(string[] strs)
        {
            int seatNum = int.Parse(strs[3]);
            string s = "";
            switch (strs[5])
            {
                //回合开始
                case "1":
                    s = "作为" + CardRes.Heros[int.Parse(strs[6])].Name + "的回合开始";
                    if (seatNum != SNum)
                    { GamePlayerList[seatNum - 1].Roles[0] = CardRes.Heros[int.Parse(strs[6])]; }
                    break;
                //回合结束
                case "2":
                    s = "作为" + CardRes.Heros[int.Parse(strs[6])].Name + "的回合结束";
                    if (seatNum == SNum)
                    { GamePlayerList[seatNum - 1].Roles.Remove(CardRes.Heros[int.Parse(strs[6])]); }
                    else
                    { GamePlayerList[seatNum - 1].Roles.RemoveAt(0); }
                    break;
            }
            return s;
        }

        //建筑相关的战报
        private string DealBattleLog5(string[] strs)
        {
            string s = "";
            switch (strs[5])
            {
                //正在选择建筑
                case "1":
                    s = "作为" + CardRes.Heros[int.Parse(strs[6])].Name + "正在选择建筑";
                    break;
                //建筑了建筑
                case "2":
                    //单个建筑
                    if (strs[6] == "1")
                    {
                        s = "作为" + CardRes.Heros[int.Parse(strs[7])].Name + "建筑了" + CardRes.Buildings[int.Parse(strs[8])].Name;
                        GamePlayerList[int.Parse(strs[3]) - 1].Buildings.Add(CardRes.Buildings[int.Parse(strs[8])]);
                    }
                    //多个建筑
                    if (strs[6] == "2")
                    {
                        s = "作为" + CardRes.Heros[int.Parse(strs[7])].Name + "建筑了";
                        for (int i = 8; i < strs.Length; i++)
                        {
                            s += (CardRes.Buildings[int.Parse(strs[i])].Name + "、");
                            GamePlayerList[int.Parse(strs[3]) - 1].Buildings.Add(CardRes.Buildings[int.Parse(strs[i])]);
                        }
                    }
                    break;
                //开局选择了建筑
                case "3":
                    s = "开局选择了" + int.Parse(strs[6]) + "张建筑牌";
                    break;
                //作为建筑师多得了两张建筑牌
                case "4":
                    s = "作为建筑师多得两张建筑牌";
                    break;
                //作为魔术师与牌堆交换了n张牌
                case "5":
                    s = "作为魔术师与牌堆交换了" + int.Parse(strs[6]) + "张牌！";
                    break;
                //魔术师与玩家进行了交换
                case "6":
                    s="作为魔术师与"+GamePlayerList[int.Parse(strs[6])-1].Nick+"交换牌，得到了" +int.Parse(strs[7])+ "张牌";
                    break;
            }
            return s;
        }

        #endregion

        //处理返回的数据
        private void DealData(string[] strs)
        {
            switch (strs[2])
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
        //多选对应的操作，价格累加
        public void SelectMulti(object o)
        {
            IsCenterBuildingMultiVisable = false;
            if (IsStepFinished[Step]) { return; }
            IsStepFinished[Step] = true;
            string s = "";
            int price = 0;
            int num = 0;
            IEnumerable a = (IEnumerable)o;

            foreach (var item in a)
            {
                Building b = item as Building;
                s += (b.Id + "|");
                price += b.Price;
                num++;
            }
            switch (Step)
            {
                //建筑师建筑牌，判断数量，然后判断经济
                case 12:
                    if (num <= 3)
                    {
                        if (price <= GamePlayerList[SNum - 1].Money)
                        {
                            foreach (var item in a)
                            {
                                PocketBuildings.Remove((Building)item);
                            }
                            Send("3|6|" + RNum + "|" + SNum + "|" + "2|2|" + s);
                        }
                        else
                        {
                            MessageBox.Show("您没有足够的钱建筑这些建筑！");
                            IsStepFinished[Step] = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("建筑师最多同时建筑3张建筑！");
                        IsStepFinished[Step] = false;
                    }
                    RaisePropertyChanged("IsStepFinished");
                    break;
                //图书馆选择牌
                case 13:
                    Send("3|6|" + RNum + "|" + SNum + "|" + "3|" + s);
                    break;
                //魔术师与牌堆交换选择牌
                case 14:
                    IsStepFinished[5] = true;
                    foreach (var item in a)
                    {
                        PocketBuildings.Remove((Building)item);
                    }
                    Send("3|6|" + RNum + "|" + SNum + "|" + "4|" + s);
                    break;
                //天文台和图书馆同时用的时候的选择牌
                case 15:
                    Send("3|6|" + RNum + "|" + SNum + "|" + "3|" + s);
                    break;
                default: break;
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
            if (Index < 0) { return; }
            if (IsStepFinished[Step]) { return; }
            IsStepFinished[Step] = true;
            switch (Step)
            {
                case 1:
                    GamePlayerList[SNum - 1].Roles.Add(CenterHeros[Index]);
                    IsCenterHeroVisable = false;
                    Send("3|3|" + Step + "|" + RNum + "|" + SNum + "|" + CenterHeros[Index].Id + "|");
                    Index = -1;
                    break;
                case 2:
                    IsCenterHeroVisable = false;
                    Send("3|3|" + Step + "|" + RNum + "|" + SNum + "|" + CenterHeros[Index].Id + "|");
                    Index = -1;
                    break;
                //刺杀英雄的单选
                case 3:
                    IsCenterHeroVisable = false;
                    Send("3|3|3|" + RNum + "|" + SNum + "|" + CenterHeros[Index].Id + "|");
                    break;
                //偷取英雄的单选
                case 4:
                    IsCenterHeroVisable = false;
                    Send("3|3|5|" + RNum + "|" + SNum + "|" + CenterHeros[Index].Id + "|");
                    break;
                //魔术师与玩家换牌
                case 5:
                    IsCenterPlayerVisable = false;
                    Send("3|7|" + RNum + "|" + SNum + "|1|" + CenterPlayer[Index].SeatNum + "|");
                    break;
                case 6:
                    Console.WriteLine("您选择的玩家为" + CenterPlayer[Index].Nick);
                    IsCenterPlayerVisable = false;
                    break;
                //普通选择建筑
                case 7:
                    IsCenterBuildingVisable = false;
                    Send("3|6|" + RNum + "|" + SNum + "|" + "3|" + CenterBuildings[Index].Id + "|");
                    PocketBuildings.Add(CenterBuildings[Index]);
                    Index = -1;
                    break;
                //普通建设,先判断钱够不够，然后操作。
                case 8:
                    IsCenterBuildingVisable = false;
                    if (CenterBuildings[Index].Price <= GamePlayerList[SNum - 1].Money)
                    {
                        Send("3|6|" + RNum + "|" + SNum + "|" + "2|1|" + CenterBuildings[Index].Id + "|");
                        PocketBuildings.Remove(CenterBuildings[Index]);
                    }
                    else
                    {
                        IsStepFinished[Step] = false;
                        MessageBox.Show("您没有足够的钱建造此建筑");
                    }
                    Index = -1;
                    RaisePropertyChanged("IsStepFinished");
                    break;
                case 9:
                    break;
                //天文台选择建筑
                case 10:
                    IsCenterBuildingVisable = false;
                    Send("3|6|" + RNum + "|" + SNum + "|" + "3|" + CenterBuildings[Index].Id + "|");
                    PocketBuildings.Add(CenterBuildings[Index]);
                    Index = -1;
                    break;
                case 11:
                    Console.WriteLine("您单选的建筑为" + CenterBuildings[Index].Name);
                    IsCenterBuildingVisable = false;
                    break;
                default: break;
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
            Index = -1;
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
                case 13:
                case 14: IsCenterBuildingMultiVisable = false; break;
                default: Console.WriteLine("中间点击取消时出现了意外的Step!!"); break;
            }
            IsCenterBuildingPocketVisable = false;
        }

        //中间选择拿钱按下的命令
        ICommand _selectMoneyCmd;
        public ICommand SelectMoneyCmd
        {
            get
            {
                return _selectMoneyCmd;
            }

            set
            {
                _selectMoneyCmd = value;
            }
        }
        //中间点击拿钱的操作
        public void SelectMoney()
        {
            IsCenterRoundStartVisible = false;
            Send("3|5|" + RNum + "|" + SNum + "|" + "1|");
        }

        //中间选择拿牌的操作
        ICommand _selectBuildingCmd;
        public ICommand SelectBuildingCmd
        {
            get
            {
                return _selectBuildingCmd;
            }

            set
            {
                _selectBuildingCmd = value;
                RaisePropertyChanged("SelectBuildingCmd");
            }
        }
        //中间选择拿牌的操作
        public void SelectBuilding()
        {
            IsCenterRoundStartVisible = false;
            if (!(IsStepFinished[7] && IsStepFinished[13]))
            {
                Send("3|6|" + RNum + "|" + SNum + "|" + "1|1|");
            }
            if (!(IsStepFinished[10] && IsStepFinished[15]))
            {
                Send("3|6|" + RNum + "|" + SNum + "|" + "1|2|");
            }
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
            //IsCenterHeroVisable = false;
            IsCenterPlayerVisable = false;
            IsCenterBuildingVisable = false;
            IsCenterBuildingMultiVisable = false;
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
            CancelSelect();
            CenterBuildings.Clear();
            foreach (var item in PocketBuildings)
            {
                CenterBuildings.Add(item);
            }
            if (!IsStepFinished[8])
            {
                Step = 8;
                Task t8 = new Task(() =>
                {
                    Thread.Sleep(200);
                    IsCenterBuildingVisable = true;
                });
                t8.Start();
                //IsCenterBuildingVisable = true;
            }
            if (!IsStepFinished[12])
            {
                Step = 12;
                Task t12 = new Task(() =>
                {
                    Thread.Sleep(200);
                    IsCenterBuildingMultiVisable = true;
                });
                t12.Start();
                //IsCenterBuildingMultiVisable = true;
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
            CancelSelect();
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
            //先发送我要偷取的信息到服务器。
            Send("3|3|4|" + RNum + "|" + SNum + "|");
            //CancelSelect();
            //Step = 4;
            //IsCenterHeroVisable = true;
            //CenterHeros = new ObservableCollection<Hero>();
            //CardRes.Heros.ForEach(x => CenterHeros.Add(x));
            //CenterHeros.RemoveAt(0);
            //CenterHeros.RemoveAt(0);
            //CenterHeros.RemoveAt(0);
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
            CancelSelect();
            Step = 5;
            IsCenterPlayerVisable = true;
            CenterPlayer.Clear();
            GamePlayerList.ToList().ForEach(x => CenterPlayer.Add(x));
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
            CancelSelect();
            Step = 14;
            CenterBuildings.Clear();
            foreach (Building item in PocketBuildings)
            { CenterBuildings.Add(item); }
            IsCenterBuildingMultiVisable = true;
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
            CancelSelect();
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
            CancelSelect();
            CenterBuildings = PocketBuildings;
            Step = 11;
            IsCenterBuildingVisable = true;
        }

        //结束回合命令
        ICommand _roundOverCmd;
        public ICommand RoundOverCmd
        {
            get
            {
                return _roundOverCmd;
            }

            set
            {
                _roundOverCmd = value;
            }
        }
        //结束回合操作,将所有的IsStepFinished归为true；发回服务器结束回合命令
        public void RoundOver()
        {
            IsRoundOver = true;
            CancelSelect();
            //全true，将IsStepFinished
            for (int i = 0; i < IsStepFinished.Count; i++)
            {
                IsStepFinished[i] = true;
            }
            RaisePropertyChanged("IsStepFinished");
            //向服务器发送结束回合命令
            Send("3|4|" + RNum + "|" + SNum + "|1|");

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
            GamePlayerList[SNum - 1].Buildings.Add(CardRes.Buildings[int.Parse(TestText)]);
            TestText = "";
        }

        public void Test3()
        {
            PocketBuildings.Add(CardRes.Buildings[int.Parse(TestText)]);
            TestText = "";
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
            SelectMoneyCmd = new RelayCommand(new Action(SelectMoney));
            SelectBuildingCmd = new RelayCommand(new Action(SelectBuilding));
            RoundOverCmd = new RelayCommand(new Action(RoundOver));
            del = new Del(DealReceivePre);
            ThReceive = new Thread(ReceiveSocket);
            ThReceive.IsBackground = true;
            ThReceive.Start(App.NetCtrl.SocketClient);
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
            IsCenterRoundStartVisible = false;
            IsRoundOver = true;
            ChatText = "";
            ChatLog = "";
            BattleLog = "\n\n\n\n\n\n\n\n这里是战报实时显示：\n";
            Index = -1;
            Step = -1;
            IsStepFinished = new ObservableCollection<bool>(new List<bool> { true,true,true,true,true, true,
                true, true, true, true, true, true, true, true,true,true });

            Send("3|1|1|" + RNum + "|" + SNum + "|");

            #region 测试
            Test1Text = "模拟接收";
            Test2Text = "桌面牌加入ID";
            Test3Text = "手牌加入ID";
            Test1Cmd = new RelayCommand(new Action(Test1));
            Test2Cmd = new RelayCommand(new Action(Test2));
            Test3Cmd = new RelayCommand(new Action(Test3));

            //PocketBuildings.Add(CardRes.Buildings[12]);
            //PocketBuildings.Add(CardRes.Buildings[14]);
            //PocketBuildings.Add(CardRes.Buildings[17]);
            //PocketBuildings.Add(CardRes.Buildings[19]);

            //CenterBuildings.Clear();

            //foreach (var item in PocketBuildings)
            //{
            //    CenterBuildings.Add(item);
            //}

            //IsCenterBuildingMultiVisable = true;

            //#region 默认卡牌
            //GamePlayerList[0].Nick = "默认1";
            //GamePlayerList[1].Nick = "默认2";
            //GamePlayerList[2].Nick = "默认3";
            //GamePlayerList[0].Buildings.Add(CardRes.Buildings[12]);
            //GamePlayerList[0].Buildings.Add(CardRes.Buildings[14]);
            //GamePlayerList[1].Buildings.Add(CardRes.Buildings[17]);
            //GamePlayerList[1].Buildings.Add(CardRes.Buildings[19]);
            //GamePlayerList[2].Buildings.Add(CardRes.Buildings[24]);
            //GamePlayerList[2].Buildings.Add(CardRes.Buildings[29]);
            //PocketBuildings.Add(CardRes.Buildings[34]);
            //PocketBuildings.Add(CardRes.Buildings[39]);
            //#endregion

            #endregion
        }
    }
}
