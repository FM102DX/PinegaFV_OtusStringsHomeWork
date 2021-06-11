using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace OtusStrings
{
    class Program
    {
        
        // 11.06.2021, Федор Пинега, ДЗ по теме Строки и регулярные выражения
        static void Main(string[] args)
        {
            Console.WriteLine("Starting image getting app");
            ImageGettingMachine machine = ImageGettingMachine.GetInstance(@"https://fonwall.ru", @"C:\temp\imgsrc\");
            machine.Run();
        }
    }

    public class ImageGettingMachine
    {
        string url;
        string storeDir;

        private ImageGettingMachine()
        {

        }
        public static ImageGettingMachine GetInstance(string _url, string _storeDir)
        {
            return new ImageGettingMachine
            {
                url = _url,
                storeDir = _storeDir
            };
        }
        public void Run()
        {
            List<string> rez = new List<string>();
            string html = "";

            //получаем html 
            html = GetCode(url);
            Regex regex = new Regex("< *img[^>]*src *= *\"([^\"]*(\\.jpeg|\\.jpg|\\.bmp|\\.gif|\\.tiff|\\.png)){1}[^>]*>", RegexOptions.IgnoreCase);
            foreach (Match match in regex.Matches(html))
            {
                rez.Add(match.Groups[1].Value);
            }
            rez.ForEach(x=> { saveResource(x, storeDir);});
            Console.WriteLine(rez.Count.ToString());

            //сохраняем картинки
        }

        private String GetCode(string urlAddress)
        {
            //честно слито со stackoverflow
            //string urlAddress = "http://google.com";
            string data = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }
                data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }
            return data;
        }

        private void saveResource(string resourceAddress, string filePath)
        {
            Console.Write($"Saving {resourceAddress} into {filePath}") ;
            try
            {
                //1 ) проверяем сущестоввание пути
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                // 2) получаем ресурс и качаем файл
                WebClient client = new WebClient();
                Uri uri = new Uri(resourceAddress);
                string targetFileName = getfileNameFromFilePath(resourceAddress);
                client.DownloadFile(uri, filePath + targetFileName);
                Console.Write("...ok");
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine($"Error {e.Message}");;
                Console.WriteLine("");
            }


        }

        private string getfileNameFromFilePath (string source)
        {
            if (source.Length < 10) return "";
            int i = source.LastIndexOf ('/');
            int targrtLength = source.Length - 1 - i;
            if (targrtLength<3) return "";
            return source.Substring(i + 1, targrtLength);
        }
    }



}
