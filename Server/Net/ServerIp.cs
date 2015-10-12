using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Net
{
    public class ServerIp
    {
        private IPAddress _ip;

        public IPAddress Ip
        {
            get
            {
                return _ip;
            }

            set
            {
                _ip = value;
            }
        }
        public ServerIp(IPAddress ip)
        {
            Ip = ip;
        }

    }
}
