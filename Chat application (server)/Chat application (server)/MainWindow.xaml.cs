using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows;
using WatsonTcp;

namespace Chat_application__server_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public class serverTCP
    {
        public static SQLiteConnection dbConnection;
        public static WatsonTcpServer server;
        public static MainWindow reff;
        public static void startServer()
        {
            initDB();
            server = new WatsonTcpServer("0.0.0.0", 21341);
            server.Events.ClientConnected += ClientConnected;
            server.Events.ClientDisconnected += ClientDisconnected;
            server.Events.MessageReceived += MessageReceived;
            try
            {
                server.Start();
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    reff.startServerButton.Content = "خاموش کردن سرور";
                    reff.statusTextBlock.Text = "فعال";
                });
                }
            catch (Exception)
            {
                MessageBox.Show("عدم توانایی در اجرای سرور", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

            //// list clients
            //IEnumerable<string> clients = server.ListClients();

            //// send a message
            //server.Send("[IP:port]", "Hello, client!");

            //// send a message with metadata
            //Dictionary<object, object> md = new Dictionary<object, object>();
            //md.Add("foo", "bar");
            //server.Send("[IP:port]", "Hello, client!  Here's some metadata!", md);

        }
        static void initDB()
        {
            // این متد فرآیند اتصال به دیتابیس را انجام میدهد
            String path = @"database\info.db";
            dbConnection = new SQLiteConnection("Data Source=" + path + ";Version=3;");
            dbConnection.Open();

        }
        static void ClientConnected(object sender, WatsonTcp.ConnectionEventArgs args)
        {
            Console.WriteLine("Client connected: " + args.IpPort);
        }

        static void ClientDisconnected(object sender, DisconnectionEventArgs args)
        {
            Console.WriteLine("Client disconnected: " + args.IpPort + ": " + args.Reason.ToString());
        }

        static void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            string msg = Encoding.UTF8.GetString(args.Data);
            //Console.WriteLine("Message from " + args.IpPort + ": " + Encoding.UTF8.GetString(args.Data));
            Dictionary<object, object> md = new Dictionary<object, object>();
            md = args.Metadata;
            //MessageBox.Show(msg);
            try
            {
                if (msg == "signup")
                {
                    string[] data = md["signup"].ToString().Split('|');

                    string sql1 = String.Format("SELECT username FROM Users WHERE username='{0}'", data[0]);
                    SQLiteCommand command1 = new SQLiteCommand(sql1, dbConnection);
                    SQLiteDataReader reader = command1.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows)
                    {
                        server.Send(args.IpPort, "Unsuccess_signup");
                        server.DisconnectClient(args.IpPort);
                        return;
                    } else
                    {
                        string sql = String.Format("INSERT INTO Users (username, password, isOnline) VALUES ('{0}', '{1}', '{2}')", data[0], data[1], 1);
                        SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                        command.ExecuteNonQuery();
                        Users.listUser.Add(new Users(args.IpPort, data[0]));
                        server.Send(args.IpPort, "Success_signup");
                    }
                    reader.Close();
                }
                else if (msg == "login")
                {
                    string[] data = md["login"].ToString().Split('|');

                    string sql1 = String.Format("SELECT username FROM Users WHERE username = '{0}' AND password = '{1}'", data[0], data[1]);
                    SQLiteCommand command1 = new SQLiteCommand(sql1, dbConnection);
                    SQLiteDataReader reader = command1.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows)
                    {
                        string sql = String.Format("UPDATE Users SET isOnline='1' WHERE username='{0}'", data[0]);
                        SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                        command.ExecuteNonQuery();
                        bool flag = false;
                        foreach (var item in Users.listUser)
                        {
                            if (item.IpPort != args.IpPort && item.Username == data[0]) {
                                server.Send(item.IpPort, "Success_logout");
                                server.DisconnectClient(item.IpPort);
                                item.IpPort = args.IpPort;
                                flag = true;
                                break;
                            }
                        }
                        if (flag == false)
                        {
                            Users.listUser.Add(new Users(args.IpPort, data[0]));
                        }
                        

                        server.Send(args.IpPort, "Success_login");
                    }
                    else
                    {
                        server.Send(args.IpPort, "Unsuccess_login");
                        server.DisconnectClient(args.IpPort);
                        return;
                    }
                    reader.Close();
                }
                else if (msg == "isOnline")
                {
                    string data = md["isOnline"].ToString();

                    string sql1 = String.Format("SELECT isOnline FROM Users WHERE username = '{0}'", data);
                    SQLiteCommand command1 = new SQLiteCommand(sql1, dbConnection);
                    SQLiteDataReader reader = command1.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows)
                    {
                       
                        if (reader["isOnline"]+"" == "1")
                        {
                            server.Send(args.IpPort, "userIsOn");
                        }
                        else
                        {
                            server.Send(args.IpPort, "userIsOff");
                        }
                        
                    }
                    else
                    {
                        server.Send(args.IpPort, "userNotAvailable");
                    }
                    
                    reader.Close();
                }
                else if (msg == "logout")
                {
                    string usernamy = "";
                    foreach (var item in Users.listUser)
                    {
                        if (item.IpPort == args.IpPort)
                        {
                            usernamy = item.Username;
                            break;
                        }
                    }
                    string sql = String.Format("UPDATE Users SET isOnline='0' WHERE username='{0}'", usernamy);
                    //MessageBox.Show(sql);
                    SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                    command.ExecuteNonQuery();
                    server.Send(args.IpPort, "Success_logout");
                    server.DisconnectClient(args.IpPort);

                }
                else if (msg == "Message")
                {
                    string[] data = md["Message"].ToString().Split('|');
                    string toIpPort = "";
                    foreach (var item in Users.listUser)
                    {
                        if (item.Username == data[0])
                        {
                            toIpPort = item.IpPort;
                            break;
                        }
                        
                    }
                    //MessageBox.Show(toIpPort, "send", MessageBoxButton.OK, MessageBoxImage.Information);
                    md["Message"] = data[1] + "|" + data[2] + "|" + data[3];
                    server.Send(toIpPort, "ReceivedMsg", md);
                }
            }
            catch (Exception e) {
                //server.Send(args.IpPort, "Unsuccess_signup");
                MessageBox.Show(e.Message);
            }
        }
    }
    public partial class MainWindow : Window
    {
        static SQLiteConnection dbConnection = serverTCP.dbConnection;
        bool flag1 = false;
        serverTCP server;
        public MainWindow()
        {
            InitializeComponent();
            serverTCP.reff = this;
        }

        private void startServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!flag1)
            {
                serverTCP.startServer();
                flag1 = true;
            } else
            {
                flag1 = false;
                serverTCP.server.Stop();
                startServerButton.Content = "روشن کردن سرور";
                statusTextBlock.Text = "غیر فعال";

            }

            
        }

    }
}
