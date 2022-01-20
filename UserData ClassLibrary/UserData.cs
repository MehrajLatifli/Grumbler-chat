using Message_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UserData_ClassLibrary
{
    [Serializable()]
    public class UserData : INotifyPropertyChanged
    {
        public UserData()
        {
          //  Usermessagelist = new ObservableCollection<Message>();
        }

        public UserData(string username, string name, string surname, string password, string confirmPassword, string email, string profileimage)
        {
            Username = username;
            Name = name;
            Surname = surname;
            Password = password;
            ConfirmPassword = confirmPassword;
            Email = email;
            Profileimage = profileimage;
        }

        public UserData(string username, string name, string surname, string password, string confirmPassword, string email, string profileimage, bool sendimage, bool sendfile, bool recordaudiofile, bool blockusers, bool removemessage)
        {
            Username = username;
            Name = name;
            Surname = surname;
            Password = password;
            ConfirmPassword = confirmPassword;
            Email = email;
            Profileimage = profileimage;
            Sendimage = sendimage;
            Sendfile = sendfile;
            Recordaudiofile = recordaudiofile;
            Blockusers = blockusers;
            Removemessage = removemessage;
        }

        string username { get; set; }
        string name { get; set; }
        string surname { get; set; }
        string password { get; set; }
        string confirmPassword { get; set; }
        string email { get; set; }
        string profileimage { get; set; }
        bool sendimage { get; set; }
        bool sendfile { get; set; }
        bool recordaudiofile { get; set; }
        bool blockusers { get; set; }
        bool removemessage { get; set; }

        public ObservableCollection<Message> usermessagelist { get; set; }

        public string Username { get { return username; } set { username = value; OnPropertyChanged(); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(); } }
        public string Surname { get { return surname; } set { surname = value; OnPropertyChanged(); } }
        public string Password { get { return password; } set { password = value; OnPropertyChanged(); } }
        public string ConfirmPassword { get { return confirmPassword; } set { confirmPassword = value; OnPropertyChanged(); } }
        public string Email { get { return email; } set { email = value; OnPropertyChanged(); } }
        public string Profileimage { get { return profileimage; } set { profileimage = value; OnPropertyChanged(); } }


        public bool Sendimage { get { return sendimage; } set { sendimage = value; OnPropertyChanged(); } }
        public bool Sendfile { get { return sendfile; } set { sendfile = value; OnPropertyChanged(); } }
        public bool Recordaudiofile { get { return recordaudiofile; } set { recordaudiofile = value; OnPropertyChanged(); } }
        public bool Blockusers { get { return blockusers; } set { blockusers = value; OnPropertyChanged(); } }
        public bool Removemessage { get { return removemessage; } set { removemessage = value; OnPropertyChanged(); } }

        public ObservableCollection<Message> Usermessagelist { get { return usermessagelist; } set { usermessagelist = value; OnPropertyChanged(); } }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {

                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void add1(string textMessage)
        {
            usermessagelist.Add(
             new Message
             {
                    TextMessage=textMessage,
             });
        }

        public void add2(string fileName, string fileShortName, string folderofFile, string fileAddDateTime, string filePath, string fileİmagePath, string textMessage)
        {
            usermessagelist.Add(
             new Message
             {
                 FileName = fileName,
                 FileShortName = fileShortName,
                 FolderofFile = folderofFile,
                 FileAddDateTime = fileAddDateTime,
                 FilePath = filePath,
                 FileİmagePath = fileİmagePath,
                 TextMessage = textMessage
             });
        }

        public void add3(string fileName, string fileShortName, string folderofFile, string fileAddDateTime, string filePath, string fileİmagePath)
        {
            usermessagelist.Add(
             new Message
             {
                 FileName = fileName,
                 FileShortName = fileShortName,
                 FolderofFile = folderofFile,
                 FileAddDateTime = fileAddDateTime,
                 FilePath = filePath,
                 FileİmagePath = fileİmagePath
             });
        }
    }
}
