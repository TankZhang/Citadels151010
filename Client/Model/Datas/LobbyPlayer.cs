using Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model.Datas
{
   public class LobbyPlayer:NotificationObject
    {
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
                RaisePropertyChanged("SNum");
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
                RaisePropertyChanged("Nick");
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
                RaisePropertyChanged("Exp");
            }
        }

    }
}
