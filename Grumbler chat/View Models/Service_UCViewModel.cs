using FileHelper_ClassLibrary;
using Grumbler_chat_Server.Commands;
using Grumbler_chat_Server.Views.UserControls;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Grumbler_chat_Server.View_Models
{
    public class Service_UCViewModel : BaseViewModel
    {
        public Service_UC Service_UCs { get; set; }

        public RelayCommand SaveConnectCommand { get; set; }
        public RelayCommand SavePermissionCommand { get; set; }






        public Service_UCViewModel()
        {


            SaveConnectCommand = new RelayCommand((sender) =>
            {

                Task.Run(() =>
                {

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        if (!Directory.Exists("../../DataServer"))
                        {
                            Directory.CreateDirectory("../../DataServer").Create();

                            if (!File.Exists("../../DataServer/setting.txt"))
                            {
                                File.Create("../../DataServer/setting.txt").Close();

                                if (!string.IsNullOrEmpty(Service_UCs.IPaddressTextBox.Text) && !string.IsNullOrEmpty(Service_UCs.PortNumerTextBox.Text))
                                {
                                    FileHelper.FileStreamWrite("../../DataServer/setting.txt", Service_UCs.IPaddressTextBox.Text + " \n" + Service_UCs.PortNumerTextBox.Text);

                                    string[] words = FileHelper.FileStreamRead("../../DataServer/setting.txt").Split(' ');

                                }

                            }
                        }

                        else
                        {
                            File.Delete("../../DataServer/setting.txt");

                            DirectoryInfo di = new DirectoryInfo("../../DataServer");

                            foreach (FileInfo file in di.GetFiles())
                            {
                                file.Delete();
                            }
                            foreach (DirectoryInfo dir in di.GetDirectories())
                            {
                                dir.Delete(true);
                            }

                            Directory.Delete("../../DataServer");

                            if (!Directory.Exists("../../DataServer"))
                            {
                                Directory.CreateDirectory("../../DataServer").Create();

                                if (!File.Exists("../../DataServer/setting.txt"))
                                {
                                    File.Create("../../DataServer/setting.txt").Close();

                                    if (!string.IsNullOrEmpty(Service_UCs.IPaddressTextBox.Text) && !string.IsNullOrEmpty(Service_UCs.PortNumerTextBox.Text))
                                    {
                                        FileHelper.FileStreamWrite("../../DataServer/setting.txt", Service_UCs.IPaddressTextBox.Text + " \n" + Service_UCs.PortNumerTextBox.Text);

                                        FileHelper.FileStreamRead("../../DataServer/setting.txt");

                                        string[] words = FileHelper.FileStreamRead("../../DataServer/setting.txt").Split(' ');

                                    }

                                }
                            }
                        }


                    }));


                });

            });


            SavePermissionCommand = new RelayCommand((sender) =>
            {



            });


        }
    }
}
