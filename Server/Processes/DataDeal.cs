using Server.Datas;
using Server.Net;
using Server.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

        public static void DealData(DataCenter dataCenter,Socket socket,string str)
        {
            string[] strs = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            //此处处理客户端传过来的信息
            //1信息。2大厅。
            switch (strs[0])
            {
                //下一级：1连接请求。2注册信息。3登录信息。
                case "1":
                    PlayerData playerData = InfoDeal.InfoDataDeal(App.vM.SQLCtrl, socket, strs);
                    if (playerData.Status.Contains("1|1|1|"))
                    { break; }
                    if (playerData.Status.Contains("1|3|1|"))
                    { SignDeal(dataCenter, socket, playerData); }
                    NetCtrl.Send(socket, playerData.Status);
                    if (playerData.Status.Contains("1|3|1|"))
                    { playerData.Status = "Online"; dataCenter.LobbyPlayerList.Add(playerData); }
                    break;

            }
        }

        private static void SignDeal(DataCenter dataCenter, Socket socket, PlayerData playerData)
        {
            foreach (var item in dataCenter.LobbyPlayerList)
            {
                if (item.Mail == playerData.Mail)
                {
                    switch(item.Status)
                    {
                        case "Online":playerData.Status = "1|3|-1|您的账号已在大厅了|";return;
                        case "Break":item.Socket = socket;item.Status = "Online";return;
                    }
                }
            }
            foreach (var room in dataCenter.RoomDataDic)
            {
                foreach (var player in room.Value.PlayerDataList)
                {
                    if(player.Mail==playerData.Mail)
                    {
                        switch (player.Status)
                        {
                            case "Online": playerData.Status = "1|3|-1|您的账号已经在游戏了|"; return;
                            case "Break": player.Socket = socket; player.Status = "Online";  return;
                        }
                    }
                }
            }        
        }
    }
}
