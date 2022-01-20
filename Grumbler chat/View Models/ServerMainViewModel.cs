using FileHelper_ClassLibrary;
using Grumbler_chat_Server.Commands;
using Grumbler_chat_Server.Views;
using Grumbler_chat_Server.Views.UserControls;
using Socket_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Grumbler_chat_Server.View_Models
{

    public class ServerMainViewModel : BaseViewModel, IDisposable
    {
        public ServerMainWindow ServerMainWindows { get; set; }

        public Service_UC Service_UCs { get; set; }

        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand MouseDownCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }
        public RelayCommand ServiceCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand SendMessageRightClickCommand { get; set; }
        public RelayCommand ConnectDisConnectCommand { get; set; }



        public static object obj = new object();



        static System.Windows.Input.Cursor c1 = new System.Windows.Input.Cursor(System.Windows.Application.GetResourceStream(new Uri("../../Images/Cursor.cur", UriKind.RelativeOrAbsolute)).Stream);

        private byte[] _buffer = new byte[1000024];


        public List<SocketClass> ClientSockets { get; set; }

        private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        long sendMessagebuttoncounter = 0;
        long contentcontrolcounter = 0;

        private object selectedViewModel;

        public object SelectedViewModel
        {
            get { return selectedViewModel; }
            set
            {
                selectedViewModel = value;
                OnPropertyChanged(nameof(SelectedViewModel));
            }
        }

        DispatcherTimer timer = new DispatcherTimer();


        public ServerMainViewModel()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 2, 0);

            timer.Tick += Timer_Tick;




            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {

                ClientSockets = new List<SocketClass>();


            }));



            int threadcount = 1001;

            Thread[] threads = new Thread[threadcount];

            for (int i = 0; i < threadcount; i++)
            {
                threads[i] = new Thread(() =>
                {


                    Server();


                });
            }



            ServiceCommand = new RelayCommand((sender) =>
            {

                NavigateToService(sender);


            });

            LoadedCommand = new RelayCommand((sender) =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    ServerMainWindows.Server_DockPanel.Cursor = c1;
                    timer.Start();



                }));



            });

            MouseDownCommand = new RelayCommand((sender) =>
            {
                ServerMainWindows.MouseDown += ServerMainWindows_MouseDown;
            });

            CloseCommand = new RelayCommand((sender) =>
            {
                ServerMainWindows.Close();

                Environment.Exit(0);
            });


            SendMessageCommand = new RelayCommand((sender) =>
            {

                sendMessagebuttoncounter++;

                if (sendMessagebuttoncounter % 1 == 0)
                {
                    ServerMainWindows.SendMessageButton.Content = "Send text";

                }

                if (sendMessagebuttoncounter % 2 == 0)
                {
                    ServerMainWindows.SendMessageButton.Content = "Send image";
                }


                if (sendMessagebuttoncounter % 3 == 0)
                {
                    ServerMainWindows.SendMessageButton.Content = "Send file";
                }

            });

            ConnectDisConnectCommand = new RelayCommand((sender) =>
            {
                System.Windows.MessageBox.Show($"{Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"))}DataServer");

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {


                    ServerMainWindows.ConnectButton.Content = "Connected";

                    ServerMainWindows.ConnectButton.FontSize = 30;

                    if (Directory.Exists("../../DataServer"))
                    {

                        if (File.Exists("../../DataServer/setting.txt"))
                        {
                            if (!string.IsNullOrEmpty(FileHelper.FileStreamRead("../../DataServer/setting.txt")))
                            {

                                string[] words = FileHelper.FileStreamRead("../../DataServer/setting.txt").Split(' ');

                                System.Windows.Forms.MessageBox.Show($" IP address: {words[0]} Port: {words[1]}");

                                threadcount--;

                                threads.ElementAt(threadcount).Start();

                                ServerMainWindows.ConnectButton.IsEnabled = false;
                            }
                            else
                            {
                                System.Windows.MessageBox.Show($"Fields of IP configuration are not filled", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show($"Change setting", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);


                        }

                    }

                    else
                    {
                        System.Windows.MessageBox.Show($"Change setting", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);


                    }


                }));







            });

            SendMessageRightClickCommand = new RelayCommand((sender) =>
            {


                if (ServerMainWindows.SendMessageButton.Content == "Send text")
                {







                    for (int i = 0; i < ServerMainWindows.UserListbox.SelectedItems.Count; i++)
                    {
                        string t = ServerMainWindows.UserListbox.SelectedItems[i].ToString();


                        for (int j = 0; j < ClientSockets.Count; j++)
                        {
                            if (ClientSockets[j]._Socket.Connected && ClientSockets[j]._Name.Equals("@" + t))

                            {
                                Sendata(ClientSockets[j]._Socket, $"Server : {ServerMainWindows.MessageTextBox.Text}");


                            }



                        }
                        for (int j = 0; j < ClientSockets.Count; j++)
                        {
                            Border border = new Border();
                            System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();

                            TextBlock textBlock = new TextBlock();


                            System.Windows.Controls.Label label = new System.Windows.Controls.Label();



                            textBlock.Text = $"\n\n  {ServerMainWindows.MessageTextBox.Text}  \n";

                            textBlock.TextWrapping = TextWrapping.WrapWithOverflow;

                            textBlock.FontSize = 25;

                            textBlock.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#7158BE"));

                            grid.Children.Add(label);
                            grid.Children.Add(textBlock);



                            border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 113, 88, 190));
                            border.BorderThickness = new Thickness(10);
                            border.CornerRadius = new CornerRadius(15);


                            border.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                            border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;


                            border.Child = grid;

                            for (int k = 0; k < ClientSockets.Count; k++)
                            {
                                label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
                                label.Content = $"\n Server -> {ServerMainWindows.UserListbox.SelectedItem.ToString()} \n\n";
                            }

                            ServerMainWindows.HistoryListbox.Items.Add(border);
                        }





                    }

                }

                if (ServerMainWindows.SendMessageButton.Content == "Send image")
                {


                    Dispatcher.CurrentDispatcher.Invoke(((Action)(() =>
                    {

                        using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                        {

                            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                            openFileDialog.Filter = "jpeg Files(*.jpeg)|*.jpeg|jpg Files(*.jpg)|*.jpg|png Files(*.png)|*.png|gif Files(*.gif)|*.gif|tiff Files(*.tiff)|*.tiff|raw Files(*.raw)|*.raw|bmp Files(*.bmp)|*.bmp|ico Files(*.ico)|*.ico|" + "All files(*.*) | *.*";
                            openFileDialog.FilterIndex = 2;
                            openFileDialog.RestoreDirectory = true;
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {

                                ServerMainWindows.MessageTextBox.Text = openFileDialog.FileName;

                            }

                        }

                    })), null);


                    for (int i = 0; i < ServerMainWindows.UserListbox.SelectedItems.Count; i++)
                    {
                        string t = ServerMainWindows.UserListbox.SelectedItems[i].ToString();


                        for (int j = 0; j < ClientSockets.Count; j++)
                        {
                            if (ClientSockets[j]._Socket.Connected && ClientSockets[j]._Name.Equals("@" + t))

                            {
                                Sendatafile(ClientSockets[j]._Socket, $"Server : {ServerMainWindows.MessageTextBox.Text}");


                            }



                        }
                        for (int j = 0; j < ClientSockets.Count; j++)
                        {

                            System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();

                            System.Windows.Controls.Image images = new System.Windows.Controls.Image();

                            System.Windows.Controls.Label label = new System.Windows.Controls.Label();

                            Border border = new Border();






                            try
                            {


                                images.Source = new BitmapImage(new Uri(ServerMainWindows.MessageTextBox.Text));
                                images.Width = 200;
                                images.Height = 200;

                                images.Stretch = Stretch.Fill;

                                images.Margin = new Thickness(5, 5, 5, 5);

                                System.Windows.Controls.Label labelPhoto = new System.Windows.Controls.Label();
                                labelPhoto.Height = 90;


                                label.Height = 40;

                                label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));


                                for (int k = 0; k < ClientSockets.Count; k++)
                                {
                                    label.Content = $" Server -> {ServerMainWindows.UserListbox.SelectedItem.ToString()}";
                                }
                            }
                            catch (Exception ex)
                            {

                                System.Windows.Forms.MessageBox.Show($"{ex.Message}");
                            }


                            border.Margin = new Thickness(5, 5, 5, 5);
                            border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 113, 88, 190));
                            border.BorderThickness = new Thickness(10);
                            border.CornerRadius = new CornerRadius(15);

                            border.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                            border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;


                            RowDefinition rowDefinition = new RowDefinition();

                            RowDefinition rowDefinition2 = new RowDefinition();

                            grid.RowDefinitions.Add(rowDefinition);
                            grid.RowDefinitions.Add(rowDefinition2);

                            Grid.SetRow(label, 0);
                            Grid.SetRow(images, 1);

                            grid.Children.Add(label);
                            grid.Children.Add(images);

                            border.Child = grid;




                            ServerMainWindows.HistoryListbox.Items.Add(border);

                        }
                    }

                }

                if (ServerMainWindows.SendMessageButton.Content == "Send file")
                {




                    Dispatcher.CurrentDispatcher.Invoke(((Action)(() =>
                    {

                        using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                        {

                            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                            openFileDialog.Filter = "pdf Files(*.pdf)|*.pdf|txt Files(*.txt)|*.txt|doc Files(*.doc)|*.doc|sql Files(*.sql)|*.sql|xml Files(*.xml)|*.xml|json Files(*.json)|*.json|epub Files(*.epub)|*.epub|djvu Files(*.djvu)|*.djvu|" + "All files(*.*) | *.*";
                            openFileDialog.FilterIndex = 2;
                            openFileDialog.RestoreDirectory = true;
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {

                                ServerMainWindows.MessageTextBox.Text = openFileDialog.FileName;

                            }

                        }

                    })), null);



                    for (int i = 0; i < ServerMainWindows.UserListbox.SelectedItems.Count; i++)
                    {
                        string t = ServerMainWindows.UserListbox.SelectedItems[i].ToString();


                        for (int j = 0; j < ClientSockets.Count; j++)
                        {
                            if (ClientSockets[j]._Socket.Connected && ClientSockets[j]._Name.Equals("@" + t))

                            {
                                Sendatafile(ClientSockets[j]._Socket, $"Server : {ServerMainWindows.MessageTextBox.Text}");


                            }



                        }
                        for (int j = 0; j < ClientSockets.Count; j++)
                        {
                            Grid grid = new Grid();

                            System.Windows.Controls.Image images = new System.Windows.Controls.Image();

                            System.Windows.Controls.TextBlock textBlock = new System.Windows.Controls.TextBlock();

                            Border border = new Border();



                            try
                            {


                                images.Source = new BitmapImage(new Uri("../../Images/files.png", UriKind.RelativeOrAbsolute));
                                images.Width = 200;
                                images.Height = 200;

                                images.Stretch = Stretch.Fill;

                                images.Margin = new Thickness(5, 5, 5, 5);

                                System.Windows.Controls.Label labelPhoto = new System.Windows.Controls.Label();
                                labelPhoto.Height = 90;


                                textBlock.Height = 55;

                                textBlock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));



                                for (int k = 0; k < ClientSockets.Count; k++)
                                {
                                    textBlock.TextWrapping = TextWrapping.Wrap;
                                    textBlock.VerticalAlignment = VerticalAlignment.Top;

                                    string[] words = Path.GetFileName(ServerMainWindows.MessageTextBox.Text).Split('-');




                                    if (ServerMainWindows.MessageTextBox.Text.Length <= 10)
                                    {
                                        textBlock.Text = $" Server -> {ServerMainWindows.UserListbox.SelectedItem.ToString()} \n {Path.GetFileName(ServerMainWindows.MessageTextBox.Text)}";
                                    }

                                    else
                                    {



                                        textBlock.Text = $" Server -> {ServerMainWindows.UserListbox.SelectedItem.ToString()} \n {words[0]}...{Path.GetExtension(ServerMainWindows.MessageTextBox.Text)} ";


                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                                System.Windows.Forms.MessageBox.Show($"{ex.Message}");
                            }


                            border.Margin = new Thickness(5, 5, 5, 5);
                            border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 113, 88, 190));
                            border.BorderThickness = new Thickness(10);
                            border.CornerRadius = new CornerRadius(15);

                            border.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                            border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;


                            RowDefinition rowDefinition = new RowDefinition();

                            RowDefinition rowDefinition2 = new RowDefinition();

                            GridLengthConverter gridLengthConverter = new GridLengthConverter();

                            rowDefinition.Height = (GridLength)gridLengthConverter.ConvertFrom("2*");

                            grid.RowDefinitions.Add(rowDefinition);
                            grid.RowDefinitions.Add(rowDefinition2);

                            Grid.SetRow(textBlock, 0);
                            Grid.SetRow(images, 1);

                            grid.Children.Add(textBlock);
                            grid.Children.Add(images);

                            border.Child = grid;




                            ServerMainWindows.HistoryListbox.Items.Add(border);

                        }
                    }
                }

            });


        }

        private void Sendatafile(Socket socket, string v)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(v);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);

               


                string[] array = v.Split(new string[] { " : " }, 2, StringSplitOptions.RemoveEmptyEntries);

                string firstItem = array[1];


                FileHelper.SendFile(socket, firstItem);



            }
            catch (Exception)
            {


            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {


            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {


                if (!Directory.Exists("../../../Files/"))
                {
                    Directory.CreateDirectory("../../../Files").Create();
                }


            }));


        }

        private void ServerMainWindows_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ServerMainWindows.DragMove();


            }
        }

        private void NavigateToService(object obj)
        {
            SelectedViewModel = new Service_UCViewModel();


            contentcontrolcounter++;


            if (contentcontrolcounter % 2 != 0)
            {

                ServerMainWindows.ListBoxesGrid.Visibility = Visibility.Visible;
                ServerMainWindows.Service_ContentControl.Visibility = Visibility.Hidden;

            }


            else
            {

                ServerMainWindows.ListBoxesGrid.Visibility = Visibility.Hidden;
                ServerMainWindows.Service_ContentControl.Visibility = Visibility.Visible;

            }
        }

        private void Server()
        {


            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {

                try
                {



                    if (Directory.Exists("../../DataServer"))
                    {

                        if (File.Exists("../../DataServer/setting.txt"))
                        {

                            if (!string.IsNullOrEmpty(FileHelper.FileStreamRead("../../DataServer/setting.txt")))
                            {

                                string[] words = FileHelper.FileStreamRead("../../DataServer/setting.txt").Split(' ');

                                System.Windows.Forms.MessageBox.Show($"{words[0]} {words[1]}");

                                serverSocket.Bind(new IPEndPoint(IPAddress.Parse($"{words[0]}"), int.Parse(words[1])));
                                serverSocket.Listen(5);
                                serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
                            }
                        }
                    }

                }
                catch (Exception)
                {


                }

            }));


        }

        private void AppceptCallback(IAsyncResult ar)
        {

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {



                Socket socket = serverSocket.EndAccept(ar);
                ClientSockets.Add(new SocketClass(socket));
                ServerMainWindows.UserListbox.Items.Add(socket.RemoteEndPoint.ToString());

                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);

                serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);



            }));


        }

        static string fineshed_clien = "";

        private void ReceiveCallback(IAsyncResult ar)
        {


            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {

                Socket socket = (Socket)ar.AsyncState;

                try
                {

                    if (socket.Connected)
                    {
                        int received;
                        try
                        {
                            received = socket.EndReceive(ar);
                        }
                        catch (Exception)
                        {
                            // 
                            for (int i = 0; i < ClientSockets.Count; i++)
                            {
                                if (ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                                {

                                    fineshed_clien = ClientSockets[i]._Name.Substring(1, ClientSockets[i]._Name.Length - 1);

                                    ClientSockets.RemoveAt(i);

                                    for (int j = 0; j < ServerMainWindows.UserListbox.Items.Count; j++)
                                    {
                                        if (ServerMainWindows.UserListbox.Items[j].Equals(fineshed_clien))
                                        {
                                            ServerMainWindows.UserListbox.Items.RemoveAt(j);

                                        }
                                    }
                                }
                            }
                            delete_client(fineshed_clien);

                            return;
                        }
                        if (received != 0)
                        {
                            byte[] dataBuf = new byte[received];

                            Array.Copy(_buffer, dataBuf, received);


                            string text = Encoding.UTF8.GetString(dataBuf);

                            string reponse = string.Empty;

                            if (text.Contains("@@"))
                            {
                                for (int i = 0; i < ServerMainWindows.UserListbox.Items.Count; i++)
                                {
                                    if (socket.RemoteEndPoint.ToString().Equals(ClientSockets[i]._Socket.RemoteEndPoint.ToString()))
                                    {
                                        ServerMainWindows.UserListbox.Items.RemoveAt(i);
                                        ServerMainWindows.UserListbox.Items.Insert(i, text.Substring(1, text.Length - 1));
                                        ClientSockets[i]._Name = text;
                                        socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                                        shownames();

                                        return;
                                    }
                                }
                            }
                            int index = text.IndexOf(" ");
                            string cli = text.Substring(0, index);

                            string mesaj = "";
                            int uzunluk = (text.Length) - (index + 2);
                            index = index + 2;
                            mesaj = text.Substring(index, uzunluk);
                            sendmessage(cli, text, mesaj);
                            for (int i = 0; i < ClientSockets.Count; i++)
                            {
                                if (socket.RemoteEndPoint.ToString().Equals(ClientSockets[i]._Socket.RemoteEndPoint.ToString()))
                                {




                                    string[] array = mesaj.Split(new string[] { " : " }, 2, StringSplitOptions.RemoveEmptyEntries);

                                    string firstItem = array[0];

                                    string remainingItems = string.Join(" ", array.Skip(1).ToList());

                          

                                    try
                                    {

                                        if (!Path.HasExtension(remainingItems))
                                        {




                                            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                            {
                                                Border border = new Border();
                                                System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();

                                                TextBlock textBlock = new TextBlock();





                                                textBlock.Text = "\n \n " + firstItem + " -> " + cli + "\n" + "\n";

                                                textBlock.TextWrapping = TextWrapping.WrapWithOverflow;

                                                textBlock.FontSize = 25;

                                                textBlock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));






                                                grid.Children.Add(textBlock);




                                                border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 113, 88, 190));
                                                border.BorderThickness = new Thickness(10);
                                                border.CornerRadius = new CornerRadius(15);


                                                border.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                                                border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;


                                                border.Child = grid;

                           


                                                ServerMainWindows.HistoryListbox.Items.Add(border);


                                            }));


                                        }

                                        if (Path.HasExtension(remainingItems))
                                        {





                                            if (".jpeg" == Path.GetExtension(remainingItems) || ".jpg" == Path.GetExtension(remainingItems) || ".png" == Path.GetExtension(remainingItems) || ".gif" == Path.GetExtension(remainingItems) || ".tiff" == Path.GetExtension(remainingItems) || ".raw" == Path.GetExtension(remainingItems) || ".bmp" == Path.GetExtension(remainingItems) || ".ico" == Path.GetExtension(remainingItems))
                                            {

                                                System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();

                                                System.Windows.Controls.Image images = new System.Windows.Controls.Image();

                                                System.Windows.Controls.Label label = new System.Windows.Controls.Label();

                                                Border border = new Border();



                                                Guid guid = Guid.NewGuid();

                                                string savefile = $"../../DataServer/{guid} {Path.GetFileName(remainingItems)}";

                                                int bufferSize = 10002400;


                                                FileHelper.ReceiveFileServer(socket, remainingItems);

                                                Thread.Sleep(200);


                                                images.Source = new BitmapImage(new Uri(Path.GetFullPath(remainingItems)));
                                                images.Width = 200;
                                                images.Height = 200;

                                                images.Stretch = Stretch.Fill;

                                                images.Margin = new Thickness(5, 5, 5, 5);

                                                System.Windows.Controls.Label labelPhoto = new System.Windows.Controls.Label();
                                                labelPhoto.Height = 90;


                                                label.Height = 60;

                                                label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));



                                                label.Content = $"\n {firstItem} -> {cli} \n";

                                                // System.Windows.MessageBox.Show($"{Path.GetFullPath(remainingItems)}");



                                                border.Margin = new Thickness(5, 5, 5, 5);
                                                border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 113, 88, 190));
                                                border.BorderThickness = new Thickness(10);
                                                border.CornerRadius = new CornerRadius(15);

                                                border.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                                                border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;


                                                RowDefinition rowDefinition = new RowDefinition();

                                                RowDefinition rowDefinition2 = new RowDefinition();

                                                grid.RowDefinitions.Add(rowDefinition);
                                                grid.RowDefinitions.Add(rowDefinition2);

                                                Grid.SetRow(label, 0);
                                                Grid.SetRow(images, 1);

                                                grid.Children.Add(label);
                                                grid.Children.Add(images);

                                                border.Child = grid;


                                                ServerMainWindows.HistoryListbox.Items.Add(border);
                                            }

                                            else
                                            {

                                                System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();

                                                System.Windows.Controls.Image images = new System.Windows.Controls.Image();

                                                System.Windows.Controls.Label label = new System.Windows.Controls.Label();

                                                Border border = new Border();




                                                Guid guid = Guid.NewGuid();

                                                string savefile = $"{Environment.CurrentDirectory})/{guid} {Path.GetFileName(remainingItems)}";

                                                int bufferSize = 10002400;


                                                FileHelper.ReceiveFileServer(socket, savefile);



                                                images.Source = new BitmapImage(new Uri(Path.GetFullPath("../../Images/files.png")));
                                                images.Width = 180;
                                                images.Height = 180;

                                                images.Stretch = Stretch.Fill;

                                                images.Margin = new Thickness(5, 5, 5, 5);

                                                System.Windows.Controls.Label labelPhoto = new System.Windows.Controls.Label();
                                                labelPhoto.Height = 90;


                                                label.Height = 75;

                                                label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));



                                                label.Content = $"\n {firstItem} -> {cli} \n  {remainingItems}";


                                                string[] words = Path.GetFileName(remainingItems).Split('-');




                                                if (ServerMainWindows.MessageTextBox.Text.Length <= 10)
                                                {
                                                    label.Content = $"\n {firstItem} -> {cli} \n {Path.GetFileName(remainingItems)}";
                                                }

                                                else
                                                {

                                                    if (ServerMainWindows.UserListbox.SelectedItem != null)
                                                    {

                                                        label.Content = $"\n {firstItem} -> {cli} \n {words[0]}...{Path.GetFileName(remainingItems)} ";
                                                    }


                                                }


                                                border.Margin = new Thickness(5, 5, 5, 5);
                                                border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 113, 88, 190));
                                                border.BorderThickness = new Thickness(10);
                                                border.CornerRadius = new CornerRadius(15);

                                                border.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                                                border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;


                                                RowDefinition rowDefinition = new RowDefinition();

                                                RowDefinition rowDefinition2 = new RowDefinition();

                                                grid.RowDefinitions.Add(rowDefinition);
                                                grid.RowDefinitions.Add(rowDefinition2);

                                                Grid.SetRow(label, 0);
                                                Grid.SetRow(images, 1);

                                                grid.Children.Add(label);
                                                grid.Children.Add(images);

                                                border.Child = grid;


                                                ServerMainWindows.HistoryListbox.Items.Add(border);
                                            }


                                        }

                                    }
                                    catch (Exception ex)
                                    {

                                        System.Windows.MessageBox.Show($"{ex.Message}");
                                    }

                                }
                            }



                        }
                        else
                        {
                            for (int i = 0; i < ClientSockets.Count; i++)
                            {
                                if (ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                                {
                                    ClientSockets.RemoveAt(i);

                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {


                }
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);

            }));
        }

        private void shownames()
        {

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                for (int j = 0; j < ClientSockets.Count; j++)
                {
                    if (ClientSockets[j]._Socket.Connected)
                    {
                        for (int i = 0; i < ServerMainWindows.UserListbox.Items.Count; i++)
                        {


                            Sendata(ClientSockets[j]._Socket, ServerMainWindows.UserListbox.Items[i].ToString());

                            Thread.Sleep(200);
                        }

                    }
                }

            }));


        }

        private void sendmessage(string cli, string text, string mesaj)
        {

            string parcc = text.Substring(2, 2);

            cli = "@" + cli;


            if (cli.Equals(ClientSockets[0]._Name))
            {

            }



            for (int j = 0; j < ClientSockets.Count; j++)
            {
                if (ClientSockets[j]._Socket.Connected)
                {
                    if (ClientSockets[j]._Name.Equals(cli))
                    {

                        Sendata(ClientSockets[j]._Socket, mesaj);
                        Thread.Sleep(200);
                    }

                }
            }


        }

        private void delete_client(string fineshed_clien)
        {
            string sil = "delete*" + fineshed_clien;
            for (int j = 0; j < ClientSockets.Count; j++)
            {
                if (ClientSockets[j]._Socket.Connected)
                {



                    Sendata(ClientSockets[j]._Socket, sil);

                    Thread.Sleep(200);


                }
            }
        }

        private void Sendata(Socket socket, string txt)
        {
            try
            {

                byte[] data = Encoding.UTF8.GetBytes(txt);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
                

            }
            catch (Exception)
            {


            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }


        public void Dispose()
        {

        }
    }

}
