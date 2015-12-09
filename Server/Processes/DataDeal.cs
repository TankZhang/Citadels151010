using Server.Cards;
using Server.Datas;
using Server.Net;
using Server.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Processes
{
    public static class DataDeal
    {
        //预处理，去掉‘*’的影响，以免数据相连造成异常
        public static void DealDataPre(DataCenter dataCenter, Socket socket, string str)
        {

            string[] strs = str.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in strs)
            {
                DealData(dataCenter, socket, item);
            }
        }

        public static void DealData(DataCenter dataCenter, Socket socket, string str)
        {
            string[] strs = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            //此处处理客户端传过来的信息
            //1信息。2大厅。
            switch (strs[0])
            {
                case "1":
                    //1连接请求。2注册信息。3登录信息。
                    PlayerData playerData = InfoDeal.InfoDataDeal(App.vM.SQLCtrl, socket, strs);
                    if (playerData.Status.Contains("1|1|1|"))
                    { break; }
                    if (playerData.Status.Contains("1|3|1|"))
                    { SignDeal(dataCenter, socket, playerData); }
                    NetCtrl.Send(socket, playerData.Status);
                    if (playerData.Status.Contains("1|3|1|"))
                    { playerData.Status = "Online"; dataCenter.LobbyPlayerList.Add(playerData); }
                    break;
                //处理大厅信息
                case "2":
                    DealLobbyData(dataCenter, socket, strs); break;
                case "3":
                    //1请求数据。2聊天
                    DealGameData(dataCenter, socket, strs); break;
                //测试信息
                case "9":
                    DealTestData(dataCenter, socket, strs); break;
            }
        }

        //处理测试信息
        private static void DealTestData(DataCenter dataCenter, Socket socket, string[] strs)
        {
            int rNum = int.Parse(strs[1]);
            int sNum = int.Parse(strs[2]);
            switch(strs[3])
            {
                //加一张手牌
                case "1":
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.Add(dataCenter.CardRes.OrderBuildings[int.Parse(strs[4]) - 1]);
                    SendToPlayer(dataCenter, rNum, sNum, "3|6|5|" + int.Parse(strs[4]) + "|");
                    break;
                //加钱
                case "2":
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money += (int.Parse(strs[4]));
                    SendToRoom(dataCenter,rNum,"3|2|1|"+sNum+"|3|3|"+ int.Parse(strs[4])+"|");
                    break;
                //加一个建筑
                case "3":
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].TableB.Add(dataCenter.CardRes.OrderBuildings[int.Parse(strs[4]) - 1]);
                    SendToRoom(dataCenter,rNum,"3|2|1|"+sNum+"|5|2|1|8|"+int.Parse(strs[4])+"|");
                    break;
            }
        }

        #region 处理游戏信息(strs[0]="3")
        private static void DealGameData(DataCenter dataCenter, Socket socket, string[] strs)
        {
            switch (strs[1])
            {
                //处理请求数据信息
                case "1":
                    DealGameInfo(dataCenter, socket, strs);
                    break;
                //处理聊天信息
                case "2":
                    SendToRoom(dataCenter, int.Parse(strs[2]), "3|2|2|" + int.Parse(strs[3]) + "|" + strs[5] + "|");
                    break;
                //处理英雄相关的信息
                case "3":
                    DealGameHero(dataCenter, strs);
                    break;
                //处理回合相关的信息
                case "4":
                    DealGameRound(dataCenter, strs);
                    break;
                //处理钱相关的信息
                case "5":
                    DealGameData5(dataCenter, strs);
                    break;
                //处理建筑相关的信息
                case "6":
                    DealGameBuilding(dataCenter, strs);
                    break;
                //处理玩家相关的信息
                case "7":
                    DealGamePLayer(dataCenter, strs);
                    break;
            }
        }

        //处理玩家相关的信息
        private static void DealGamePLayer(DataCenter dataCenter, string[] strs)
        {
            int rNum = int.Parse(strs[2]);
            int sNum = int.Parse(strs[3]);
            switch (strs[4])
            {
                //魔术师选择与某个玩家交换手牌
                case "1":
                    int sNum1 = int.Parse(strs[5]);
                    List<Building> bTemp = new List<Building>();
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.ForEach(s => bTemp.Add(s));
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.Clear();
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum1 - 1].PocketB.ForEach(s => dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.Add(s));
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum1 - 1].PocketB.Clear();
                    bTemp.ForEach(s => dataCenter.RoomDataDic[rNum].PlayerDataList[sNum1 - 1].PocketB.Add(s));
                    bTemp = null;
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|6|" + sNum1 + "|" + dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.Count + "|");
                    string s1 = "3|6|4|";
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.ForEach(s => s1 += (s.ID + "|"));
                    SendToPlayer(dataCenter, rNum, sNum, s1);
                    s1 = "3|6|4|";
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum1 - 1].PocketB.ForEach(s => s1 += (s.ID + "|"));
                    SendToPlayer(dataCenter, rNum, sNum1, s1);
                    break;
            }
        }

        #region 处理建筑相关的信息
        //处理建筑相关的信息
        private static void DealGameBuilding(DataCenter dataCenter, string[] strs)
        {
            int rNum = int.Parse(strs[2]);
            int sNum = int.Parse(strs[3]);
            switch (strs[4])
            {
                //选择拿牌操作
                case "1":
                    DealGameBuilding1(dataCenter, rNum, sNum, strs);
                    break;
                //选择建设操作
                case "2":
                    DealGameBuilding2(dataCenter, rNum, sNum, strs);
                    break;
                //开局拿的建筑牌
                case "3":
                    DealGameBuilding3(dataCenter, rNum, sNum, strs);
                    break;
                //魔术师与牌堆交换的牌
                case "4":
                    DealGameBuilding4(dataCenter, rNum, sNum, strs);
                    break;
                //军阀摧毁某个玩家的某张牌
                case "5":
                    DealGameBuilding5(dataCenter, rNum, sNum, strs);
                    break;
                //发动铁匠铺
                case "6":
                    DealGameBuilding6(dataCenter, rNum, sNum, strs);
                    break;
                //发动实验室
                case "7":
                    DealGameBuilding7(dataCenter, rNum, sNum, strs);
                    break;
                //回合结束时候丢弃的手牌
                case "8":
                    DealGameBuilding8(dataCenter, rNum, sNum, strs);
                    break;
                //墓地相关的内容
                case "9":
                    DealGameBuilding9(dataCenter, rNum, sNum, strs);
                    break;
            }

        }

        //墓地相关的内容。如果是使用墓地，则将刚才牌堆最后一张给他，并减少他的钱，并群发战报，如果放弃则群发战报
        private static void DealGameBuilding9(DataCenter dataCenter, int rNum, int sNum, string[] strs)
        {
            switch (strs[5])
            {
                case "1":
                    int id = dataCenter.RoomDataDic[rNum].BackB[dataCenter.RoomDataDic[rNum].BackB.Count - 1].ID;
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.Add(dataCenter.CardRes.OrderBuildings[id - 1]);
                    dataCenter.RoomDataDic[rNum].BackB.RemoveAt(dataCenter.RoomDataDic[rNum].BackB.Count - 1);
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money -= 1;
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|10|2|" + id + "|");
                    break;
                case "2":
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|10|3|");
                    break;
            }
        }

        //回合结束时候丢弃的手牌，将其从某人手牌中拆掉，然后放到牌堆中。
        private static void DealGameBuilding8(DataCenter dataCenter, int rNum, int sNum, string[] strs)
        {
            int id;
            for (int i = 5; i < strs.Length; i++)
            {
                id = int.Parse(strs[i]);
                dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.RemoveAll(s => s.ID == id);
                dataCenter.RoomDataDic[rNum].BackB.Add(dataCenter.CardRes.OrderBuildings[id - 1]);
            }
        }

        //发动实验室时候，首先将牌拿回到牌堆，然后将对应玩家的手牌拿掉，然后加钱，然后群发战报
        private static void DealGameBuilding7(DataCenter dataCenter, int rNum, int sNum, string[] strs)
        {
            int id = int.Parse(strs[5]);
            dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.RemoveAll(b => b.ID == id);
            dataCenter.RoomDataDic[rNum].BackB.Add(dataCenter.CardRes.OrderBuildings[id - 1]);
            dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money++;
            SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|9|");
        }

        //发动铁匠铺时候的反应,减钱，然后给三张牌，然后发送给个人，群发战报
        private static void DealGameBuilding6(DataCenter dataCenter, int rNum, int sNum, string[] strs)
        {
            dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money -= 2;
            string s = "3|6|5|";
            for (int i = 0; i < 3; i++)
            {
                dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.Add(dataCenter.RoomDataDic[rNum].BackB[0]);
                s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
            }
            SendToPlayer(dataCenter, rNum, sNum, s);
            SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|8|");
        }

        //军阀摧毁某个玩家的某张牌,减掉应该减的钱和牌，将牌加入到牌堆中，并群发战报
        private static void DealGameBuilding5(DataCenter dataCenter, int rNum, int sNum, string[] strs)
        {
            int TargetSnum = int.Parse(strs[5]);
            int TargetID = int.Parse(strs[6]);
            int money = int.Parse(strs[7]);
            dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money -= money;
            dataCenter.RoomDataDic[rNum].PlayerDataList[TargetSnum - 1].TableB.RemoveAll(b => b.ID == TargetID);
            dataCenter.RoomDataDic[rNum].BackB.Add(dataCenter.CardRes.OrderBuildings[TargetID - 1]);
            SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|7|" + TargetSnum + "|" + TargetID + "|" + money + "|");
            //检查是否有人持有墓地并且钱的数目大于1并且不是军阀的。
            int index = dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(playerData => ((playerData.Money >= 1) && (playerData.TableB.Exists(b => b.ID == 65)) && (playerData.SNum != sNum)));
            if (index >= 0)
            {
                SendToRoom(dataCenter, rNum, "3|2|1|" + dataCenter.RoomDataDic[rNum].PlayerDataList[index].SNum + "|5|10|1|");
            }
        }

        //魔术师与牌堆交换的牌，将牌放入到牌堆中，然后牌堆中取出相应的牌
        private static void DealGameBuilding4(DataCenter dataCenter, int rNum, int sNum, string[] strs)
        {
            for (int i = 5; i < strs.Length; i++)
            {
                dataCenter.RoomDataDic[rNum].BackB.Add(dataCenter.CardRes.OrderBuildings[int.Parse(strs[i]) - 1]);
                dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.RemoveAll(b => b.ID == int.Parse(strs[i]));
            }
            string s = "3|6|3|";
            for (int i = 0; i < strs.Length - 5; i++)
            {
                dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.Add(dataCenter.RoomDataDic[rNum].BackB[0]);
                s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
            }
            NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, s);
            SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|5|" + (strs.Length - 5) + "|");
        }

        //开局拿的建筑牌,将tableB中的对应牌放入pocketB并且将tableB剩余牌放回backB
        private static void DealGameBuilding3(DataCenter dataCenter, int rNum, int sNum, string[] strs)
        {
            int id;
            switch (strs[5])
            {
                //选择的牌
                case "1":
                    for (int i = 6; i < strs.Length; i++)
                    {
                        id = int.Parse(strs[i]);
                        dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.Add(dataCenter.CardRes.OrderBuildings[id - 1]);
                    }
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|" + "5|3|" + (strs.Length - 6) + "|");
                    break;
                //没有选择的牌
                case "2":
                    for (int i = 6; i < strs.Length; i++)
                    {
                        id = int.Parse(strs[i]);
                        dataCenter.RoomDataDic[rNum].BackB.Add(dataCenter.CardRes.OrderBuildings[id - 1]);
                    }
                    break;

            }
        }

        //选择建设操作
        private static void DealGameBuilding2(DataCenter dataCenter, int rNum, int sNum, string[] strs)
        {
            switch (strs[5])
            {
                //普通建设
                case "1":
                    int id1 = int.Parse(strs[6]);
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].TableB.Add(dataCenter.CardRes.OrderBuildings[id1 - 1]);
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.RemoveAll(s => s.ID == id1);
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money -= dataCenter.CardRes.OrderBuildings[id1 - 1].Price;
                    SendToRoom(dataCenter, rNum, "3|5|3|" + sNum + "|" + dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money + "|");
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|2|1|" + dataCenter.RoomDataDic[rNum].FinishCount + "|" + id1 + "|");
                    break;
                //建设多张牌
                case "2":
                    string s2 = "";
                    int money2 = 0;
                    int id2 = -1;
                    for (int i = 6; i < strs.Length; i++)
                    {
                        id2 = int.Parse(strs[i]);
                        dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].TableB.Add(dataCenter.CardRes.OrderBuildings[id2 - 1]);
                        dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.RemoveAll(s => s.ID == id2);
                        money2 += dataCenter.CardRes.OrderBuildings[id2 - 1].Price;
                        s2 += (id2 + "|");
                    }
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money -= money2;
                    SendToRoom(dataCenter, rNum, "3|5|3|" + sNum + "|" + dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money + "|");
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|2|2|" + dataCenter.RoomDataDic[rNum].FinishCount + "|" + s2);
                    break;
            }
        }

        //选择拿牌操作
        private static void DealGameBuilding1(DataCenter dataCenter, int rNum, int sNum, string[] strs)
        {
            string s = "3|6|1|";
            //从两个里面选择
            if (strs[5] == "1")
            {
                s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, s);
                SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|1|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
            }
            //从三个里面选择
            if (strs[5] == "2")
            {
                s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, s);
                SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|1|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
            }
        }

        #endregion

        //处理钱相关的信息
        private static void DealGameData5(DataCenter dataCenter, string[] strs)
        {
            int rNum = int.Parse(strs[2]);
            int sNum = int.Parse(strs[3]);
            switch (strs[4])
            {
                //选择拿钱
                case "1":
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money += 2;
                    SendToRoom(dataCenter, rNum, "3|5|3|" + sNum + "|" + dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money + "|");
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|3|2|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
                    break;
            }
        }

        //处理回合相关的信息
        private static void DealGameRound(DataCenter dataCenter, string[] strs)
        {
            int rNum = int.Parse(strs[2]);
            int sNum = int.Parse(strs[3]);
            switch (strs[4])
            {
                //回合结束，群发战报，然后通知下家开始新的回合
                case "1":
                    if(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum-1].TableB.Count>=8)
                    {
                        if (dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(p1 => p1.IsFirst) >=0)
                        {
                            if (dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(p2 => p2.IsSecond) < 0)
                                dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].IsSecond = true;
                        }
                        else
                        {
                            dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].IsFirst = true;
                        }
                    }
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|2|2|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
                    SendRoundStart(dataCenter, rNum);
                    break;
            }
        }

        #region 处理英雄相关的信息
        //处理英雄相关的信息
        private static void DealGameHero(DataCenter dataCenter, string[] strs)
        {
            int rNum = int.Parse(strs[3]);
            int sNum = int.Parse(strs[4]);
            switch (strs[2])
            {
                //选英雄的信息
                case "1":
                    DealHeroData1(dataCenter, strs);
                    break;
                //盖下英雄的信息
                case "2":
                    DealHeroData2(dataCenter, strs);
                    break;
                //英雄被杀的信息
                case "3":
                    DealGameHero3(dataCenter, strs);
                    break;
                //选择我要偷取
                case "4":
                    DealGameHero4(dataCenter, strs);
                    break;
                //角色被偷的信息
                case "5":
                    DealGameHero5(dataCenter, strs);
                    break;


            }
        }

        //角色被偷的信息
        private static void DealGameHero5(DataCenter dataCenter, string[] strs)
        {
            int rNum = int.Parse(strs[3]);
            int sNum = int.Parse(strs[4]);
            dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].IsStoling = true;
            int stoledNum = int.Parse(strs[5]);
            if (dataCenter.RoomDataDic[rNum].Hero2PlayerDic.Keys.Contains(stoledNum))
            {
                dataCenter.RoomDataDic[rNum].PlayerDataList[(dataCenter.RoomDataDic[rNum].Hero2PlayerDic[stoledNum]) - 1].StoledNum = stoledNum;
            }
            SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|1|6|" + stoledNum + "|");
        }

        //处理我要偷取的信息
        private static void DealGameHero4(DataCenter dataCenter, string[] strs)
        {
            int rNum = int.Parse(strs[3]);
            int sNum = int.Parse(strs[4]);
            string s = "3|3|4|";
            for (int i = 2; i < 8; i++)
            {
                //if (dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(p => p.KilledNum == (i+1)) < 0)
                //    s += ((i+1) + "|");
                if ((i + 1) != dataCenter.RoomDataDic[rNum].KilledNum)
                    s += ((i + 1) + "|");
            }
            NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, s);
        }

        //处理英雄被杀的信息
        private static void DealGameHero3(DataCenter dataCenter, string[] strs)
        {
            int rNum = int.Parse(strs[3]);
            int sNum = int.Parse(strs[4]);
            int id = int.Parse(strs[5]);
            SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|" + "1|5|" + id + "|");
            dataCenter.RoomDataDic[rNum].KilledNum = id;
            if (dataCenter.RoomDataDic[rNum].Hero2PlayerDic.Keys.Contains(id))
            {
                dataCenter.RoomDataDic[rNum].PlayerDataList[dataCenter.RoomDataDic[rNum].Hero2PlayerDic[id] - 1].KilledNum = id;
            }

        }

        //处理普通盖下英雄牌的操作
        private static void DealHeroData2(DataCenter dataCenter, string[] strs)
        {
            int rNum = int.Parse(strs[3]);
            int sNum = int.Parse(strs[4]);
            int id = int.Parse(strs[5]);
            int index = dataCenter.RoomDataDic[rNum].BackH.FindIndex(hero => hero.ID == id);
            dataCenter.RoomDataDic[rNum].BackH.RemoveAt(index);
            SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|1|4|");
            SendPickHero(dataCenter, rNum);
        }

        //处理普通选英雄的操作
        private static void DealHeroData1(DataCenter dataCenter, string[] strs)
        {
            int rNum = int.Parse(strs[3]);
            int sNum = int.Parse(strs[4]);
            int id = int.Parse(strs[5]);
            int index = dataCenter.RoomDataDic[rNum].BackH.FindIndex(hero => hero.ID == id);
            if (id == 5)
            { dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].IsBishop = true; }
            dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Roles.Add(dataCenter.RoomDataDic[rNum].BackH[index]);
            dataCenter.RoomDataDic[rNum].Hero2PlayerDic.Add(id, sNum);
            dataCenter.RoomDataDic[rNum].BackH.RemoveAt(index);
            SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|1|2|");
            dataCenter.RoomDataDic[rNum].FinishCount++;
            SendReverseHero(dataCenter, rNum);
        }
        #endregion

        //处理请求数据信息
        private static void DealGameInfo(DataCenter dataCenter, Socket socket, string[] strs)
        {
            switch (strs[2])
            {
                //收到的是请求初始数据的数据
                case "1":
                    int rNum = int.Parse(strs[3]);
                    int sNum = int.Parse(strs[4]);
                    string s = "3|1|1|";
                    foreach (var item in dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB)
                    {
                        s += (item.ID + "|");
                    }
                    s += "2|";
                    foreach (var item in dataCenter.RoomDataDic[rNum].PlayerDataList)
                    {
                        s += (item.Nick + "|");
                    }
                    NetCtrl.Send(socket, s);
                    //检查是否每个人都返回信息了，当全部返回信息之后，进行选择角色牌 
                    if (dataCenter.RoomDataDic[rNum].FinishCount < dataCenter.RoomDataDic[rNum].PlayerDataList.Count - 1)
                    {
                        dataCenter.RoomDataDic[rNum].FinishCount++;
                    }
                    else
                    {
                        dataCenter.RoomDataDic[rNum].FinishCount = 0;
                        SendPickHero(dataCenter, rNum);
                    }
                    break;
            }
        }

        #region rNum房间让某人盖下英雄

        //rNum房间让某人盖下英雄
        private static void SendReverseHero(DataCenter dataCenter, int rNum)
        {
            if (dataCenter.RoomDataDic[rNum].FinishCount == 1)
            { SendPickHero(dataCenter, rNum); return; }
            if (dataCenter.RoomDataDic[rNum].PlayerDataList.Count == 2)
            {
                SendReverseHeroCase1(dataCenter, rNum);
            }
            else
            {
                SendReverseHeroCase2(dataCenter, rNum);
            }
        }

        //玩家数量不为2时候发送的盖牌信息
        private static void SendReverseHeroCase2(DataCenter dataCenter, int rNum)
        {
            if (dataCenter.RoomDataDic[rNum].FinishCount == dataCenter.RoomDataDic[rNum].PlayerDataList.Count)
            {
                dataCenter.RoomDataDic[rNum].FinishCount = 0;
                SendRoundStart(dataCenter, rNum);
                return;
            }
            else
            {
                string s = "3|3|2|";
                foreach (var item in dataCenter.RoomDataDic[rNum].BackH)
                {
                    s += (item.ID + "|");
                }
                int index = dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(player => player.IsKing == true);
                int seatNum = (dataCenter.RoomDataDic[rNum].PlayerDataList[index].SNum + dataCenter.RoomDataDic[rNum].FinishCount - 1) % dataCenter.RoomDataDic[rNum].PlayerDataList.Count;
                if (seatNum == 0) seatNum = dataCenter.RoomDataDic[rNum].PlayerDataList.Count;
                NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[seatNum - 1].Socket, s);
                SendToRoom(dataCenter, rNum, "3|2|1|" + seatNum + "|1|3|");
                return;
            }
        }

        //玩家数量为2时候的发送盖牌信息
        private static void SendReverseHeroCase1(DataCenter dataCenter, int rNum)
        {
            if (dataCenter.RoomDataDic[rNum].FinishCount == 4)
            {
                dataCenter.RoomDataDic[rNum].FinishCount = 0;
                SendRoundStart(dataCenter, rNum);
                return;
            }
            else
            {
                string s = "3|3|2|";
                foreach (var item in dataCenter.RoomDataDic[rNum].BackH)
                {
                    s += (item.ID + "|");
                }
                int index = dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(player => player.IsKing == true);
                int seatNum = (dataCenter.RoomDataDic[rNum].PlayerDataList[index].SNum + ((dataCenter.RoomDataDic[rNum].FinishCount - 1) % 2)) % 2;
                if (seatNum == 0) seatNum = 2;
                NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[seatNum - 1].Socket, s);
                SendToRoom(dataCenter, rNum, "3|2|1|" + seatNum + "|1|3|");
                return;
            }
        }


        #endregion

        #region rNum号房间让人选英雄
        //rNum号房间让某人选英雄
        private static void SendPickHero(DataCenter dataCenter, int rNum)
        {
            if (dataCenter.RoomDataDic[rNum].FinishCount == 0)
            {
                //如果有人达到了第一个八个建筑
                int winSeatNum = dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(p => p.IsFirst);
                if (winSeatNum >= 0)
                {
                    string s = "3|4|3|";
                    foreach (var item in dataCenter.RoomDataDic[rNum].PlayerDataList)
                    {
                        s += (item.SNum+"|");
                        s += (item.Score+"|");
                    }
                    SendToRoom(dataCenter, rNum, s);
                    return;
                }
                dataCenter.RoomDataDic[rNum].BackH = CardRes.RandOrderHList(dataCenter.CardRes.OrderHeros);
                dataCenter.RoomDataDic[rNum].BackH.RemoveAt(0);
            }
            //当玩家数量为2时，直到finishCount为4时才停止，不是2时直到finishCount为count时候就停止开始游戏。
            if (dataCenter.RoomDataDic[rNum].PlayerDataList.Count == 2)
            {
                SendPickHeroCase1(dataCenter, rNum);
                return;
            }
            else
            {
                SendPickHeroCase2(dataCenter, rNum);
                return;
            }
        }

        //rNum号房间第次让某人选英雄，当人数不为2时
        private static void SendPickHeroCase2(DataCenter dataCenter, int rNum)
        {
            string s = "3|3|1|";
            foreach (var item in dataCenter.RoomDataDic[rNum].BackH)
            {
                s += (item.ID + "|");
            }
            int index = dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(player => player.IsKing == true);
            int seatNum = (dataCenter.RoomDataDic[rNum].PlayerDataList[index].SNum + dataCenter.RoomDataDic[rNum].FinishCount) % (dataCenter.RoomDataDic[rNum].PlayerDataList.Count);
            if (seatNum == 0) seatNum = dataCenter.RoomDataDic[rNum].PlayerDataList.Count;
            index = dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(player => player.SNum == seatNum);
            NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[index].Socket, s);
            SendToRoom(dataCenter, rNum, "3|2|1|" + seatNum + "|1|1|");
            return;
        }

        //rNum号房间第次让某人选英雄，当人数为2时
        private static void SendPickHeroCase1(DataCenter dataCenter, int rNum)
        {
            if (dataCenter.RoomDataDic[rNum].FinishCount < 4)
            {
                string s = "3|3|1|";
                foreach (var item in dataCenter.RoomDataDic[rNum].BackH)
                {
                    s += (item.ID + "|");
                }
                int index = dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(player => player.IsKing == true);
                int seatNum = (dataCenter.RoomDataDic[rNum].PlayerDataList[index].SNum + (dataCenter.RoomDataDic[rNum].FinishCount % 2)) % 2;
                if (seatNum == 0) seatNum = 2;
                NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[seatNum - 1].Socket, s);
                SendToRoom(dataCenter, rNum, "3|2|1|" + seatNum + "|1|1|");
                return;
            }
        }

        #endregion

        //回合开始
        private static void SendRoundStart(DataCenter dataCenter, int rNum)
        {
            //如果是最后一个结束，重置playerdata和int，移位王冠，并清空当前Hero2PlayerDic。
            if (dataCenter.RoomDataDic[rNum].FinishCount == 8)
            {
                SendToRoom(dataCenter, rNum, "3|4|2|");
                int kingSeat = dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(player => player.IsKing);
                dataCenter.RoomDataDic[rNum].KilledNum = -1;
                foreach (var playerdata in dataCenter.RoomDataDic[rNum].PlayerDataList)
                {
                    playerdata.IsKing = false;
                    playerdata.IsBishop = false;
                    playerdata.StoledNum = -1;
                    playerdata.IsStoling = false;
                    //playerdata.KilledNum = -1;
                }
                if (dataCenter.RoomDataDic[rNum].Hero2PlayerDic.Keys.Contains(4))
                {
                    dataCenter.RoomDataDic[rNum].PlayerDataList[dataCenter.RoomDataDic[rNum].Hero2PlayerDic[4] - 1].IsKing = true;
                }
                else
                {
                    dataCenter.RoomDataDic[rNum].PlayerDataList[(kingSeat + 1) % (dataCenter.RoomDataDic[rNum].PlayerDataList.Count)].IsKing = true;
                }
                dataCenter.RoomDataDic[rNum].Hero2PlayerDic.Clear();

                dataCenter.RoomDataDic[rNum].FinishCount = 0;
                SendPickHero(dataCenter, rNum);
                return;
            }
            dataCenter.RoomDataDic[rNum].FinishCount++;
            //如果有玩家选了叫到的角色
            if (dataCenter.RoomDataDic[rNum].Hero2PlayerDic.Keys.Contains(dataCenter.RoomDataDic[rNum].FinishCount))
            {
                #region 测试
                string ss;
                foreach (var item in dataCenter.RoomDataDic[rNum].PlayerDataList)
                {
                    ss = "3|2|3|";
                    ss += (item.Nick + ":");
                    foreach (var pB in item.PocketB)
                    {
                        ss += (pB.Name + "-");
                    }
                    ss += item.Money;
                    ss += "\n";
                    foreach (var tB in item.TableB)
                    {
                        ss += (tB.Name + "-");
                    }
                    ss += "\n";
                    SendToRoom(dataCenter, rNum, ss);
                }
                #endregion
                int sNum = dataCenter.RoomDataDic[rNum].Hero2PlayerDic[dataCenter.RoomDataDic[rNum].FinishCount];

                //如果是主教，发送主教的标签
                if (dataCenter.RoomDataDic[rNum].FinishCount == 5)
                { SendToRoom(dataCenter, rNum, "3|3|5|" + sNum + "|"); }

                //如果被偷，后台操作钱之后通知到小偷和被偷者,并更新战报，并将被偷归零
                if (dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].StoledNum == dataCenter.RoomDataDic[rNum].FinishCount)
                {
                    int money = dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money;
                    int stoleNum = dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(s => s.IsStoling == true) + 1;
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money = 0;
                    dataCenter.RoomDataDic[rNum].PlayerDataList[stoleNum - 1].Money += money;
                    NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, "3|5|2|");
                    NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[stoleNum - 1].Socket, "3|5|1|" + dataCenter.RoomDataDic[rNum].PlayerDataList[stoleNum - 1].Money + "|");
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|3|1|" + stoleNum + "|" + dataCenter.RoomDataDic[rNum].FinishCount + "|" + money + "|");
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].StoledNum = -1;
                    dataCenter.RoomDataDic[rNum].PlayerDataList[stoleNum - 1].IsStoling = false;
                }
                //如果被杀，通知全体，通知个人，然后将被杀归零,叫到下家开始回合
                if (dataCenter.RoomDataDic[rNum].KilledNum == dataCenter.RoomDataDic[rNum].FinishCount)
                {
                    NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, "3|3|3|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|4|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
                    //dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].KilledNum = -1;
                    SendRoundStart(dataCenter, rNum);
                    return;
                }
                //通知全体某人的回合开始
                SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|2|1|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");

                #region 回合开始时候的初始化（该给的钱和建筑师开始多的两张牌）
                int moneyBuilding = 0;
                switch (dataCenter.RoomDataDic[rNum].FinishCount)
                {
                    case 4:
                        foreach (var item in dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].TableB)
                        {
                            if (item.Type == FunctionType.noble)
                                moneyBuilding++;
                            if (item.ID == 59)
                                moneyBuilding++;
                        }
                        break;
                    case 5:
                        foreach (var item in dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].TableB)
                        {
                            if (item.Type == FunctionType.religious)
                                moneyBuilding++;
                            if (item.ID == 59)
                                moneyBuilding++;
                        }
                        break;
                    case 6:
                        SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|3|4|");
                        dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money += 1;
                        foreach (var item in dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].TableB)
                        {
                            if (item.Type == FunctionType.commercial)
                                moneyBuilding++;
                            if (item.ID == 59)
                                moneyBuilding++;
                        }
                        break;
                    case 8:
                        foreach (var item in dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].TableB)
                        {
                            if (item.Type == FunctionType.warlord)
                                moneyBuilding++;
                            if (item.ID == 59)
                                moneyBuilding++;
                        }
                        break;
                }
                //如果存在建筑加成则加上并群发。
                if (moneyBuilding != 0)
                {
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|3|3|" + moneyBuilding + "|");
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money += moneyBuilding;
                }

                //如果是建筑师，则多发两张手牌并群发战报与单发卡牌信息
                if (dataCenter.RoomDataDic[rNum].FinishCount == 7)
                {
                    string s = "";
                    s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.Add(dataCenter.RoomDataDic[rNum].BackB[0]);
                    dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                    s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.Add(dataCenter.RoomDataDic[rNum].BackB[0]);
                    dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|4|");
                    NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, "3|6|2|" + s);
                }
                #endregion
                //通知个人回合开始
                NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, "3|4|1|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");

                return;
            }
            else
            { SendRoundStart(dataCenter, rNum); }
        }

        #endregion

        #region 通用函数
        //给特定房间的人发送数据
        private static void SendToRoom(DataCenter dataCenter, int rNum, string str)
        {
            foreach (var item in dataCenter.RoomDataDic[rNum].PlayerDataList)
            {
                NetCtrl.Send(item.Socket, str);
            }
        }

        //给特定的玩家发送数据
        private static void SendToPlayer(DataCenter dataCenter, int rNum, int sNum, string s)
        {
            NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, s);
        }
        #endregion

        #region 处理信息信息(strs[0]="1")
        private static void SignDeal(DataCenter dataCenter, Socket socket, PlayerData playerData)
        {
            foreach (var item in dataCenter.LobbyPlayerList)
            {
                if (item.Mail == playerData.Mail)
                {
                    switch (item.Status)
                    {
                        case "Online": playerData.Status = "1|3|-1|您的账号已在大厅了|"; return;
                        case "Break": item.Socket = socket; item.Status = "Online"; return;
                    }
                }
            }
            foreach (var room in dataCenter.RoomDataDic)
            {
                foreach (var player in room.Value.PlayerDataList)
                {
                    if (player.Mail == playerData.Mail)
                    {
                        switch (player.Status)
                        {
                            case "Online": playerData.Status = "1|3|-1|您的账号已经在游戏了|"; return;
                            case "Break": player.Socket = socket; player.Status = "Online"; return;
                        }
                    }
                }
            }
        }
        #endregion

        #region 处理大厅信息(strs[0]="2")
        private static void DealLobbyData(DataCenter dataCenter, Socket socket, string[] strs)
        {
            switch (strs[1])
            {
                //处理创建房间数据
                case "1":
                    DealLobbyData1(dataCenter, socket, strs);
                    break;
                //处理加入房间数据
                case "2":
                    DealLobbyData2(dataCenter, socket, strs);
                    break;
                //处理请求信息数据
                case "3":
                    DealLobbyData3(dataCenter, socket, strs);
                    break;
                //处理开始游戏数据
                case "4":
                    DealLobbyData4(dataCenter, socket, strs);
                    break;

            }
        }

        private static void DealLobbyData4(DataCenter dataCenter, Socket socket, string[] strs)
        {
            int rNum = int.Parse(strs[2]);
            dataCenter.RoomDataDic[rNum].Status = "已开始";
            SendRoughLobbyData(dataCenter);
            for (int i = 0; i < dataCenter.RoomDataDic[rNum].PlayerDataList.Count; i++)
            {
                int index = dataCenter.LobbyPlayerList.FindIndex(s => s.RNum == rNum);
                dataCenter.LobbyPlayerList.RemoveAt(index);
            }
            InitGameData(dataCenter, rNum);
            SendToRoom(dataCenter, rNum, "2|4|1|");
        }

        //初始化游戏数据，给玩家初始的牌和钱
        private static void InitGameData(DataCenter dataCenter, int rNum)
        {
            foreach (PlayerData playerData in dataCenter.RoomDataDic[rNum].PlayerDataList)
            {
                playerData.Money = 2;
                playerData.PocketB.Add(dataCenter.RoomDataDic[rNum].BackB[0]);
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                playerData.PocketB.Add(dataCenter.RoomDataDic[rNum].BackB[0]);
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
            }
        }

        //处理信息请求的数据
        private static void DealLobbyData3(DataCenter dataCenter, Socket socket, string[] strs)
        {
            switch (strs[2])
            {
                //请求粗略信息
                case "1":
                    string str = "2|3|1|";
                    foreach (var item in dataCenter.RoomDataDic)
                    {
                        str += item.Key;
                        str += "|";
                        str += item.Value.PlayerDataList.Count;
                        str += "|";
                        str += item.Value.PlayerDataList[0].Nick;
                        str += "|";
                        str += item.Value.Status;
                        str += "|";
                    }
                    NetCtrl.Send(socket, str);
                    break;
            }
        }

        //处理大厅的加入房间数据
        private static void DealLobbyData2(DataCenter dataCenter, Socket socket, string[] strs)
        {
            int rNum = int.Parse(strs[2]);
            int index = dataCenter.LobbyPlayerList.FindIndex(i => i.Mail == strs[3]);
            dataCenter.LobbyPlayerList[index].RNum = rNum;
            dataCenter.LobbyPlayerList[index].SNum = dataCenter.RoomDataDic[rNum].PlayerDataList.Count + 1;
            dataCenter.RoomDataDic[rNum].PlayerDataList.Add(dataCenter.LobbyPlayerList[index]);
            NetCtrl.Send(socket, "2|2|1|" + rNum + "|" + dataCenter.RoomDataDic[rNum].PlayerDataList.Count + "|");
            NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[0].Socket, "2|4|2");
            SendDetailLobbyData(dataCenter, rNum);
            SendRoughLobbyData(dataCenter);
        }

        //处理大厅创建房间的数据
        private static void DealLobbyData1(DataCenter dataCenter, Socket socket, string[] strs)
        {
            dataCenter.RoomNum = 1;
            while (dataCenter.RoomDataDic.Keys.Contains(dataCenter.RoomNum))
            { dataCenter.RoomNum++; }
            int index = dataCenter.LobbyPlayerList.FindIndex(i => i.Mail == strs[2]);
            RoomData rd = new RoomData();
            dataCenter.LobbyPlayerList[index].IsKing = true;
            dataCenter.LobbyPlayerList[index].RNum = dataCenter.RoomNum;
            dataCenter.LobbyPlayerList[index].SNum = 1;
            rd.PlayerDataList.Add(dataCenter.LobbyPlayerList[index]);
            dataCenter.RoomDataDic.Add(dataCenter.RoomNum, rd);
            NetCtrl.Send(socket, "2|1|1|" + dataCenter.RoomNum + "|" + "1|");
            SendDetailLobbyData(dataCenter, dataCenter.RoomNum);
            SendRoughLobbyData(dataCenter);
        }

        //给在大厅中的人发送粗略信息数据
        private static void SendRoughLobbyData(DataCenter dataCenter)
        {
            string str = "2|3|1|";
            foreach (var item in dataCenter.RoomDataDic)
            {
                str += item.Key;
                str += "|";
                str += item.Value.PlayerDataList.Count;
                str += "|";
                str += item.Value.PlayerDataList[0].Nick;
                str += "|";
                str += item.Value.Status;
                str += "|";
            }
            foreach (var item in dataCenter.LobbyPlayerList)
            {
                NetCtrl.Send(item.Socket, str);
            }
        }

        //给在特定房间内的人发送详细信息
        private static void SendDetailLobbyData(DataCenter dataCenter, int rNum)
        {
            string str = "2|3|2|" + rNum + "|";
            foreach (var item in dataCenter.RoomDataDic[rNum].PlayerDataList)
            {
                str += item.SNum;
                str += "|";
                str += item.Nick;
                str += "|";
                str += item.Exp;
                str += "|";
            }
            foreach (var item in dataCenter.RoomDataDic[rNum].PlayerDataList)
            {
                NetCtrl.Send(item.Socket, str);
            }
        }

        #endregion
    }
}
