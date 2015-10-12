using Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model.Datas
{
   public class LobbyRoom:NotificationObject
    {
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
                RaisePropertyChanged("RNum");
            }
        }

        string _Creater;
        public string Creater
        {
            get
            {
                return _Creater;
            }

            set
            {
                _Creater = value;
                RaisePropertyChanged("Creater");
            }
        }

        int _num;
        public int Num
        {
            get
            {
                return _num;
            }

            set
            {
                _num = value;
                RaisePropertyChanged("Num");
            }
        }
        
    }
}
