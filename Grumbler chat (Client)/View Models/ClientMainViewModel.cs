using FileHelper_ClassLibrary;
using Grumbler_chat__Client_.Commands;
using Grumbler_chat__Client_.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
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
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;

namespace Grumbler_chat__Client_.View_Models
{


    public class ClientMainViewModel : BaseViewModel
    {
        public ClientMainWindow ClientMainWindows { get; set; }

        public RelayCommand LoadedCommand { get; set; }

        public RelayCommand MouseDownCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }
        //   public RelayCommand ServiceCommand { get; set; }
        //public RelayCommand JoinCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand SendMessageRightClickCommand { get; set; }
        public RelayCommand ConnectDisConnectCommand { get; set; }

        private Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byte[] receivedBuf = new byte[100002400];
        public static object obj = new object();
        public static object obj2 = new object();

        static System.Windows.Input.Cursor c1 = new System.Windows.Input.Cursor(System.Windows.Application.GetResourceStream(new Uri("../../Images/Cursor.cur", UriKind.RelativeOrAbsolute)).Stream);



        long sendMessagebuttoncounter = 0;
        long countentbuttoncounter = 0;
        long contentcontrolcounter = 0;

        public string username, name, surname, password, confirmPassword, email, profileimage;
        bool sendimage, sendfile, recordaudiofile, blockusers, removemessage;


        public string username2, name2, surname2, password2, confirmPassword2, email2, profileimage2;
        bool sendimage2 = true, sendfile2 = true, recordaudiofile2 = false, blockusers2 = true, removemessage2 = true;

        string path;


        public ICommand ServiceCommand { get; set; }

        public ICommand JoinCommand { get; set; }

        private object selectedViewModel;
        private object selectedViewModel2;

        public object SelectedViewModel
        {
            get { return selectedViewModel; }
            set
            {
                selectedViewModel = value;
                OnPropertyChanged(nameof(SelectedViewModel));
            }
        }




        DispatcherTimer dispatcherTimer = new DispatcherTimer();



        public ClientMainViewModel()
        {
            int threadcount = 1001;

            Thread[] threads = new Thread[threadcount];

            for (int i = 0; i < threadcount; i++)
            {
                threads[i] = new Thread(() =>
                {


                    LoopConnect();


                });
            }




            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(2000);

            dispatcherTimer.Tick += DispatcherTimer_Tick;


            ServiceCommand = new RelayCommand(NavigateToService);



            LoadedCommand = new RelayCommand((sender) =>
            {

                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    ClientMainWindows.Client_DockPanel.Cursor = c1;


                    dispatcherTimer.Start();



                    if (File.Exists("../../DataClient/LogInStatus.txt"))
                    {
                        File.Delete("../../DataClient/LogInStatus.txt");
                    }



                }));





            });

            MouseDownCommand = new RelayCommand((sender) =>
            {
                ClientMainWindows.MouseDown += ServerMainWindows_MouseDown;
            });

            CloseCommand = new RelayCommand((sender) =>
            {
                ClientMainWindows.Close();

                Environment.Exit(0);
            });


            SendMessageCommand = new RelayCommand((sender) =>
            {

                sendMessagebuttoncounter++;

                if (sendMessagebuttoncounter % 1 == 0)
                {
                    ClientMainWindows.SendMessageButton.Content = "Send text";


                }

                if (sendMessagebuttoncounter % 2 == 0)
                {
                    ClientMainWindows.SendMessageButton.Content = "Send image";
                }


                if (sendMessagebuttoncounter % 3 == 0)
                {
                    ClientMainWindows.SendMessageButton.Content = "Send file";
                }

            });


            ConnectDisConnectCommand = new RelayCommand((sender) =>
            {

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    countentbuttoncounter++;


                    ClientMainWindows.ConnectButton.Content = "Connected";

                    ClientMainWindows.ConnectButton.FontSize = 30;

                    if (!string.IsNullOrEmpty(ClientMainWindows.ClientNameTextBox.Text))
                    {
                        if (Directory.Exists("../../DataClient"))
                        {

                            if (File.Exists("../../DataClient/setting.txt"))
                            {
                                if (!string.IsNullOrEmpty(FileHelper.FileStreamRead("../../DataClient/setting.txt")))
                                {

                                    string[] words = FileHelper.FileStreamRead("../../DataClient/setting.txt").Split(' ');

                                    System.Windows.Forms.MessageBox.Show($" IP address: {words[0]} Port: {words[1]}");

                                    threadcount--;


                                    Thread thread = new Thread(LoopConnect);
                                    thread.Start();

                                    ClientMainWindows.ConnectButton.IsEnabled = false;
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
                    }

                    else
                    {
                        System.Windows.MessageBox.Show($"Enter Client's name", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                }));


            });


            SendMessageRightClickCommand = new RelayCommand((sender) =>
            {


                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {


                    if (ClientMainWindows.SendMessageButton.Content == "Send text")
                    {


                        Border border = new Border();
                        System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();

                        TextBlock textBlock = new TextBlock();


                        System.Windows.Controls.Label label = new System.Windows.Controls.Label();



                        textBlock.Text = $"\n\n  {ClientMainWindows.MessageTextBox.Text}  \n";

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


                        label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
                        if (ClientMainWindows.UserListbox.SelectedItem != null)
                        {
                            label.Content = $"\n {ClientMainWindows.ClientNameTextBox.Text} -> {ClientMainWindows.UserListbox.SelectedItem.ToString()} \n\n";
                        }


                        if (clientSocket.Connected)
                        {
                            string tmpStr = "";
                            foreach (var item in ClientMainWindows.UserListbox.SelectedItems)
                            {



                                tmpStr = item.ToString();


                                byte[] buffer = Encoding.UTF8.GetBytes(tmpStr + " :" + ClientMainWindows.MessageTextBox.Text + "\n" + ClientMainWindows.ClientNameTextBox.Text);
                                clientSocket.Send(buffer);
                                Thread.Sleep(200);

                            }
                            if (tmpStr.Equals(""))
                            {
                                System.Windows.Forms.MessageBox.Show("Select Listbox");
                            }
                            else
                            {

                                ClientMainWindows.HistoryListbox.Items.Add(border);
                            }


                        }



                    }

                    if (ClientMainWindows.SendMessageButton.Content == "Send image")
                    {


                        System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();

                        System.Windows.Controls.Image images = new System.Windows.Controls.Image();

                        System.Windows.Controls.Label label = new System.Windows.Controls.Label(); 

                        Border border = new Border();

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

                                    ClientMainWindows.MessageTextBox.Text = openFileDialog.FileName;

                                }

                            }


                            try
                            {


                                images.Source = new BitmapImage(new Uri(ClientMainWindows.MessageTextBox.Text, UriKind.RelativeOrAbsolute));
                                images.Width = 180;
                                images.Height = 180;

                                images.Stretch = Stretch.Fill;

                                images.Margin = new Thickness(5, 5, 5, 5);

                                System.Windows.Controls.Label labelPhoto = new System.Windows.Controls.Label();
                                labelPhoto.Height = 90;


                                label.Height = 40;

                                label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));

                                if (ClientMainWindows.UserListbox.SelectedIndex != -1)
                                {

                                    label.Content = $" {ClientMainWindows.ClientNameTextBox.Text} -> {ClientMainWindows.UserListbox.SelectedItem.ToString()}";
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

                            if (clientSocket.Connected)
                            {
                                string tmpStr = "";
                                foreach (var item in ClientMainWindows.UserListbox.SelectedItems)
                                {



                                    tmpStr = item.ToString();


                                    byte[] buffer = Encoding.UTF8.GetBytes(tmpStr + " : " + ClientMainWindows.ClientNameTextBox.Text + " : " + ClientMainWindows.MessageTextBox.Text);
                                    clientSocket.Send(buffer);
                                    Thread.Sleep(200);



                                    Task.Run(() =>
                                    {

                                        ClientMainWindows.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            FileHelper.SendFile(clientSocket, ClientMainWindows.MessageTextBox.Text);


                                        }));

                                    });

                                }
                                if (tmpStr.Equals(""))
                                {
                                    System.Windows.Forms.MessageBox.Show("Select Listbox");
                                }
                                else
                                {


                                    ClientMainWindows.HistoryListbox.Items.Add(border);


                                }


                            }
                        })), null);
                    }

                    if (ClientMainWindows.SendMessageButton.Content == "Send file")
                    {



                        Grid grid = new Grid();

                        System.Windows.Controls.Image images = new System.Windows.Controls.Image();

                        System.Windows.Controls.TextBlock textBlock = new System.Windows.Controls.TextBlock();

                        Border border = new Border();

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

                                    ClientMainWindows.MessageTextBox.Text = openFileDialog.FileName;

                                }

                            }




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




                                textBlock.TextWrapping = TextWrapping.Wrap;
                                textBlock.VerticalAlignment = VerticalAlignment.Top;

                                string[] words = Path.GetFileName(ClientMainWindows.MessageTextBox.Text).Split('-');




                                if (ClientMainWindows.MessageTextBox.Text.Length <= 10)
                                {
                                    try
                                    {
                                        textBlock.Text = $"\n {ClientMainWindows.ClientNameTextBox.Text} -> {ClientMainWindows.UserListbox.SelectedItem.ToString()} \n {Path.GetFileName(ClientMainWindows.MessageTextBox.Text)}";

                                    }
                                    catch (Exception)
                                    {


                                    }
                                }

                                else
                                {

                                    if (ClientMainWindows.UserListbox.SelectedItem != null)
                                    {

                                        textBlock.Text = $"\n {ClientMainWindows.ClientNameTextBox.Text} -> {ClientMainWindows.UserListbox.SelectedItem.ToString()} \n {words[0]}...{Path.GetExtension(ClientMainWindows.MessageTextBox.Text)} ";
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


                            if (clientSocket.Connected)
                            {
                                string tmpStr = "";
                                foreach (var item in ClientMainWindows.UserListbox.SelectedItems)
                                {



                                    tmpStr = item.ToString();


                                    byte[] buffer = Encoding.UTF8.GetBytes(tmpStr + " : " + ClientMainWindows.ClientNameTextBox.Text + " : " + ClientMainWindows.MessageTextBox.Text);
                                    clientSocket.Send(buffer);
                                    Thread.Sleep(200);


                                    Task.Run(() =>
                                    {

                                        ClientMainWindows.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            FileHelper.SendFile(clientSocket, ClientMainWindows.MessageTextBox.Text);


                                        }));

                                    });

                                }
                                if (tmpStr.Equals(""))
                                {
                                    System.Windows.Forms.MessageBox.Show("Select Listbox");
                                }
                                else
                                {


                                    ClientMainWindows.HistoryListbox.Items.Add(border);


                                }


                            }


                        })), null);

                    }

                }));

            });


        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {


            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (path != null)
                {

                    if (!Directory.Exists("../../../Files/"))
                    {
                        Directory.CreateDirectory("../../../Files").Create();
                    }
                }





            }));

        }

        private void ServerMainWindows_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ClientMainWindows.DragMove();


            }
        }

        private void NavigateToService(object obj)
        {
            SelectedViewModel = new Service_UCViewModel();


            contentcontrolcounter++;


            if (contentcontrolcounter % 2 != 0)
            {

                ClientMainWindows.ListBoxesGrid.Visibility = Visibility.Visible;
                ClientMainWindows.Client_ContentControl1.Visibility = Visibility.Hidden;


            }


            else
            {

                ClientMainWindows.ListBoxesGrid.Visibility = Visibility.Hidden;
                ClientMainWindows.Client_ContentControl1.Visibility = Visibility.Visible;

            }
        }







        private void ReceiveData(IAsyncResult ar)
        {


            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {



                    int listempty = 0;

                    Socket socket = (Socket)ar.AsyncState;
                    int received = socket.EndReceive(ar);
                    byte[] dataBuf = new byte[received];
                    Array.Copy(receivedBuf, dataBuf, received);
                    string inbox = Encoding.UTF8.GetString(dataBuf).ToString();
                    if (inbox.Contains("delete*"))
                    {
                        string parcala = inbox.Substring(4, (inbox.Length - 4));

                        for (int j = 0; j < ClientMainWindows.UserListbox.Items.Count; j++)
                        {
                            if (ClientMainWindows.UserListbox.Items[j].Equals(parcala))
                            {
                                ClientMainWindows.UserListbox.Items.RemoveAt(j);

                            }
                        }
                    }
                    else if (inbox.Contains("@"))
                    {

                        for (int i = 0; i < ClientMainWindows.UserListbox.Items.Count; i++)
                        {
                            if (ClientMainWindows.UserListbox.Items[i].ToString().Equals(inbox))
                            {
                                listempty = 1;
                            }
                        }
                        if (listempty == 0)
                        {


                            string ben = "@" + ClientMainWindows.ClientNameTextBox.Text;
                            if (ben.Equals(inbox))
                            {

                            }
                            else
                            {
                                ClientMainWindows.UserListbox.Items.Add(inbox);
                            }
                        }

                    }
                    else
                    {

                        string[] array = inbox.Split(new string[] { " : " }, 2, StringSplitOptions.RemoveEmptyEntries);

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


                                    System.Windows.Controls.Label label = new System.Windows.Controls.Label();



                                    textBlock.Text = "\n \n    " + remainingItems + "    \n" + "\n";

                                    textBlock.TextWrapping = TextWrapping.WrapWithOverflow;

                                    textBlock.FontSize = 25;

                                    textBlock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));


                                    label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 113, 88, 190));
                                    label.Content = $"\n\n  {firstItem}  -> {ClientMainWindows.ClientNameTextBox.Text.ToString()} \n";



                                    grid.Children.Add(textBlock);
                                    grid.Children.Add(label);



                                    border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 113, 88, 190));
                                    border.BorderThickness = new Thickness(10);
                                    border.CornerRadius = new CornerRadius(15);


                                    border.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                                    border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;


                                    border.Child = grid;



                                    ClientMainWindows.HistoryListbox.Items.Add(border);


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

                                        string savefile = $"../../DataClient/{guid}.{Path.GetExtension(remainingItems)}";

                                        int bufferSize = 10002400;


                                        Guid guid1 = Guid.NewGuid();


                                

                                            FileHelper.ReceiveFileClient(socket, remainingItems);

                                            Thread.Sleep(500);



                              



                                            images.Source = new BitmapImage(new Uri(Path.GetFullPath("../../Images/images.png")));
                                  

                                            images.Width = 150;
                                            images.Height = 150;

                                            images.Stretch = Stretch.Fill;

                                            images.Margin = new Thickness(5, 5, 5, 5);

                                            System.Windows.Controls.Label labelPhoto = new System.Windows.Controls.Label();
                                            labelPhoto.Height = 90;


                                            label.Height = 60;

                                            label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));



                                            string[] words = Path.GetFileName(remainingItems).Split('-');


                                            label.Content = $" {firstItem} -> {ClientMainWindows.ClientNameTextBox.Text.ToString()} \n {words[0]} \n";






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


                                            ClientMainWindows.HistoryListbox.Items.Add(border);
                             
                                    

                                }

                                else
                                {



                                    System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();

                                    System.Windows.Controls.Image images = new System.Windows.Controls.Image();

                                    System.Windows.Controls.Label label = new System.Windows.Controls.Label();

                                    Border border = new Border();






                                    Guid guid = Guid.NewGuid();

                                    string savefile = $"../../DataClient/{guid}.{Path.GetExtension(remainingItems)}";

                                    int bufferSize = 10002400;



                                    Guid guid1 = Guid.NewGuid();






                               



                                        FileHelper.ReceiveFileClient(socket, remainingItems);

                                        Thread.Sleep(500);


                                     


                                            images.Source = new BitmapImage(new Uri(Path.GetFullPath("../../Images/files.png")));



                                        images.Width = 175;
                                            images.Height = 175;

                                            images.Stretch = Stretch.Fill;

                                            images.Margin = new Thickness(5, 5, 5, 5);

                                            System.Windows.Controls.Label labelPhoto = new System.Windows.Controls.Label();
                                            labelPhoto.Height = 90;


                                            label.Height = 75;

                                            label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));


                                            string[] words = Path.GetFileName(remainingItems).Split('-');





                                            label.Content = $" {firstItem} -> {ClientMainWindows.ClientNameTextBox.Text.ToString()} \n {words[0]} \n";

                                            System.Windows.MessageBox.Show($"{Path.GetFullPath(remainingItems)}");



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


                                            ClientMainWindows.HistoryListbox.Items.Add(border);


                                  
                                }


                            }

                        }

                        catch (Exception ex)
                        {

                            MessageBox.Show($"{ex.Message}");
                        }

                    }

                    clientSocket.BeginReceive(receivedBuf, 0, receivedBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);



                }
                catch (Exception)
                {


                }

            }));

        }

        private void SendLoop()
        {


            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                while (true)
                {


                    byte[] receivedBuf = new byte[1000024];
                    int rev = clientSocket.Receive(receivedBuf);
                    if (rev != 0)
                    {
                        byte[] data = new byte[rev];
                        Array.Copy(receivedBuf, data, rev);

                        ClientMainWindows.HistoryListbox.Items.Add("\nServer: " + Encoding.UTF8.GetString(data) + "\n");
                    }
                    else clientSocket.Close();

                }
            }));

        }

        private void LoopConnect()
        {



            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                int attempts = 0;
                while (!clientSocket.Connected)
                {
                    try
                    {
                        attempts++;


                        if (Directory.Exists("../../DataClient"))
                        {

                            if (File.Exists("../../DataClient/setting.txt"))
                            {
                                if (!string.IsNullOrEmpty(FileHelper.FileStreamRead("../../DataClient/setting.txt")))
                                {

                                    string[] words = FileHelper.FileStreamRead("../../DataClient/setting.txt").Split(' ');

                                    System.Windows.Forms.MessageBox.Show($" IP address: {words[0]} Port: {words[1]}");

                                    try
                                    {
                                    clientSocket.Connect(IPAddress.Parse($"{words[0]}"), int.Parse(words[1]));

                                    }
                                    catch (Exception ex)
                                    {

                                        MessageBox.Show($"{ex.Message}");


                                        Thread.Sleep(1000);

                                        Environment.Exit(1);
                                    }
                                }
                            }
                        }


                    }
                    catch (SocketException ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                    }
                }
                clientSocket.BeginReceive(receivedBuf, 0, receivedBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);
                byte[] buffer = Encoding.UTF8.GetBytes("@@" + ClientMainWindows.ClientNameTextBox.Text);

                clientSocket.Send(buffer);



            }));



        }
    }
}
