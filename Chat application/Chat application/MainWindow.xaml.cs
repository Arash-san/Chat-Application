using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WatsonTcp;

namespace Chat_application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public class clientTCP
    {
        public static Grid usList;
        public static MainWindow reff;
        public static WatsonTcpClient client;
        public static void startClient(string address)
        {
            
            client = new WatsonTcpClient(address, 21341);
            client.Events.ServerConnected += ServerConnected;
            client.Events.ServerDisconnected += ServerDisconnected;
            client.Events.MessageReceived += MessageReceived;
            
            try
            {
                client.Connect();
                } catch(Exception) { MessageBox.Show("عدم توانایی در اتصال به سرور","اخطار",MessageBoxButton.OK,MessageBoxImage.Error); };
        }
        static void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            string msg = Encoding.UTF8.GetString(args.Data);
            Dictionary<object, object> md = new Dictionary<object, object>();
            md = args.Metadata;
            //MessageBox.Show(msg, "Client");
            try
            {
                if (msg == "Success_signup")
                {
                    MessageBox.Show("ثبت نام با موفقیت انجام شد", "پیغام", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        reff.loginSection.Visibility = Visibility.Hidden;
                        reff.appSection.Visibility = Visibility.Visible;
                        reff.userName.Text = reff.usernameTextBox.Text;
                    });
                }
                else if (msg == "Unsuccess_signup")
                {
                    MessageBox.Show("ثبت نام با خطا مواجه گشت", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (msg == "Success_login")
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        reff.loginSection.Visibility = Visibility.Hidden;
                        reff.appSection.Visibility = Visibility.Visible;
                        reff.userName.Text = reff.usernameTextBox.Text;
                    });

                }
                else if (msg == "Success_logout")
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        reff.loginSection.Visibility = Visibility.Visible;
                        reff.appSection.Visibility = Visibility.Hidden;
                        reff.clearChat();
                        reff.clearContact();
                        reff.currentUserText.Text = "";
                        reff.sendSection.Visibility = Visibility.Collapsed;
                        reff.entireHistory.Clear();

                    });

                }
                else if (msg == "Unsuccess_login")
                {
                    MessageBox.Show("ورود با خطا مواجه گشت", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if(msg == "userIsOn")
                {
                   
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        reff.addToContacts(reff.userTextBox.Text,false);
                        reff.entireHistory.Add(reff.userTextBox.Text, new List<textChat>());
                    });

                }
                else if(msg == "userIsOff")
                {
                    MessageBox.Show("کاربر آنلاین نمی باشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else if(msg == "userNotAvailable")
                {
                    MessageBox.Show("کاربری با این نام وجود ندارد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else if(msg == "ReceivedMsg")
                {
                    
                    string[] data = md["Message"].ToString().Split('|');

                    //MessageBox.Show("Text said: " + data[1]);
                    
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        
                        if (data[0] == reff.currentUserText.Text)
                        {
                            reff.addToChat(data[1],true,data[2]);
                            reff.entireHistory[data[0]].Add(new textChat(true, data[1], data[2]));
                        }
                        else
                        {
                            if (reff.entireHistory.ContainsKey(data[0]))
                            {
                                reff.entireHistory[data[0]].Add(new textChat(true, data[1], data[2]));
                                foreach (object child in reff.userList.Children)
                                {
                                    string childname = null;
                                    if (child is FrameworkElement)
                                    {
                                        childname = (child as FrameworkElement).Name;
                                    }

                                    if (childname == data[0])
                                    {
                                        //Border border = (Border)(child as FrameworkElement).FindName("newMessage");
                                        Border border = MainWindow.FindChild<Border>((Grid)(child as FrameworkElement), "newMessage");
                                        try
                                        {
                                            border.Visibility = Visibility.Visible;
                                        }
                                        catch (Exception){}
                                    }
                                }

                            }
                            else
                            {
                                List<textChat> tempList = new List<textChat>();
                                tempList.Add(new textChat(true, data[1], data[2]));
                                reff.entireHistory.Add(data[0], tempList);
                                reff.addToContacts(data[0], true);
                            }
                        }
                        
                    });
                    

                }
            }
            catch (Exception d) {
                MessageBox.Show(d.Message);
            }
            //Console.WriteLine("Message from " + args.IpPort + ": " + Encoding.UTF8.GetString(args.Data));
        }

        static void ServerConnected(object sender, ConnectionEventArgs args)
        {
            Console.WriteLine("Server " + args.IpPort + " connected");
        }

        static void ServerDisconnected(object sender, DisconnectionEventArgs args)
        {
            Console.WriteLine("Server " + args.IpPort + " disconnected");
        }

    }
    public class textChat
    {
        public bool isItOpponent { get; set; }
        public string text { get; set; }
        public string clock { get; set; }
        public textChat(bool isItOpponent, string text, string clock)
        {
            this.isItOpponent = isItOpponent;
            this.text = text;
            this.clock = clock;
        }
    }
    public partial class MainWindow : Window
    {
        public Dictionary<string, List<textChat>> entireHistory = new Dictionary<string, List<textChat>>();

        public MainWindow()
        {
            InitializeComponent();
            clientTCP.reff = this;
            loginSection.Visibility = Visibility.Visible;
            appSection.Visibility = Visibility.Collapsed;

        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            //clientTCP connection = new clientTCP(serverTextBox.Text);
            clientTCP.startClient(serverTextBox.Text);
            Dictionary<object, object> md = new Dictionary<object, object>();
            md["login"] = $"{usernameTextBox.Text}|{getHash(passwordTextBox.Password)}";
            clientTCP.client.Send("login", md);

        }   

        private void signupButton_Click(object sender, RoutedEventArgs e)
        {
            clientTCP.startClient(serverTextBox.Text);
            Dictionary<object, object> md = new Dictionary<object, object>();
            md["signup"] = $"{usernameTextBox.Text}|{getHash(passwordTextBox.Password)}";
            clientTCP.client.Send("signup",md);
            
        }

        private void addUserButton_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            addUserPopup.Visibility = Visibility.Visible;
        }
        private string changeToPersianNumber(string input)
        {
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            for (int j = 0; j < persian.Length; j++)
                input = input.Replace(j.ToString(), persian[j]);

            return input;
        }
        private void openContactEvent(object sender, MouseButtonEventArgs e)
        {
            sendSection.Visibility = Visibility.Visible;
            string tag = (string)((Grid)sender).Tag;
            currentUserText.Text = tag;
            //Border border = (Border)((Grid)sender).FindName("newMessage");

            Border border = FindChild<Border>((Grid)sender, "newMessage");
            try
            {
                border.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {}
            clearChat();
            foreach (var item in entireHistory[currentUserText.Text])
            {
                addToChat(item.text, item.isItOpponent, item.clock);
            }
        }

        private void addPopupButton_Click(object sender, RoutedEventArgs e)
        {
            addUserPopup.Visibility = Visibility.Hidden;
            Dictionary<object, object> md = new Dictionary<object, object>();
            md["isOnline"] = userTextBox.Text;
            clientTCP.client.Send("isOnline", md);

        }
        private void closePopupButton_Click(object sender, RoutedEventArgs e)
        {
            addUserPopup.Visibility = Visibility.Hidden;
        }

        private void sendButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Dictionary<object, object> md = new Dictionary<object, object>();
            md["Message"] = $"{currentUserText.Text}|{userName.Text}|{msgTextBox.Text}|{DateTime.Now.ToString("HH:mm")}";
            addToChat(msgTextBox.Text, false, DateTime.Now.ToString("HH:mm"));
            entireHistory[currentUserText.Text].Add(new textChat(false, msgTextBox.Text, DateTime.Now.ToString("HH:mm")));
            msgTextBox.Text = "";
            clientTCP.client.Send("Message", md);
        }

        private void exitUserButton_MouseUp(object sender, MouseButtonEventArgs e)
        {

            Dictionary<object, object> md = new Dictionary<object, object>();
            clientTCP.client.Send("logout");
        }

        public void addToChat(string text, bool kind, string clock)
        {
            Border border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(10),
                Margin = new Thickness(0,0,20,10),
                CornerRadius =new CornerRadius(20),
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#121212")),
                MaxWidth = 420
            };
            if (kind)
            {
                border.HorizontalAlignment = HorizontalAlignment.Right;
                border.Background = Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#353535"));
            }
            Grid grid = new Grid();
            ColumnDefinition col1 = new ColumnDefinition
            {
                Width = new GridLength(50)
            };
            ColumnDefinition col2 = new ColumnDefinition();
            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);
            TextBlock txtBlock1 = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Left,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Vazir"),
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            if(kind)
            {
                txtBlock1.HorizontalAlignment = HorizontalAlignment.Right;
            }
            Grid.SetColumn(txtBlock1, 1);
            TextBlock txtBlock2 = new TextBlock
            {
                Text = changeToPersianNumber(clock),
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Left,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Vazir"),
                FontSize = 10,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Grid.SetColumn(txtBlock2, 0);
            grid.Children.Add(txtBlock1);
            grid.Children.Add(txtBlock2);

            border.Child = grid;
            textChat.Children.Add(border);

        }
        public void clearChat()
        {
            textChat.Children.Clear();
            

        }
        public void clearContact()
        {
            userList.Children.Clear();

        }

        public void addToContacts(string userName, bool isNewMsg)
        {
            string[] colors = {"#00695c", "#6d4c41", "#ef6c00","#0091ea","#0091ea","#9c27b0" };
            Grid grid = new Grid
            {
                Height = 50,
                Margin = new Thickness(0, 0, 0, 10),
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#090909")),
                Cursor = Cursors.Hand,
                Tag = userName,
                Name = userName
                
            };
            grid.MouseUp += new MouseButtonEventHandler(openContactEvent);
            ColumnDefinition col1 = new ColumnDefinition
            {
                Width = new GridLength(68)
            };
            ColumnDefinition col2 = new ColumnDefinition();
            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);
            Random rnd = new Random();
            Border border = new Border
            {
                Width = 40,
                Height = 40,
                CornerRadius = new CornerRadius(50),
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(20,0,0,0),
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(colors[new Random().Next(colors.Length)]))
            };
            Border border1 = new Border
            {
                Width = 10,
                Height = 10,
                Margin = new Thickness(0,0,8,5),
                CornerRadius = new CornerRadius(50),
                Background = Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Visibility = Visibility.Collapsed,
                Name = "newMessage"
            };
            if (isNewMsg == true)
            {
                border1.Visibility = Visibility.Visible;
            }
            grid.Children.Add(border);
            grid.Children.Add(border1);

            TextBlock textBlock1 = new TextBlock
            {
                Text = userName.Substring(0, 2).ToUpperInvariant(),
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = Brushes.White,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Vazir"),
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 30,
                Margin = new Thickness(29,5,0,0)
            };
            grid.Children.Add(textBlock1);

            TextBlock textBlock2 = new TextBlock
            {
                Text = userName,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(0, 5, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.White,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Vazir"),
                FontSize = 16
                
            };
            grid.Children.Add(textBlock2);
            Grid.SetColumn(textBlock2, 1);
            userList.Children.Add(grid);
        }

        public String getHash(String pass)
        {
            var data = Encoding.Unicode.GetBytes(pass);
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1data = sha1.ComputeHash(data);
            return Convert.ToBase64String(sha1data);
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

    }
}
