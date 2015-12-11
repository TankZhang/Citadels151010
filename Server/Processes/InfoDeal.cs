using Server.Datas;
using Server.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Processes
{
    public static class InfoDeal
    {

        public static PlayerData InfoDataDeal(SQLCtrl sqlCtrl, Socket socket, string[] strs)
        {
            PlayerData playerData = new PlayerData();
            //1连接请求。2注册信息。3登录信息
            switch (strs[1])
            {
                //连接
                case "1":
                    playerData.Status = "1|1|1|";
                    return playerData;
                //邮箱|昵称|密码|真实姓名|
                //注册
                case "2":
                    if (sqlCtrl.IsMailExistInDb(strs[2]))
                    { playerData.Status = "1|2|-1|邮箱已注册|"; return playerData; }
                    if (sqlCtrl.IsNickExistInDb(strs[3]))
                    { playerData.Status = "1|2|-1|昵称已注册|"; return playerData; }
                    Register(sqlCtrl, socket, strs, out playerData);
                    return playerData;
                //邮箱|密码|
                //登陆
                case "3":
                    if (!sqlCtrl.IsMailExistInDb(strs[2]))
                    { playerData.Status = "1|3|-1|邮箱未注册|"; return playerData; }
                    Sign(sqlCtrl, socket, strs, out playerData);
                    return playerData;
                default:
                    playerData.Status = "1|信息未知情形|";
                    return playerData;
            }
        }
        //处理登陆数据
        private static bool Sign(SQLCtrl sqlCtrl, Socket socket, string[] strs, out PlayerData playerData)
        {
            playerData = sqlCtrl.SelectInDb(strs[2]);
            if (strs[3] == playerData.Pwd)
            {
                playerData.Status = "1|3|1|"+playerData.Mail+"|"+0 + "|" + playerData.Nick + "|" + playerData.Real + "|" + playerData.Exp + "|";
                playerData.Socket = socket; return true;
            }
            else { playerData.Status = "1|3|-1|登陆失败，请检查密码或重试|"; return false; }
        }

        //处理注册数据
        private static bool Register(SQLCtrl sqlCtrl, Socket socket, string[] strs, out PlayerData playerData)
        {
            if (sqlCtrl.InsertToDb(strs[2], strs[3], strs[4], strs[5]))
            {
                playerData = new PlayerData(strs[2], strs[3], strs[4], strs[5], 0);
                playerData.Status = "1|2|1";
                playerData.Socket = socket;
                return true;
            }
            playerData = new PlayerData();
            playerData.Status = "1|2|-1|注册失败，请重试|";
            return false;
        }
    }
}
