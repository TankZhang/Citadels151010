using Server.Datas;
using Server.Net;
using Server.SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
   public class VM:NotificationObject
    {
        public SQLCtrl SQLCtrl { get; set; }
        public NetCtrl NetCtrl { get; set; }
        public DataCenter DataCenter { get; set; }

        ObservableCollection<ServerIp> _ips;
        public ObservableCollection<ServerIp> Ips
        {
            get
            {
                return _ips;
            }

            set
            {
                _ips = value;
                RaisePropertyChanged("Ips");
            }
        }

        public VM()
        {
            NetCtrl = new NetCtrl();
            SQLCtrl = new SQLCtrl();
            DataCenter = new DataCenter();
            Ips = new ObservableCollection<ServerIp>();
            foreach (var item in NetCtrl.GetLocalIP())
            {
                if (!item.ToString().Contains("%"))
                {
                    Ips.Add(new ServerIp(item));
                }
            }
        }
    }
}
