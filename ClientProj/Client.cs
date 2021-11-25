using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;

namespace ClientProj
{
    public class Client
    {
        private TcpClient       _TcpClient;
        private NetworkStream   _stream;
        private StreamWriter    _writer;
        private StreamReader    _reader;
        private MainWindow      _form;

        public Client()
        {
            _TcpClient = new TcpClient();
        }

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                _TcpClient.Connect(ipAddress, port);
                _stream = _TcpClient.GetStream();
                _writer = new StreamWriter(_stream, Encoding.UTF8);
                _reader = new StreamReader(_stream, Encoding.UTF8);

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
            _form = new MainWindow()

            // Create a new thread to proccess the server response
            Thread processServerResponse = new Thread(new ThreadStart(ProcessServerResponse));
            processServerResponse.Start();
            


            while ((userInput = Console.ReadLine()) != null)
            {
                _writer.WriteLine(userInput);
                _writer.Flush();

                ProcessServerResponse();

                if (userInput == "Exit")
                    break;
            }
            _TcpClient.Close();
        }

        public void SendMessage(string message)
        {
            _writer.WriteLine(message);
            _writer.Flush();
        }

        private void ProcessServerResponse()
        {
            Console.WriteLine("Server Says: " + _reader.ReadLine());
            Console.WriteLine();
        }
    }
}
