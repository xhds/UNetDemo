using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CSClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[Client] Start.");
            _sendBuff = Encoding.Default.GetBytes(_sendStr);
            Connection();

            var update = new Thread(Update);
            update.Start();

            while(true){}
        }

        private static Socket _clientSock = null;
        private static string _sendStr = "OMG";
        private static byte[] _sendBuff = null;

        private static string _receiveStr = null;
        private static byte[] _receiveBuff = new byte[1024];

        private static void Connection()
        {
            _clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSock.BeginConnect("127.0.0.1", 8888, ConnectionCallback, _clientSock);
        }

        private static void ConnectionCallback(IAsyncResult ar)
        {
            try
            {
                var sock = (Socket)ar.AsyncState;
                if (sock.Connected)
                {
                    Console.WriteLine("[Client] Connect To Server Successfully!");
                    sock.EndConnect(ar);
                    sock.BeginSend(_sendBuff, 0, _sendBuff.Length, 0, SendCallback, sock);
                    sock.BeginReceive(_receiveBuff, 0, _receiveBuff.Length, 0, ReceiveCallback, sock);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[Client] Connect Exception: " + e.ToString());
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                var sock = (Socket)ar.AsyncState;
                var sendCount = sock.EndSend(ar);
                Console.WriteLine("[Client] Send Count: " + sendCount.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("[Client] Send Exception: " + e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var sock = (Socket)ar.AsyncState;
                var receiveCount = sock.EndReceive(ar);
                Console.WriteLine("[Client] Receive Count: " + receiveCount.ToString());
                _receiveStr = null;
                _receiveStr = Encoding.Default.GetString(_receiveBuff, 0, receiveCount);
                if (receiveCount == 3)
                {
                    var close = new byte[1];
                    sock.BeginSend(close, 0, close.Length, 0, SendCallback, sock);
                }
                sock.BeginReceive(_receiveBuff, 0, _receiveBuff.Length, 0, ReceiveCallback, sock);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Client] Receive Exception: " + e.ToString());
            }
        }

        private static void Update()
        {
            while (true)
            {
                Thread.Sleep(300);
                if (!string.IsNullOrEmpty(_receiveStr))
                {
                    Console.WriteLine("[Client] Receive: " + _receiveStr);
                    _receiveStr = null;
                }
            }
            
        }
    }
}
