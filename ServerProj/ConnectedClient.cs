using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace ServerProj
{
    class ConnectedClient
    {
        private Socket          m_Socket;
        private Object          m_ReadLock;
        private Object          m_WriteLock;
        private NetworkStream   m_NetworkStream;
        private BinaryWriter    m_StreamWriter;
        private BinaryReader    m_StreamReader;
        private BinaryFormatter m_BinaryFormater;

        public ConnectedClient(Socket socket)
        {
            m_WriteLock = new object();
            m_ReadLock = new object();

            m_Socket = socket;
            m_BinaryFormater = new BinaryFormatter();

            m_NetworkStream = new NetworkStream(socket, true);
            m_StreamReader = new BinaryReader(m_NetworkStream, Encoding.UTF8);
            m_StreamWriter = new BinaryWriter(m_NetworkStream, Encoding.UTF8);
        }

        public void Close()
        {
            m_NetworkStream.Close();
            m_StreamReader.Close();
            m_StreamWriter.Close();
            m_Socket.Close();
        }

        public string Read()
        {
            lock (m_ReadLock)
            {
                return m_StreamReader.ReadLine();
            }
        }

        public void Send(String message)
        {
            lock (m_ReadLock)
            {
                m_StreamWriter.WriteLine(message);
                m_StreamWriter.Flush();
            }
        }
    }
}
