using System;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace AssetsParserCSharp
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            const SslProtocols tls12Protocol = (SslProtocols)0x00000C00;
            const SecurityProtocolType tls12 = (SecurityProtocolType)tls12Protocol;
            ServicePointManager.SecurityProtocol = tls12;

            Console.WriteLine("Select a directory for workPath");
            Console.WriteLine("This directory used to download the objects");
            var workDirectory = "";
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    workDirectory = fbd.SelectedPath;
                }
            }
            
            
            var jsonDir = workDirectory + @"\jsons";
            var file18 = jsonDir + @"\1.8.json";
            var file188 = jsonDir + @"\1.8.8.json";
            var file112 = jsonDir + @"\1.12.json"; 
            var file115 = jsonDir + @"\1.15.json";
            var file116 = jsonDir + @"\1.16.json";
            
            const string url112 = "https://raw.githubusercontent.com/VechoDEV/MinecraftJSONAssets/main/1.12.json";
            const string url18 = "https://raw.githubusercontent.com/VechoDEV/MinecraftJSONAssets/main/1.8.json";
            const string url188 = "https://raw.githubusercontent.com/VechoDEV/MinecraftJSONAssets/main/1.8.8.json";
            const string url115 = "https://raw.githubusercontent.com/VechoDEV/MinecraftJSONAssets/main/1.15.json";
            const string url116 = "https://raw.githubusercontent.com/VechoDEV/MinecraftJSONAssets/main/1.16.json";
            
            if (!File.Exists(jsonDir)) Directory.CreateDirectory(jsonDir);
            
            if (!File.Exists(file18))
            {
                var client = new WebClient {Encoding = Encoding.UTF8};
                var jsonStr = client.DownloadString(url18);
                File.WriteAllText(file18, jsonStr);
            }
            
            if (!File.Exists(file188))
            {
                var client = new WebClient {Encoding = Encoding.UTF8};
                var jsonStr = client.DownloadString(url188);
                File.WriteAllText(file188, jsonStr);
            }
            if (!File.Exists(file112))
            {
                var client = new WebClient {Encoding = Encoding.UTF8};
                var jsonStr = client.DownloadString(url112);
                File.WriteAllText(file112, jsonStr);
            }
            if (!File.Exists(file115))
            {
                var client = new WebClient {Encoding = Encoding.UTF8};
                var jsonStr = client.DownloadString(url115);
                File.WriteAllText(file115, jsonStr);
            }
            if (!File.Exists(file116))
            {
                var client = new WebClient {Encoding = Encoding.UTF8};
                var jsonStr = client.DownloadString(url116);
                File.WriteAllText(file116, jsonStr);
            }

            var objects = workDirectory + @"\objects";
            Console.WriteLine("Checking/Downloading assets.");
            Download(file18, objects);
            Download(file188, objects);
            Download(file112, objects);
            Download(file115, objects);
            Download(file116, objects);
            Console.WriteLine("Assets checked/downloaded.");

            Console.WriteLine("Doing mappings 1.8...");
            var mapFile = workDirectory + @"\map_1.8.txt";
            Map(file18, mapFile);
            Console.WriteLine("Doing mappings 1.8.8...");
            var mapFile1 = workDirectory + @"\map_1.8.8.txt";
            Map(file188, mapFile1);
            Console.WriteLine("Doing mappings 1.12...");
            var mapFile2 = workDirectory + @"\map_1.12.txt";
            Map(file112, mapFile2);
            Console.WriteLine("Doing mappings 1.15...");
            var mapFile3 = workDirectory + @"\map_1.15.txt";
            Map(file115, mapFile3);
            Console.WriteLine("Doing mappings 1.16...");
            var mapFile4 = workDirectory + @"\map_1.16.txt";
            Map(file116, mapFile4);
            Console.WriteLine("For close press any key...");
            Console.ReadKey();
        }

        private static void Download(string json, string objects)
        {
            var obj = JObject.Parse(File.ReadAllText(json));
            var objectsJson = JObject.FromObject(obj["objects"] ?? "");
            foreach (var asset in objectsJson)
            {
                var hash = (string)asset.Value["hash"];
                if (hash != null)
                {
                    var downloadLink = "http://resources.download.minecraft.net/" + hash.Substring(0, 2) + "/" + hash;
                    var locationCopyFolder = objects + @"\" + hash.Substring(0, 2);
                    var locationCopy = locationCopyFolder + @"\" + hash;
                    if (!File.Exists(locationCopyFolder)) Directory.CreateDirectory(locationCopyFolder);
                    if (!File.Exists(locationCopy) || new FileInfo(locationCopy).Length != (long) asset.Value["size"])
                    {
                        Console.WriteLine("Downloading: "+asset.Key + " --- "+new FileInfo(json).Name);
                        var client = new WebClient {Encoding = Encoding.UTF8};
                        File.WriteAllBytes(locationCopy,client.DownloadData(downloadLink));
                    }
                } 
            }
        }

        private static void Map(string json, string mapFile)
        {
            var obj = JObject.Parse(File.ReadAllText(json));
            var objectsJson = JObject.FromObject(obj["objects"] ?? "");
            if (!File.Exists(mapFile))
            {
                var stream = new FileStream(mapFile, FileMode.OpenOrCreate);
                stream.Flush();
                stream.Close();
            }
            var toWrite = new string[] {};
            foreach (var asset in objectsJson)
            {
                var hash = (string)asset.Value["hash"];
                if (hash != null)
                {
                    var folder = hash.Substring(0, 2);
                    Array.Resize(ref toWrite, toWrite.Length + 1);
                    toWrite[toWrite.GetUpperBound(0)] = asset.Key + " : " + folder + "/" + hash;
                } 
            }
            
            File.WriteAllLines(mapFile, toWrite);
        }
    }
}