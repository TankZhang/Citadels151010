using MySql.Data.MySqlClient;
using Server.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.SQL
{
   public class SQLCtrl
    {
        MySqlConnection _SqlConnection;
        public MySqlConnection SqlConnection
        {
            get
            {
                return _SqlConnection;
            }

            set
            {
                _SqlConnection = value;
            }
        }

        MySqlCommand _SqlCmd;
        public MySqlCommand SqlCmd
        {
            get
            {
                return _SqlCmd;
            }

            set
            {
                _SqlCmd = value;
            }
        }
        //插入数据
        public bool InsertToDb(string mail, string nickname, string pwd, string realName)
        {
            SqlCmd.CommandText = String.Format("insert into GameUser(GameUser_Mail,GameUser_Nickname,GameUser_Pwd,GameUser_Name,GameUser_Exp) values('{0}','{1}','{2}','{3}',0)", mail, nickname, pwd, realName);
            SqlCmd.Connection = SqlConnection;
            if (SqlCmd.ExecuteNonQuery() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //查看Mail是否存在
        public bool IsMailExistInDb(string mail)
        {
            SqlCmd = new MySqlCommand(String.Format("select count(*) from GameUser where GameUser_Mail='{0}'", mail), SqlConnection);
            if (Convert.ToInt32(SqlCmd.ExecuteScalar()) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //查看Nick是否存在
        public bool IsNickExistInDb(string nickName)
        {
            SqlCmd = new MySqlCommand(String.Format("select count(*) from GameUser where GameUser_Nickname='{0}'", nickName), SqlConnection);
            if (Convert.ToInt32(SqlCmd.ExecuteScalar()) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //在citadelsdb的GameUser表中依靠邮箱查询
        public PlayerData SelectInDb(string mail)
        {

            PlayerData playerData = new PlayerData();
            SqlCmd.CommandText = String.Format("select* from GameUser where GameUser_Mail='{0}'", mail);
            SqlCmd.Connection = SqlConnection;
            MySqlDataReader reader = SqlCmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.HasRows)
                {
                    playerData = new PlayerData(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), int.Parse(reader.GetString(5)));
                }
            }
            reader.Close();
            return playerData;
        }
        //构造函数
        public SQLCtrl()
        {
            SqlConnection = new MySqlConnection("server=localhost;User Id=GameServer;password=forever;Database=citadelsdb");
            SqlConnection.Open();
        }
    }
}
