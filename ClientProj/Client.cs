using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using Packets;

namespace ClientProj
{
    public class Client
    {
        private TcpClient       m_TcpClient;
        private NetworkStream   m_Stream;
        private BinaryWriter    m_BinaryWriter;
        private BinaryReader    m_BinaryReader;
        private MainWindow      m_MainWindow;
        private BinaryFormatter m_BinaryFormatter;

        public Client()
        {
            m_TcpClient = new TcpClient();
            m_BinaryFormatter = new BinaryFormatter();
        }

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                m_TcpClient.Connect(ipAddress, port);
                m_Stream = m_TcpClient.GetStream();
                m_BinaryWriter = new BinaryWriter(m_Stream, Encoding.UTF8);
                m_BinaryReader = new BinaryReader(m_Stream, Encoding.UTF8);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        public void Run()
        {
            m_MainWindow = new MainWindow(this);

            // Create a new thread to proccess the server response
            Thread processServerResponse = new Thread(new ThreadStart(ProcessServerResponse));
            processServerResponse.Start();

            m_MainWindow.ShowDialog();
        }

        public void SendMessage(Packet message)
        {
            // initialize the memory stream
            MemoryStream memoryStream = new MemoryStream();

            m_BinaryFormatter.Serialize(memoryStream, message);

            // Get byte array from the memory stream
            byte[] buffer = memoryStream.GetBuffer();

            // Write the size of the buffer and the buffer array to the stream
            m_BinaryWriter.Write(buffer.Length);
            m_BinaryWriter.Write(buffer);
            m_BinaryWriter.Flush();
        }

        private void ProcessServerResponse()
        {
            while (m_TcpClient.Connected)
            {
                try
                {
                    // read the input from the server and update the messagebox
                    int numberOfBytes; // temp int to store size of array

                    // check the size of the array
                    if ((numberOfBytes = m_BinaryReader.ReadInt32()) != -1)
                    {
                        // store the array
                        byte[] buffer = m_BinaryReader.ReadBytes(numberOfBytes);

                        // Create a new memory stream using the buffer
                        MemoryStream ms = new MemoryStream(buffer);
                        Packet recievedPacket = m_BinaryFormatter.Deserialize(ms) as Packet;

                        if (recievedPacket != null)
                        {
                            switch (recievedPacket.GetPacketType())
                            {
                                case PacketType.ChatMessage:
                                    ChatMessagePacket chatPacket = (ChatMessagePacket)recievedPacket;
                                    m_MainWindow.UpdateChatBox(chatPacket.m_message, "Server");
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
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception" + e.Message);
                }
            }
        }
    }
}
