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

        public void UpdateChatBox(string message)
        { 
            messageBox.Dispatcher.Invoke(() =>
            { 
                messageBox.Text += message + Environment.NewLine;
                // messageBox.ScrollToEnd(); not found
            }
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (messageInputBox.Text == "")
            {
                MessageBox.Show("No message in text box!", "Warning");
            }
            else
            {
                string message = messageInputBox.Text;
                messageInputBox.Text = "";
                if (usernameBlock.Text == "Template")
                {
                    MessageBox.Show("Please input your username!", "Warning");
                    messageInputBox.Text = message;
                }
                else
                {
                    string name = usernameBlock.Text;
                    messageBox.Text += name + ": " + message + "\n";
                }
            }
        }

        private void SendMessageDispacher(object sender, RoutedEventArgs e)
        {
            string message = messageInputBox.Text;

            // The work to perform on another thread
            ThreadStart ButtonThread = delegate ()
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(SendMessage), message);
            };

            // Creathe the thread and kick it started!
            new Thread(ButtonThread).Start();
        }

        private void SendMessage(string status)
        {
            MessageDisplay.Text += usernameBlock.Text + ": " + status + "\n";
        }
    }
}
