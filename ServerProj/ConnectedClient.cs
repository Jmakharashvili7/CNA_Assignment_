using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerProj
{
    class ConnectedClient
    {
        private Socket        m_Socket;
        private NetworkStream m_NetworkStream;
        private StreamWriter  m_StreamWriter;
        private StreamReader  m_StreamReader;
        private Object        m_ReadLock;
        private Object        m_WriteLock;

        public ConnectedClient(Socket socket)
        {
            m_WriteLock = new object();
            m_ReadLock = new object();

            m_Socket = socket;

            m_NetworkStream = new NetworkStream(socket, true);
            m_StreamReader = new StreamReader(m_NetworkStream, Encoding.UTF8);
            m_StreamWriter = new StreamWriter(m_NetworkStream, Encoding.UTF8);
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
