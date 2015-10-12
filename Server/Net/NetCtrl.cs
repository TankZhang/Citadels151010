using Server.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Net
{
    public  class NetCtrl
    {
        //生成服务器监听的socket
        private Socket _socketWatch;
        public Socket SocketWatch
        {
            get
            {
                return _socketWatch;
            }

            set
            {
                _socketWatch = value;
            }
        }

        /// <summary>
        /// 返回本机IP
        /// </summary>
        /// <returns>本机的IP的数组</returns>
        public static IPAddress[] GetLocalIP()
        {
            string hostname = Dns.GetHostName();
            IPHostEntry localhost = Dns.GetHostEntry(hostname);
            return localhost.AddressList;
        }
        /// <summary>
        /// 监听socket开始监听
        /// </summary>
        /// <param name="o">监听socket</param>
        void Listen(object o)
        {
            Socket socketTemp = o as Socket;
            while (true)
            {
                Socket socketSend;
                try
                {
                    //负责与客户端通信的socket
                    socketSend = socketTemp.Accept();
                    //连接成功
                    Console.WriteLine(socketSend.RemoteEndPoint.ToString() + ":" + "连接成功");
                    //开启一个新的线程不断地接收客户端发过来的消息
                    Thread th = new Thread(Receive);
                    th.IsBackground = true;
                    th.Start(socketSend);
                }//try结束
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString()); //抛出异常信息
                }
            }
        }
        /// <summary>
        /// 接收信息
        /// </summary>
        /// <param name="o"></param>
        void Receive(object o)
        {
            Socket socketSend = o as Socket;
            while (true)
            {
                try
                {
                    //客户端连接成功后，服务器应该接收客户端发来的消息
                    byte[] buffer = new byte[1024 * 1024 * 2];
                    int r = socketSend.Receive(buffer);
                    if (r == 0)
                    { break; }
                    string str = Encoding.UTF8.GetString(buffer, 0, r);
                    Console.WriteLine("服务器收到了：" + str);
                    DataDeal.DealDataPre(App.vM.DataCenter, socketSend, str);
                }
                catch (Exception ex)
                {
                    //抛出异常信息
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="str"></param>
        public static bool Send(Socket socket, string str)
        {
            str += "*";
            Console.WriteLine("服务器发送了：" + str);
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                socket.Send(buffer);
                return socket.Connected;
            }
            catch (Exception ex)
            {
                //抛出异常信息
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
        }
        //构造函数
        public NetCtrl()
        { }
        public NetCtrl(string ipAddr, string port)
        {
            SocketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(ipAddr);
            IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(port));
            SocketWatch.Bind(point);
            SocketWatch.Listen(10);
            Thread th = new Thread(Listen);
            th.IsBackground = true;
            th.Start(SocketWatch);
            Console.WriteLine("监听成功");
        }
    }
}
