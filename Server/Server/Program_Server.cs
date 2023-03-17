using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;


namespace Server
{
    class Program
    {
        private static byte[] buffer = new byte[512];
        private static byte[] sendBuffer = new byte[512];
        private static Socket server;
        private static string sendMsg = "";

        // Client list
        private static List<Socket> clientSockets = new List<Socket>();


        public static void StartServer()
        {
            Console.WriteLine("Starting Server...");

            IPHostEntry hostinfo = Dns.GetHostEntry(Dns.GetHostName());

            IPAddress ip = hostinfo.AddressList[1];

            server = new Socket(ip.AddressFamily,
                SocketType.Stream, ProtocolType.Udp);

            Console.WriteLine("Server name: {0} IP:{1}", hostinfo.HostName, ip);

            IPEndPoint localEP = new IPEndPoint(ip, 8889);

            EndPoint RemoteClient = new IPEndPoint(IPAddress.Any, 0);

            server.Bind(localEP);
            server.Listen(10);
            server.BeginAccept(new AsyncCallback(AcceptCallback), null);
            Thread sendThread = new Thread(new ThreadStart(SendLoop));
            sendThread.Name = "SendThread";
            sendThread.Start();

            Console.ReadLine();
            
        }

        static void Main(string[] args)
        {
            StartServer();
     
        }

        private static void AcceptCallback(IAsyncResult result)
        {
            Socket socket = server.EndAccept(result);
            Console.WriteLine("Client connected!!");

            clientSockets.Add(socket);

            socket.BeginReceive(buffer, 0, buffer.Length, 0,
                new AsyncCallback(ReceiveCallback), socket);

            server.BeginAccept(new AsyncCallback(AcceptCallback), null);

        }

        private static void ReceiveCallback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            int rec = socket.EndReceive(result);
            byte[] data = new byte[rec];
            Array.Copy(buffer, data, rec);

            string[] msg = Encoding.ASCII.GetString(buffer,0,rec).Split(',');

            Console.WriteLine("Recv: " + msg);

            ////// Here is where you protect the resource (buffer)
            sendMsg += " " + msg;

            socket.BeginReceive(buffer, 0, buffer.Length, 0,
                new AsyncCallback(ReceiveCallback), socket);

        }

        private static void SendCallback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            socket.EndSend(result);
        }

        private static void SendLoop()
        {
            while(true)
            {
                sendBuffer = Encoding.ASCII.GetBytes(sendMsg);

                foreach (var socket in clientSockets)
                {
                    Console.WriteLine("Sent to: " + 
                        socket.RemoteEndPoint.ToString());

                    socket.BeginSend(sendBuffer, 0, sendBuffer.Length,
                            0, new AsyncCallback(SendCallback), socket);

                }

                sendMsg = "";
                Thread.Sleep(1000);



            }
        }

    }
}
