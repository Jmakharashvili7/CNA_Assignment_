using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientProj
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        Client m_Client;

        public MainWindow(Client client)
        {
            InitializeComponent();
            
            // Set the client
            m_Client = client;

            // Setup Send Message Button Function
            SendButton.Click += SendMessageDispacher;
        }

        // Used for updating the Chatbox from outside the client
        public void UpdateChatBox(string message, string userName)
        {
            messageBox.Dispatcher.Invoke(() =>
            {
                messageBox.Text += userName + ": " + message + Environment.NewLine;
                messageBox.ScrollToEnd();
            });
        }

        // delegate for calling SendMessage function
        private void SendMessageDispacher(object sender, RoutedEventArgs e)
        {
            string message = messageInputBox.Text;

            // The work to perform on another thread
            ThreadStart ButtonThread = delegate ()
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(SendMessage), message);
            };

            // Create the the thread and start it
            new Thread(ButtonThread).Start();
        }

        // Used for updating the MessageBox locally
        private void SendMessage(string status)
        {
            m_Client.SendMessage(messageInputBox.Text);

            messageBox.Text += usernameBlock.Text + ": " + messageInputBox.Text + Environment.NewLine;
            messageBox.ScrollToEnd();
        }
    }
}
