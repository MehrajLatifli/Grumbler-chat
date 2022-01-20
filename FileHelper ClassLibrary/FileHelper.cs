using Newtonsoft.Json;
using Socket_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace FileHelper_ClassLibrary
{
    public static class FileHelper
    {

        public static void ReceiveFileServer(Socket socket, string filename)
        {

            try
            {
                using (var networkStream = new NetworkStream(socket))
                {
                    var header = new byte[4];
                    var bytesLeft = 4;
                    var offset = 0;

                    while (bytesLeft > 0)
                    {
                        var bytesRead = networkStream.Read(header, offset, bytesLeft);
                        offset += bytesRead;
                        bytesLeft -= bytesRead;
                    }

                    bytesLeft = BitConverter.ToInt32(header, 0);
                    offset = 0;
                    var fileContents = new byte[bytesLeft];

                    while (bytesLeft > 0)
                    {
                        var bytesRead = networkStream.Read(fileContents, offset, bytesLeft);
                        offset += bytesRead;
                        bytesLeft -= bytesRead;
                    }





                    //File.WriteAllBytes($"{Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"))}", fileContents);

                    File.WriteAllBytes($"{Path.GetPathRoot(Environment.SystemDirectory)}Users\\{Environment.UserName}\\{Environment.SpecialFolder.Desktop}\\{Path.GetFileName(filename)}", fileContents);


                    //File.WriteAllBytes($"{Path.GetPathRoot(Environment.SystemDirectory)}Users\\{Environment.UserName}\\{Environment.SpecialFolder.Desktop}\\{guid1}{Path.GetExtension(filename)}", fileContents);

                    //   File.WriteAllBytes($"../../DataServer/{filename} {Path.GetExtension(filename)}", fileContents);



                }


            }
            catch (Exception)
            {

            }
        }

        public static void ReceiveFileClient(Socket socket, string filename)
        {

            try
            {
                using (var networkStream = new NetworkStream(socket))
                {
                    var header = new byte[4];
                    var bytesLeft = 4;
                    var offset = 0;

                    while (bytesLeft > 0)
                    {
                        var bytesRead = networkStream.Read(header, offset, bytesLeft);
                        offset += bytesRead;
                        bytesLeft -= bytesRead;
                    }

                    bytesLeft = BitConverter.ToInt32(header, 0);
                    offset = 0;
                    var fileContents = new byte[bytesLeft];

                    while (bytesLeft > 0)
                    {
                        var bytesRead = networkStream.Read(fileContents, offset, bytesLeft);
                        offset += bytesRead;
                        bytesLeft -= bytesRead;
                    }



                    File.WriteAllBytes($"{Path.GetPathRoot(Environment.SystemDirectory)}Users\\{Environment.UserName}\\{Environment.SpecialFolder.Desktop}\\{Path.GetFileName(filename)}", fileContents);




                }


            }
            catch (Exception)
            {

            }
        }

        public static void SendFile(Socket socket, string filename)
        {
            try
            {
                using (var networkStream = new NetworkStream(socket))
                {
                    var bytesToSend = File.ReadAllBytes(filename);
                    var header = BitConverter.GetBytes(bytesToSend.Length);
                    networkStream.Write(header, 0, header.Length);
                    networkStream.Write(bytesToSend, 0, bytesToSend.Length);



                }
            }
            catch (Exception)
            {

            }

        }

        public static void SendFile2(Socket socket, string filename, Guid guid)
        {
            try
            {
                using (var networkStream = new NetworkStream(socket))
                {
                    var bytesToSend = File.ReadAllBytes(filename);
                    var header = BitConverter.GetBytes(bytesToSend.Length);
                    networkStream.Write(header, 0, header.Length);
                    networkStream.Write(bytesToSend, 0, bytesToSend.Length);


                    File.WriteAllBytes($"../../Data/{guid}.{Path.GetExtension(filename)}", bytesToSend);
                }
            }
            catch (Exception)
            {

            }

        }

        public static void Uploadfile(string filePath)
        {
            string path = Path.GetFileName(filePath);
            MessageBox.Show(path);

            string password = Console.ReadLine();


            try
            {



                FileInfo fileInfo = new FileInfo(filePath);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://ftpupload.net/htdocs/{path}");
                request.Credentials = new NetworkCredential("epiz_30770440", "qQs9vfxpbjcExl9");
                request.Method = WebRequestMethods.Ftp.UploadFile;


                using (Stream fileStream = File.OpenRead($"{filePath}"))
                using (Stream ftpStream = request.GetRequestStream())
                {
                    byte[] buffer = new byte[102400000];
                    int read;
                    while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ftpStream.Write(buffer, 0, read);
                        Console.WriteLine(" \n Uploaded {0} bytes", fileStream.Position);




                    }
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        public static void Downloadfile(string filePath)
        {

            try
            {


                string path = Path.GetFileName(filePath);

                string path2 = Path.GetFileNameWithoutExtension(filePath);

                FileInfo fileInfo = new FileInfo(filePath);

                string ex = fileInfo.Extension;


                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://ftpupload.net/htdocs/{path}");
                request.Credentials = new NetworkCredential("epiz_30770440", "qQs9vfxpbjcExl9");
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                request.Timeout = 500;
                request.UsePassive = true;

                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                using (Stream fileStream = File.Create(filePath))
                {
                    byte[] buffer = new byte[102400000];
                    int read;
                    while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, read);
                        Console.WriteLine("Downloaded {0} bytes", fileStream.Position);

                        // File.WriteAllBytes($"../../Data/{Path.GetFileName(filePath)}", buffer);
                    }


                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }


        }


        public static object BinaryDeSerialize_Socket()
        {


            if (!System.IO.File.Exists("../../../Files/Socket.bin"))
            {
                MessageBox.Show($"Bin error");


            }


            BinaryFormatter formatter = new BinaryFormatter();

            using (Stream fStream = File.OpenRead("../../../Files/Socket.bin"))
            {
                return formatter.Deserialize(fStream);
            }

        }

        public static void BinarySerialize_Socket(List<SocketClass> files)
        {
            if (!Directory.Exists("../../../Files/"))
            {
                Directory.CreateDirectory("../../../Files");
            }

            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                using (Stream fStream = new FileStream("../../../Files/Socket.bin", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(fStream, files);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show($"{ex.Message}");
            }


        }




        public static string EncryptString(string plainText)
        {
            string key2 = @"myKey123#%myKey123";

            int dataLen = plainText.Length;
            int keyLen = key2.Length;
            char[] output = new char[dataLen];

            for (int i = 0; i < dataLen; ++i)
            {
                output[i] = (char)(plainText[i] ^ key2[i % keyLen]);
            }

            return new string(output);

        }

        public static string DecryptString(string cipherText)
        {
            string Text = EncryptString(cipherText);

            string Text2 = EncryptString(Text);

            return Text2;
        }

        public static void EncryptFile(string inputFile, string outputFile)
        {
            try
            {


                string password = @"myKey123#%myKey123"; // Your Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();




            }
            catch (Exception)
            {


            }



        }


        public static void DecryptFile(string inputFile, string outputFile)
        {
            try
            {





                string password = @"myKey123#%myKey123"; // Your Key Here

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();


            }
            catch (Exception)
            {


            }

        }


        public static void FileStreamWrite(string filename, string text)
        {

            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                byte[] bytes = Encoding.Default.GetBytes(text);
                fs.Write(bytes, 0, bytes.Length);

            }



        }

        public static string FileStreamRead(string filename)
        {

            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
            {
                byte[] bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                string text = Encoding.Default.GetString(bytes);

                return text;
            }

        }

    }
}
