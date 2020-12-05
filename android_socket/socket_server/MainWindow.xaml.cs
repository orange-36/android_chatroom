using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;


namespace socket_server
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    ///

    public partial class MainWindow : Window
    {

        private static int Port = 3030;
        Socket ServerSocket = null;
        private static bool isServerWorking = false;
        private static int num = 0;
        private static bool is_doubleStart = false;

        private static IPAddress IPaddr;
        private static Thread startListenClient;

        private static Dictionary<String, Socket> dic_socket = new Dictionary<string, Socket>();
        private static Dictionary<String, Thread> dic_Threads = new Dictionary<string, Thread>();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListenClient()
        {
            while ( isServerWorking )
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    system_msg.AppendText("Waiting for connections..."+"\n");
                }));

                Socket client = null ;
                try { client = ServerSocket.Accept(); }
                catch (Exception e )
                {
                    num = 0;
                }
                if (client == null) break;
                ++num;

                string RemoteIP = client.RemoteEndPoint.ToString();
                dic_socket.Add(RemoteIP, client);

                this.Dispatcher.Invoke((Action)(() =>
                {
                    system_msg.AppendText("User IP = " + RemoteIP + " is connected"+"\n");
                    system_msg.AppendText("Number of Users : " + num.ToString() + "\n");
                }));

                Thread t = new Thread(getMsg);
                t.IsBackground = true;
                dic_Threads.Add(RemoteIP, t);
                t.Start(client);
            }

        }

        private void getMsg(Object obj)
        {
            Socket client = (Socket)obj;
            string RemoteIP = client.RemoteEndPoint.ToString();
            byte[] buffer;
            string msg = null;
            while ( isServerWorking )
            {
                buffer = new byte[1024];
                try { msg = Encoding.UTF8.GetString(buffer, 0, client.Receive(buffer)); }
                catch (Exception e)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        if ( true == dic_socket.ContainsKey(RemoteIP) )
                            system_msg.AppendText("Client IP = : " + RemoteIP + " has disconnected" + "\n");
                    }));
                    break;
                }

                if (msg != null && msg != "" && msg != " ")
                {
                    this.Dispatcher.Invoke((Action)(() =>
                        {
                            system_msg.AppendText(client.RemoteEndPoint.ToString() + " where name = " + msg + "\n");
                        }));
                }
                castMsg(msg);
                msg = null;
            }
            --num;
            dic_socket.Remove(client.RemoteEndPoint.ToString());
        }

        public void castMsg( string Msg )
        {
            byte[] bt = Encoding.UTF8.GetBytes(Msg+"\n");
            foreach ( KeyValuePair<String,Socket> item in dic_socket)
            {
                try { item.Value.Send(bt); }
                catch (Exception e) { }
            }
        }


        private void Start_server_Click(object sender, RoutedEventArgs e)
        {
            if (is_doubleStart == false)
            {
                is_doubleStart = true;
                socket_state_tbox.Text = "Connected";

                IPaddr = new IPAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].Address);
                system_msg.AppendText("Server started : IP = " + IPaddr.ToString() +", Port = "+ Port+"\n");
                IPEndPoint endPoint = new IPEndPoint(IPaddr, Port);

                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerSocket.Bind(endPoint);
                ServerSocket.Listen(10);

                startListenClient = new Thread(new ThreadStart(ListenClient));
                startListenClient.IsBackground = true;
                isServerWorking = true;
                startListenClient.Start();
            }
        }

        private void End_server_Click(object sender, RoutedEventArgs e)
        {
            if (is_doubleStart == true)
            {
                foreach ( KeyValuePair<String, Thread> thread in dic_Threads)
                {
                    thread.Value.Interrupt();
                    thread.Value.Abort();
                }
                foreach ( KeyValuePair<String,Socket> client in dic_socket)
                {
                    client.Value.Close();
                }
                startListenClient.Interrupt();
                dic_Threads.Clear();
                dic_socket.Clear();

                system_msg.AppendText("Server has Shut down"+"\n");
                socket_state_tbox.Text = "Disconnected";
                ServerSocket.Close();
                
                isServerWorking = false;
                ServerSocket = null;
                is_doubleStart = false;
                num = 0;
            }
        }
    }
}
