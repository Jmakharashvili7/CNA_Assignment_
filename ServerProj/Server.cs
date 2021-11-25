using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using Packets;

namespace ServerProj
{
    class Server
    {
        private TcpListener m_TcpListener;
        private ConcurrentDictionary<int, ConnectedClient> m_Clients;

        public Server(string ipAddress, int port)
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            m_TcpListener = new TcpListener(ip, port);
        }

        public void Start()
        {
            m_Clients = new ConcurrentDictionary<int, ConnectedClient>();
            int clientIndex = 0;

            m_TcpListener.Start();

            while (clientIndex < 3)
            {
                Console.WriteLine();

                Socket socket = m_TcpListener.AcceptSocket();
                Console.WriteLine("ConnectionMade");

                ConnectedClient client = new ConnectedClient(socket);
                int index = clientIndex;
                clientIndex++;

                m_Clients.TryAdd(index, client);

                Thread thread = new Thread(() => { ClientMethod(index); });
                thread.Start();
            }
        }

        public void Stop()
        {
            m_TcpListener.Stop();
        }

        private void ClientMethod(int index)
        {
            Packet recievedPacket;

            //m_Clients[index].Send("You have connected to the server - send 0 for valid options");

            while ((recievedPacket = m_Clients[index].Read()) != null)
            {
                if (recievedPacket != null)
                {
                    switch (recievedPacket.GetPacketType())
                    {
                        case PacketType.ChatMessage:
                            ChatMessagePacket chatPacket = (ChatMessagePacket)recievedPacket;
                            m_Clients[index].Send(new ChatMessagePacket(GetReturnMessage(chatPacket.m_message)));
                            break;
                        case PacketType.ClientName:
                            break;
                        case PacketType.PrivateMessage:
                            break;
                        default:
                            break;
                    }
                }
            }

            m_Clients[index].Close();
            ConnectedClient c;
            m_Clients.TryRemove(index, out c);
        }

        private string GetReturnMessage(string code)
        {
            code = code.ToLower();

            if (code == "hi")
            {
                return "Hello";
            }

            if (code == "hello")
                return "Hi!";

            if (code == "how are you")
                return "I am great!";

            if (code == "exit")
                return "exit";

            // if no valid response found
            return "Invalid response!";
        }
    }
}
