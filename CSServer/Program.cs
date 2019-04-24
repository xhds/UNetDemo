using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace CSServer
{
    public class ClientState
    {
        public Socket Socket;
        public byte[] ReadBuff = new byte[1024];
    }

    class Program
    {
        private static Socket _listenSock;
        private static Dictionary<Socket, ClientState> _clients = new Dictionary<Socket, ClientState>();

        static void Main(string[] args)
        {
            Console.WriteLine("[Server] Start");

            _listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ip = IPAddress.Parse("127.0.0.1");
            var ep = new IPEndPoint(ip, 8888);
            _listenSock.Bind(ep);
            _listenSock.Listen(0);
            _listenSock.BeginAccept(AcceptCallback, _listenSock);
            Console.WriteLine("[Server] Waiting...");
            Console.ReadLine();
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                var sock = (Socket)ar.AsyncState;
                var clientSocket = sock.EndAccept(ar);
                var newClient = new ClientState(){
                    Socket = clientSocket,
                };
                _clients.Add(clientSocket, newClient);
                Console.WriteLine("[Server] Accept. Now Client Count " + _clients.Count);
                sock.BeginAccept(AcceptCallback, sock);
                Console.WriteLine("[Server] Waiting...");
                clientSocket.BeginReceive(newClient.ReadBuff, 0, newClient.ReadBuff.Length, 0, ReceiveCallback, newClient);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Server] AcceptCallback Exception: " + e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var client = (ClientState)ar.AsyncState;
                var receiveCount = client.Socket.EndReceive(ar);
                if (receiveCount == 1)
                {
                    client.Socket.Close();
                    _clients.Remove(client.Socket);
                    Console.WriteLine("[Server] Client close. Now Client Count " + _clients.Count);
                    return;
                }

                var receiveStr = Encoding.Default.GetString(client.ReadBuff, 0, receiveCount);
                Console.WriteLine("[Server] Receive Str: " + receiveStr);

                var sendBuff = Encoding.Default.GetBytes(receiveStr);
                client.Socket.BeginSend(sendBuff, 0, sendBuff.Length, 0, SendCallback, client);

                client.Socket.BeginReceive(client.ReadBuff, 0, client.ReadBuff.Length, 0, ReceiveCallback, client);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Server] ReceiveCallback Exception: " + e.ToString());
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                var client = (ClientState)ar.AsyncState;
                client.Socket.EndSend(ar);
                Console.WriteLine("[Server] SendCallback EndSend Client " + client.Socket.RemoteEndPoint.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("[Server] SendCallback Exception: " + e.ToString());
            }
        }
    }
}
 