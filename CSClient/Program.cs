using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CSClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var sendSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sendSock.Connect("127.0.0.1", 8888);
            Console.WriteLine("[Client] Connect To Server");

            var sendStr = "OMG";
            var sendBuff = Encoding.Default.GetBytes(sendStr);
            sendSock.Send(sendBuff);
            Console.WriteLine("[Client] Send To Server: " + sendStr);

            var receiveBuff = new byte[1024];
            var readCount = sendSock.Receive(receiveBuff);
            var receiveStr = Encoding.Default.GetString(receiveBuff, 0, readCount);
            Console.WriteLine("[Client] Receive from Server: " + receiveStr);

            sendSock.Close();
            Console.WriteLine("[Client] Close!");

            //Console.ReadLine();
        }
    }
}
