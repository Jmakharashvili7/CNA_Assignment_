using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using Packets;

namespace ServerProj
{
    class ConnectedClient
    {
        private Socket          m_Socket;
        private Object          m_ReadLock;
        private Object          m_WriteLock;
        private NetworkStream   m_NetworkStream;
        private BinaryWriter    m_BinaryWriter;
        private BinaryReader    m_BinaryReader;
        private BinaryFormatter m_BinaryFormatter;

        public ConnectedClient(Socket socket)
        {
            // Initialize the locks
            m_WriteLock = new object();
            m_ReadLock = new object();

            m_Socket = socket;
            m_BinaryFormatter = new BinaryFormatter();

            m_NetworkStream = new NetworkStream(socket, true);
            m_BinaryReader = new BinaryReader(m_NetworkStream, Encoding.UTF8);
            m_BinaryWriter = new BinaryWriter(m_NetworkStream, Encoding.UTF8);
        }

        public void Close()
        {
            m_NetworkStream.Close();
            m_BinaryReader.Close();
            m_BinaryWriter.Close();
            m_Socket.Close();
        }

        public Packet Read()
        {
            lock (m_ReadLock)
            {
                int numberOfBytes; // temp int to store size of array

                // check the size of the array
                if ((numberOfBytes = m_BinaryReader.ReadInt32()) != -1)
                {
                    // store the array
                    byte[] buffer = m_BinaryReader.ReadBytes(numberOfBytes);

                    // Create a new memory stream using the buffer
                    MemoryStream ms = new MemoryStream(buffer);
                    return m_BinaryFormatter.Deserialize(ms) as Packet;
                }
                else
                    return null;
            }
        }

        public void Send(Packet message)
        {
            lock (m_ReadLock)
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
        }
    }
}
