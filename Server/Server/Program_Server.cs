﻿using System;
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

            //serverTcp = new Socket(ip.AddressFamily,
            //    SocketType.Stream, ProtocolType.Tcp);

            serverUdp = new Socket(ip.AddressFamily,
                SocketType.Dgram, ProtocolType.Udp);

            Console.WriteLine("Server name: {0} IP:{1}", hostinfo.HostName, ip);

            //IPEndPoint localEPTcp = new IPEndPoint(ip, 8888);
   
            


            ////TCP for sending texts
            //serverTcp.Bind(localEPTcp);
            //serverTcp.Listen(10);
            //serverTcp.BeginAccept(new AsyncCallback(AcceptCallback), null);
            //Thread sendThread = new Thread(new ThreadStart(SendLoop));
            //sendThread.Name = "SendThread";
            //sendThread.Start();
 
            
            
            //UDP for sending positions, 


            IPEndPoint localEPUdp = new IPEndPoint(ip, 8889);
            EndPoint RemoteClient = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                serverUdp.Bind(localEPUdp);
                Console.WriteLine("Waiting for data....");

                
                while (true)
                {
                    int recv = serverUdp.ReceiveFrom(buffer, ref RemoteClient);

                    if (!UDPclientSockets.Contains(RemoteClient)) //Check if the list contains the client and if remote client has the value
                    { //work
                        UDPclientSockets.Add(RemoteClient);
                        Console.WriteLine("Client Add", UDPclientSockets[0]);
                    }
                    
                   
                    for (int client = 0; client < UDPclientSockets.Count; client++) //cycle through client list works
                    {
                        
                        // if (RemoteClient != UDPClients.current index or whatever)

                        Console.WriteLine("Recv from: {0}   Data: {1}  Dest: {2}",
                            RemoteClient.ToString(), Encoding.ASCII.GetString(buffer, 0, recv), UDPclientSockets[client]);

                        if (RemoteClient.ToString() != UDPclientSockets[client].ToString()) //works, idk how "0.0.0.0:0" appears
                        {
                            //Console.WriteLine("ID: " + RemoteClient.ToString());

                            Console.WriteLine("Sent To {0}", UDPclientSockets[client]);

                            try
                            {
                                serverUdp.SendTo(buffer, UDPclientSockets[client]);
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

        private static void AcceptCallback(IAsyncResult result)
        {
            Socket socket = serverTcp.EndAccept(result);
            Console.WriteLine("Client connected!!");

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


        public void RecievePositions()
        {

        }

        public void SendPositions()
        {

        }

    }
}
