using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Datas
{
    public class DataCenter
    {
        public int RoomNum { get; set; }
        public List<PlayerData> LobbyPlayerList { get; set; }
        public Dictionary<int, RoomData> RoomDataDic { get; set; }
        public DataCenter()
        {
            RoomNum = 1;
            LobbyPlayerList = new List<PlayerData>();
            RoomDataDic = new Dictionary<int, RoomData>();
        }
    }
}
