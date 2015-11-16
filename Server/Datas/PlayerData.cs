using Server.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Datas
{
    public class PlayerData
    {
        #region 玩家信息
        string _mail;
        public string Mail
        {
            get
            {
                return _mail;
            }

            set
            {
                _mail = value;
            }
        }

        string _nick;
        public string Nick
        {
            get
            {
                return _nick;
            }

            set
            {
                _nick = value;
            }
        }

        string _pwd;
        public string Pwd
        {
            get
            {
                return _pwd;
            }

            set
            {
                _pwd = value;
            }
        }

        string _real;
        public string Real
        {
            get
            {
                return _real;
            }

            set
            {
                _real = value;
            }
        }

        int _exp;
        public int Exp
        {
            get
            {
                return _exp;
            }

            set
            {
                _exp = value;
            }
        }

        string _status;
        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
            }
        }
        #endregion

        Socket _socket;
        public Socket Socket
        {
            get
            {
                return _socket;
            }

            set
            {
                _socket = value;
            }
        }

        #region 游戏数据
        int _rNum;
        public int RNum
        {
            get
            {
                return _rNum;
            }

            set
            {
                _rNum = value;
            }
        }

        int _sNum;
        public int SNum
        {
            get
            {
                return _sNum;
            }

            set
            {
                _sNum = value;
            }
        }

        bool _isKing;
        public bool IsKing
        {
            get
            {
                return _isKing;
            }

            set
            {
                _isKing = value;
            }
        }      

        int _stoledNum;
        public int StoledNum
        {
            get
            {
                return _stoledNum;
            }

            set
            {
                _stoledNum = value;
            }
        }

        bool _isStoling;
        public bool IsStoling
        {
            get
            {
                return _isStoling;
            }

            set
            {
                _isStoling = value;
            }
        }

        int _killedNum;
        public int KilledNum
        {
            get
            {
                return _killedNum;
            }

            set
            {
                _killedNum = value;
            }
        }

        bool _isFirst;
        public bool IsFirst
        {
            get
            {
                return _isFirst;
            }

            set
            {
                _isFirst = value;
            }
        }

        bool _isSecond;
        public bool IsSecond
        {
            get
            {
                return _isSecond;
            }

            set
            {
                _isSecond = value;
            }
        }

        int _money;
        public int Money
        {
            get
            {
                return _money;
            }

            set
            {
                _money = value;
            }
        }

        int _score;
        public int Score
        {
            get
            {
                return _score;
            }

            set
            {
                _score = value;
            }
        }


        public List<Hero> Roles;
        public List<Building> PocketB;
        public List<Building> TableB;
        #endregion

        public PlayerData()
        {
            Roles = new List<Hero>();
            PocketB = new List<Building>();
            TableB = new List<Building>();
        }
        public PlayerData(string mail, string nick,string pwd,string real, int exp)
        {
            Mail = mail;
            Nick = nick;
            Pwd = pwd;
            Real = real;
            Exp = exp;
            Status ="-1";
            RNum = -1;
            SNum = -1;
            IsKing = false;
            StoledNum = -1;
            IsStoling = false;
            KilledNum = -1;
            IsFirst = false;
            IsSecond = false;
            Money = -1;
            Score = -1;
            Roles = new List<Hero>();
            PocketB = new List<Building>();
            TableB = new List<Building>();
        }
    }
}
