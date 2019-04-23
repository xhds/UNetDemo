using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CSServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //init bind
            var listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddr = IPAddress.Parse("127.0.0.1");
            var ipEndPoint = new IPEndPoint(ipAddr, 8888);
            listenSock.Bind(ipEndPoint);

            //listen
            listenSock.Listen(0);
            Console.WriteLine("[Server] Start Listen");

            while (true)
            {
                //accept
                var connectSock = listenSock.Accept();
                Console.WriteLine("[Server] Accept");

                //receive
                var readBuff = new byte[1024];
                var readCount = connectSock.Receive(readBuff);
                var readStr = Encoding.Default.GetString(readBuff, 0, readCount);
                Console.WriteLine("[Server] Recevive " + readStr);

                //send
                var sendBuff = Encoding.Default.GetBytes(readStr);
                connectSock.Send(sendBuff);
            }
        }
    }
}
 