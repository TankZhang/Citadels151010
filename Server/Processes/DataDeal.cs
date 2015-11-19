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
                case "2":
                    DealLobbyData(dataCenter, socket, strs); break;
                case "3":
                    //1请求数据。2聊天
                    DealGameData(dataCenter, socket, strs); break;
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
            }

        }

        //开局拿的建筑牌,将tableB中的对应牌放入pocketB并且将tableB剩余牌放回backB
        private static void DealGameBuilding3(DataCenter dataCenter, int rNum, int sNum, string[] strs)
        {
            int id;
            int index;
            for (int i = 5; i < strs.Length; i++)
            {
                id = int.Parse(strs[i]);
                index = dataCenter.RoomDataDic[rNum].TableB.FindIndex(s => s.ID == id);
                dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.Add(dataCenter.RoomDataDic[rNum].TableB[index]);
                dataCenter.RoomDataDic[rNum].TableB.RemoveAt(index);
            }
            for (int i = 0; i < dataCenter.RoomDataDic[rNum].TableB.Count; i++)
            {
                dataCenter.RoomDataDic[rNum].BackB.Add(dataCenter.RoomDataDic[rNum].TableB[0]);
                dataCenter.RoomDataDic[rNum].TableB.RemoveAt(0);
            }
            SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|" + "5|3|" + (strs.Length - 5) + "|");
        }

        //选择建设操作
        private static void DealGameBuilding2(DataCenter dataCenter, int rNum, int sNum, string[] strs)
        {
            switch (strs[5])
            {
                //普通建设
                case "1":
                    int id1 = int.Parse(strs[6]);
                    int index1 = dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.FindIndex(s1 => s1.ID == id1);
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].TableB.Add(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB[index1]);
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money -= dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB[index1].Price;
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.RemoveAt(index1);
                    NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, "3|5|3|" + dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money + "|");
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|2|1|" + dataCenter.RoomDataDic[rNum].FinishCount + "|" + id1 + "|");
                    break;
                //建设多张牌
                case "2":
                    string s2 = "";
                    int money2 = 0;
                    for (int i = 6; i < strs.Length; i++)
                    {
                        int id2 = int.Parse(strs[6]);
                        int index2 = dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.FindIndex(s1 => s1.ID == id2);
                        dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].TableB.Add(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB[index2]);
                        money2 += dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB[index2].Price;
                        dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].PocketB.RemoveAt(index2);
                        s2 += (id2 + "|");
                    }
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money -= money2;
                    NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, "3|5|3|" + dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money + "|");
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|2|1|" + dataCenter.RoomDataDic[rNum].FinishCount + "|" + s2);
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
                dataCenter.RoomDataDic[rNum].TableB.Add(dataCenter.RoomDataDic[rNum].BackB[0]);
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                dataCenter.RoomDataDic[rNum].TableB.Add(dataCenter.RoomDataDic[rNum].BackB[0]);
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, s);
                SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|5|1|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
            }
            //从三个里面选择
            if (strs[5] == "2")
            {
                s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                dataCenter.RoomDataDic[rNum].TableB.Add(dataCenter.RoomDataDic[rNum].BackB[0]);
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                dataCenter.RoomDataDic[rNum].TableB.Add(dataCenter.RoomDataDic[rNum].BackB[0]);
                dataCenter.RoomDataDic[rNum].BackB.RemoveAt(0);
                s += (dataCenter.RoomDataDic[rNum].BackB[0].ID + "|");
                dataCenter.RoomDataDic[rNum].TableB.Add(dataCenter.RoomDataDic[rNum].BackB[0]);
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
                    NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, "3|5|3|" + dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Money + "|");
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
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|2|2|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
                    SendRoundStart(dataCenter, rNum);
                    break;

            }

        }

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

        //回合开始
        private static void SendRoundStart(DataCenter dataCenter, int rNum)
        {
            if (dataCenter.RoomDataDic[rNum].FinishCount == 8)
            {
                //重置playerdata和int，移位王冠，并清空当前Hero2PlayerDic。
                int kingSeat = dataCenter.RoomDataDic[rNum].PlayerDataList.FindIndex(player => player.IsKing);
                foreach (var playerdata in dataCenter.RoomDataDic[rNum].PlayerDataList)
                {
                    playerdata.IsKing = false;
                    playerdata.IsBishop = false;
                    playerdata.StoledNum = -1;
                    playerdata.IsStoling = false;
                    playerdata.KilledNum = -1;
                }
                if(dataCenter.RoomDataDic[rNum].Hero2PlayerDic.Keys.Contains(4))
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
                int sNum = dataCenter.RoomDataDic[rNum].Hero2PlayerDic[dataCenter.RoomDataDic[rNum].FinishCount];
                //dataCenter.RoomDataDic[rNum].Hero2PlayerDic.Remove(dataCenter.RoomDataDic[rNum].FinishCount);
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
                if (dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].KilledNum == dataCenter.RoomDataDic[rNum].FinishCount)
                {
                    NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, "3|3|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
                    SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|4|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
                    dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].KilledNum = -1;
                    SendRoundStart(dataCenter, rNum);
                    return;
                }
                NetCtrl.Send(dataCenter.RoomDataDic[rNum].PlayerDataList[sNum - 1].Socket, "3|4|1|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
                SendToRoom(dataCenter, rNum, "3|2|1|" + sNum + "|2|1|" + dataCenter.RoomDataDic[rNum].FinishCount + "|");
                return;
            }
            else
            { SendRoundStart(dataCenter, rNum); }
        }

        #endregion

        #region rNum号房间让人选英雄
        //rNum号房间让某人选英雄
        private static void SendPickHero(DataCenter dataCenter, int rNum)
        {
            if (dataCenter.RoomDataDic[rNum].FinishCount == 0)
            {
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
