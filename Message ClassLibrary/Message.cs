using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Message_ClassLibrary
{
    [Serializable]
    public class Message : INotifyPropertyChanged
    {
        public Message() { }
        public Message(string fileName, string fileSize, string fileAddDateTime)
        {
            FileName = fileName;
            FolderofFile = fileSize;
            FileAddDateTime = fileAddDateTime;
        }

        public Message(string fileName, string folderofFile, string fileAddDateTime, string filePath, string fileİmagePath, string fileShortName)
        {
            FileInfo FileInfo_1 = new FileInfo(filePath);
            fileShortName = FileInfo_1.Name;

            FileName = fileName;
            FolderofFile = folderofFile;
            FileAddDateTime = fileAddDateTime;
            FilePath = filePath;
            FileİmagePath = fileİmagePath;
            FileShortName = fileShortName;
        }

        public Message(string fileName, string fileShortName, string folderofFile, string fileAddDateTime, string filePath, string fileİmagePath, string textMessage)
        {
            FileName = fileName;
            FileShortName = fileShortName;
            FolderofFile = folderofFile;
            FileAddDateTime = fileAddDateTime;
            FilePath = filePath;
            FileİmagePath = fileİmagePath;
            TextMessage = textMessage;
        }

        public Message(string textMessage)
        {
            TextMessage = textMessage;
        }

        string fileName; string folderofFile; string fileAddDateTime; string filePath; string fileİmagePath; string fileShortName; string textMessage;
        public string FileName { get { return fileName; } set { fileName = value; OnPropertyChanged(); } }

        public string FileShortName { get { return fileShortName; } set { fileShortName = value; OnPropertyChanged(); } }
        public string FolderofFile { get { return folderofFile; } set { folderofFile = value; OnPropertyChanged(); } }
        public string FileAddDateTime { get { return fileAddDateTime; } set { fileAddDateTime = value; OnPropertyChanged(); } }
        public string FilePath { get { return filePath; } set { filePath = value; OnPropertyChanged(); } }

        public string FileİmagePath { get { return fileİmagePath; } set { fileİmagePath = value; OnPropertyChanged(); } }

        public string TextMessage { get { return textMessage; } set { textMessage = value; OnPropertyChanged(); } }


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
    }
}
