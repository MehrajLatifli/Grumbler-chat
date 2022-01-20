using FileHelper_ClassLibrary;
using Grumbler_chat__Client_.Commands;
using Grumbler_chat__Client_.Views;
using Grumbler_chat__Client_.Views.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Grumbler_chat__Client_.View_Models
{
    public class Service_UCViewModel : BaseViewModel
    {
        public Service_UC Service_UCs { get; set; }


        public RelayCommand SaveConnectCommand { get; set; }
        public RelayCommand SaveUpdateButton { get; set; }

        public RelayCommand SelectImageButton { get; set; }





        public Service_UCViewModel()
        {


            SaveConnectCommand = new RelayCommand((sender) =>
            {

                Task.Run(() =>
                {

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        if (!Directory.Exists("../../DataClient"))
                        {
                            Directory.CreateDirectory("../../DataClient").Create();

                            if (!File.Exists("../../DataClient/setting.txt"))
                            {
                                File.Create("../../DataClient/setting.txt").Close();

                                if (!string.IsNullOrEmpty(Service_UCs.IPaddressTextBox.Text) && !string.IsNullOrEmpty(Service_UCs.PortNumerTextBox.Text))
                                {
                                    FileHelper.FileStreamWrite("../../DataClient/setting.txt", Service_UCs.IPaddressTextBox.Text + " \n" + Service_UCs.PortNumerTextBox.Text);

                                    string[] words = FileHelper.FileStreamRead("../../DataClient/setting.txt").Split(' ');

                                }

                            }
                        }

                        else
                        {
                            File.Delete("../../DataClient/setting.txt");

                            DirectoryInfo di = new DirectoryInfo("../../DataClient");

                            foreach (FileInfo file in di.GetFiles())
                            {
                                file.Delete();
                            }
                            foreach (DirectoryInfo dir in di.GetDirectories())
                            {
                                dir.Delete(true);
                            }
                            Directory.Delete("../../DataClient");

                            if (!Directory.Exists("../../DataClient"))
                            {
                                Directory.CreateDirectory("../../DataClient").Create();

                                if (!File.Exists("../../DataClient/setting.txt"))
                                {
                                    File.Create("../../DataClient/setting.txt").Close();

                                    if (!string.IsNullOrEmpty(Service_UCs.IPaddressTextBox.Text) && !string.IsNullOrEmpty(Service_UCs.PortNumerTextBox.Text))
                                    {
                                        FileHelper.FileStreamWrite("../../DataClient/setting.txt", Service_UCs.IPaddressTextBox.Text + " \n" + Service_UCs.PortNumerTextBox.Text);

                                        FileHelper.FileStreamRead("../../DataClient/setting.txt");

                                        string[] words = FileHelper.FileStreamRead("../../DataClient/setting.txt").Split(' ');

                                    }

                                }
                            }
                        }


                    }));


                });

            });


            SaveUpdateButton = new RelayCommand((sender) =>
            {



            });

            SelectImageButton = new RelayCommand((sender) =>
            {



            });
        }
    }
}
