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
        private static byte[] udpUpdateBuffer = Encoding.ASCII.GetBytes("updatePos");
        private static Socket serverTcp;
        private static Socket serverUdp;
        private static string sendMsg = "";

        // Client list
        private static List<Socket> clientSockets = new List<Socket>(); //TCP Sockets
        private static List<EndPoint> UDPclientSockets = new List<EndPoint>(); //UDP Sockets

        public static void StartServer()
        {
             
            Console.WriteLine("Starting Server... v6");

            IPHostEntry hostinfo = Dns.GetHostEntry(Dns.GetHostName());

            //IPAddress ip = hostinfo.AddressList[1];

            IPAddress ip = IPAddress.Parse("127.0.0.1");

            serverTcp = new Socket(ip.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            serverUdp = new Socket(ip.AddressFamily,
                SocketType.Dgram, ProtocolType.Udp);

            Console.WriteLine("Server name: {0} IP:{1}", hostinfo.HostName, ip);

            IPEndPoint localEPTcp = new IPEndPoint(ip, 8888);




            ////TCP for sending texts
            serverTcp.Bind(localEPTcp);
            serverTcp.Listen(10);
            serverTcp.BeginAccept(new AsyncCallback(AcceptCallback), null);
            Thread sendThread = new Thread(new ThreadStart(SendLoop));
            sendThread.Name = "SendThread";
            sendThread.Start();



            //UDP for sending positions


            IPEndPoint localEPUdp = new IPEndPoint(ip, 8889);
            EndPoint RemoteClient = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                serverUdp.Bind(localEPUdp);
                Console.WriteLine("Waiting for data....");

                
                while (true)
                {
                    int recv = serverUdp.ReceiveFrom(buffer, ref RemoteClient);
                    
                    Console.WriteLine("Received ({1}) from {0}", RemoteClient.ToString(), Encoding.ASCII.GetString(buffer, 0, recv)); //Print Recieve


                    if (!UDPclientSockets.Contains(RemoteClient)) //Check if the client that sent the packet is a known user
                    { //work
                        UDPclientSockets.Add(RemoteClient); //If not, add the client to the list

                        for (int client = 0; client < UDPclientSockets.Count; client++) //Cycles through Client list
                        {
                            //Request all clients to send position

                            serverUdp.SendTo(udpUpdateBuffer, UDPclientSockets[client]);
                            Console.WriteLine("New client, Sent update request to {0}", UDPclientSockets[client]);

                        }

                    }
                    
                    //UPDATE CLIENT POSITIONS
                    for (int client = 0; client < UDPclientSockets.Count; client++) //cycle through client list works
                    {

                        // Psuedocode: if (RemoteClient != UDPClients.current index or whatever)

                        //Received x,y,z from [C1 IP] ... Sent x,y,x to [C2 IP]

                        if (RemoteClient.ToString() != UDPclientSockets[client].ToString()) //works
                        {
                            //Console.WriteLine("ID: " + RemoteClient.ToString());

                            //Console.WriteLine("Sent To {0}", UDPclientSockets[client]);

                            try
                            {
                                serverUdp.SendTo(buffer, UDPclientSockets[client]);
                                Console.WriteLine("Sent ({0}) to {1}", Encoding.ASCII.GetString(buffer, 0, recv), UDPclientSockets[client]);
                            }
                            catch(SocketException se)
                            {
                                Console.WriteLine(se.ToString());
                                
                            }
                        }

                    }

                    //Might need to write a C# console client to test it


                    


                   
                }
                //server shutdown
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

          


            Console.ReadLine();


            
        }

        static void Main(string[] args)
        {
            StartServer();

     
        }

        private static void AcceptCallback(IAsyncResult result) //accept clients and add to list
        {
            Socket socket = serverTcp.EndAccept(result);
            Console.WriteLine("Client connected! IP: {0}", socket.RemoteEndPoint.ToString());

            clientSockets.Add(socket);

            socket.BeginReceive(buffer, 0, buffer.Length, 0,
                new AsyncCallback(ReceiveCallback), socket);

           serverTcp.BeginAccept(new AsyncCallback(AcceptCallback), null);

        }

       
        private static void ReceiveCallback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            int rec = socket.EndReceive(result);
            byte[] data = new byte[rec];
            Array.Copy(buffer, data, rec);

            string msg = Encoding.ASCII.GetString(data);
            Console.WriteLine("Recv: " + msg);

            sendMsg += " " + msg;
            socket.BeginReceive(buffer, 0, buffer.Length, 0,
                new AsyncCallback(ReceiveCallback), socket);

        }

        private static void SendCallback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            Console.WriteLine("Sending");
            socket.EndSend(result);

            //UPDATE CLIENT POSITIONS
            //for (int client = 0; client < clientSockets.Count; client++) //cycle through client list works
            //{

            //    if (socket.RemoteEndPoint.ToString() != clientSockets[client].RemoteEndPoint.ToString()) //works
            //    {
            //        //Console.WriteLine("ID: " + RemoteClient.ToString());

            //        //Console.WriteLine("Sent To {0}", UDPclientSockets[client]);

            //        try
            //        {
            //            Console.WriteLine("Sent To {0}", clientSockets[client].RemoteEndPoint.ToString());
            //            clientSockets[client].EndSend(result);
            //        }
            //        catch (SocketException se)
            //        {
            //            Console.WriteLine(se.ToString());

            //        }
            //    }

            //}


        }

        private static void SendLoop()
        {
            while (true)
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


        //public void RecievePositions()
        //{

        //}

        //public void SendPositions()
        //{

        //}

    }
}
