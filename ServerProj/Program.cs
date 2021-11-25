using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace ServerProj
{ 
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 4444);
            server.Start();
            server.Stop();
        }
    }
}
