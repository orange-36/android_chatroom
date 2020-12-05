using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
//using System.Text.Json;
using System.Threading;
using System.Windows;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int listen = 1;
        public Socket server;
        public String host = "127.0.0.7";
        public int port = 20001;
        public List<Socket> SocketLists = new List<Socket>();
        public List<int> IdLists = new List<int>();
        int id = 0;
        Thread thread;

        public MainWindow()
        {
            InitializeComponent();
        }

        //start a socket server
        private void Start(object sender, RoutedEventArgs e)
        {
            host = ip_text.Text;
            port = Int16.Parse(port_text.Text);
            if (server == null)
            {
                receive_msg.Text = "Server Start\n";
                IPAddress ip = IPAddress.Parse(host);
                //socket()
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //bind()
                server.Bind(new IPEndPoint(ip, port));
                //listen()
                server.Listen(10);

                thread = new Thread(Listen);
                thread.Start();
            }
            else
            {
                if (listen == 0)
                {
                    listen = 1;
                    receive_msg.Text += "Server Start\n";
                }
            }
        }

        //listen to socket client
        private void Listen()
        {
            while (true)
            {
                //accept()
                try
                {
                    Socket client = server.Accept();
                    SocketLists.Add(client);
                    Thread receive = new Thread(ReceiveMsg);
                    receive.Start(client);
                }
                catch
                {
                    break;
                }
            }
        }

        //receive client message and send to client
        public void ReceiveMsg(object client)
        {
            int connect = 0;
            int connectid = id;
            id++;
            Socket connection = (Socket)client;
            IPAddress clientIP = (connection.RemoteEndPoint as IPEndPoint).Address;
            int clientPort = (connection.RemoteEndPoint as IPEndPoint).Port;

            while (true)
            {
                try
                {
                    byte[] result = new byte[1024];
                    //receive message from client
                    int receive_num = connection.Receive(result);
                    String receive_str = Encoding.ASCII.GetString(result, 0, receive_num);

                    if (receive_num > 0)
                    {
                        dynamic receive_json = JsonConvert.DeserializeObject(receive_str);
                        if (listen == 1)
                        {
                            if (connect == 1)
                            {
                                if (receive_json.Msg =="shut down")
                                {
                                    if (listen == 1)
                                    {
                                        listen = 0;
                                        UserData userData = new UserData("", "", "server is not ready", "2");
                                        string jstr = System.Text.Json.JsonSerializer.Serialize(userData);
                                        //resend message to client
                                        receive_msg.Dispatcher.BeginInvoke(
                                        new Action(() => { receive_msg.Text += "server stop\n"; }), null);
                                        for (int i = 0; i < SocketLists.Count; i++)
                                        {
                                            if (SocketLists[i] != null)
                                            {
                                                try
                                                {
                                                    SocketLists[i].Send(Encoding.ASCII.GetBytes(jstr + "\n"));
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("bad");
                                                }
                                            }
                                        }
                                    }
                                    //connection.Send(Encoding.ASCII.GetBytes(jstr));
                                }
                                else if(receive_json.Connect == 1)
                                {
                                    String send_str = "" + receive_json.Username + "(" + clientIP + ") : " + receive_str + "\n";
                                    UserData userData = new UserData("", "", "" + receive_json.Username + ": " + receive_json.Msg, "1");
                                    string jstr = System.Text.Json.JsonSerializer.Serialize(userData);
                                    //resend message to client
                                    for (int i = 0; i < SocketLists.Count; i++)
                                    {
                                        if (SocketLists[i] != null)
                                        {
                                            try
                                            {
                                                SocketLists[i].Send(Encoding.ASCII.GetBytes(jstr + "\n"));
                                            }
                                            catch
                                            {
                                                Console.WriteLine("bad");
                                            }
                                        }
                                    }
                                    //connection.Send(Encoding.ASCII.GetBytes(jstr));


                                    //receive_msg.Dispatcher.BeginInvoke(
                                    //  new Action(() => { receive_msg.Text += send_str; }), null);
                                    receive_msg.Dispatcher.BeginInvoke(
                                        new Action(() => { receive_msg.Text += ""+receive_json.Username + "(" + clientIP + ":" + clientPort + ")= " +receive_str + "\n"; }), null);
                                }
                                 

                            }
                            else
                            {
                                UserData userData = new UserData("", "" + receive_json.Username, "Welcome " + receive_json.Username + " !", "1");
                                string jstr = System.Text.Json.JsonSerializer.Serialize(userData);
                                //resend message to client
                                for (int i = 0; i < SocketLists.Count; i++)
                                {
                                    if (SocketLists[i] != null)
                                    {
                                        try
                                        {
                                            SocketLists[i].Send(Encoding.ASCII.GetBytes(jstr + "\n"));
                                        }
                                        catch
                                        {
                                            Console.WriteLine("bad");
                                        }
                                    }
                                }
                                //connection.Send(Encoding.ASCII.GetBytes(jstr+"\n"));
                                receive_msg.Dispatcher.BeginInvoke(
                                    new Action(() => { receive_msg.Text += "" + receive_json.Username + "(" + clientIP +":"+ clientPort+ ")  connect\n"; }), null);
                                //send welcome message to client
                                connect = 1;
                            }

                        }
                        else
                        {
                            UserData userData = new UserData("", "", "server is not ready", "2");
                            string jstr = System.Text.Json.JsonSerializer.Serialize(userData);
                            //resend message to client
                            connection.Send(Encoding.ASCII.GetBytes(jstr + "\n"));
                            //send welcome message to client
                        }


                    }
                }
                catch (Exception e)
                {
                    //exception close()
                    Console.WriteLine(e);
                    //connection.Shutdown(SocketShutdown.Both);
                    //connection.Close();
                    break;
                }
            }
        }

        //close() when close window
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            if (listen == 1)
            {
                listen = 0;
                UserData userData = new UserData("", "", "server is not ready", "2");
                string jstr = System.Text.Json.JsonSerializer.Serialize(userData);
                //resend message to client
                receive_msg.Text += "server stop\n";
                for (int i = 0; i < SocketLists.Count; i++)
                {
                    if (SocketLists[i] != null)
                    {
                        try
                        {
                            SocketLists[i].Send(Encoding.ASCII.GetBytes(jstr + "\n"));
                        }
                        catch
                        {
                            Console.WriteLine("bad");
                        }
                    }
                }
            }

        }
    }
}
