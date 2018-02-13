using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PointVideoGallery;
using PointVideoGallery.Models;
using PointVideoGallery.Services;
using WinSCP;

namespace ScheduleTaskExecuter
{
    class Program
    {
        private static readonly string _leadingRelativePath =
            ConfigurationManager.AppSettings["XmlAssetsLeadingRelativePath"];

        // where media assets store locally
        private static readonly string LocalAssetsDirectory = ConfigurationManager.AppSettings["LibraryIndexBasePath"];

        // where media assets will be uploaded remotely
        private static string RemoteAssetsDirectory { get; } =
            ConfigurationManager.AppSettings["RemoteAssetsDirectory"];

        // the local directory where xml file generated locally
        private static readonly string XmlOutputDirectory = ConfigurationManager.AppSettings["XmlOutputPath"];

        // the full path of generated xml file stored locally 
        private static readonly string XmlOutputFilePath = Path.Combine(XmlOutputDirectory, "data.xml");

        private static string RemoteRootDirectory { get; } = ConfigurationManager.AppSettings["RemoteRootDirectory"];
        private static string CombineDirectory { get; } = Path.Combine(XmlOutputDirectory, "combine");
        private static string CombineDirectoryFilePath { get; } = Path.Combine(CombineDirectory, "data.xml");


        private static string HostName { get; } = ConfigurationManager.AppSettings["HostName"];
        private static string UserName { get; } = ConfigurationManager.AppSettings["UserName"];
        private static string Password { get; } = ConfigurationManager.AppSettings["Password"];

        private static ScheduleService _service = new ScheduleService();

        // local combination directory
        private static SessionOptions _options = new SessionOptions
        {
            Protocol = Protocol.Ftp,
            HostName = HostName,
            UserName = UserName,
            Password = Password,
        };

        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                int id;
                if (Int32.TryParse(args[0], out id))
                    GenerateXml(id);
                else
                    Console.WriteLine("INVALID INPUT");
                return;
            }
            var events = _service.GetSchedulesByDateAsync(DateTime.Today).Result;
            foreach (var scheduleEvent in events)
            {
                List<string> paths = new List<string>();
                List<string> soList = new List<string>();
                var adEvent = GenerateXml(scheduleEvent.EventId);
                adEvent.Resources.ForEach(s =>
                {
                    paths.Add(s.Path.Replace("/", @"\"));
                    paths.AddRange(from obj in s.Actions
                        where obj.Type.Equals("image", StringComparison.OrdinalIgnoreCase) &&
                              !string.IsNullOrWhiteSpace(obj.Action)
                        select obj.Action.Replace("/", @"\"));
                });
                adEvent.SoSettings.ForEach(s => soList.Add(s.Code));
                UploadFiles(paths, soList).Wait();
            }
            Console.ReadKey();
        }


        public static AdEvent GenerateXml(int id)
        {
            AdService service = new AdService();
            var adEvent = service.GetAdEventByIdAsync(id).Result;
            return Generate(adEvent, service);
        }

        /// <summary>
        /// Query database and generate associate xml file
        /// </summary>
        /// <param name="adEvent"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        private static AdEvent Generate(AdEvent adEvent, AdService service)
        {
            var setting = new XmlWriterSettings {Async = false, Encoding = Encoding.UTF8, Indent = true};
            var startDate = DateTime.Today;

            using (var writer = XmlWriter.Create(XmlOutputFilePath, setting))
            {
                // <?xml version="1.0" encoding="utf-8"?>
                writer.WriteStartDocument();
                // <root xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
                writer.WriteStartElement("root");
                writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");

                // <entry time="2016/09/19 00:00:00">
                writer.WriteStartElement("entry");
                writer.WriteAttributeString("time", startDate.ToString("yyyy/MM/dd HH:mm:ss"));

                if (adEvent != null && adEvent.LocationTags != null)
                    foreach (LocationTag location in adEvent.LocationTags)
                    {
                        // <block name="miniepg">
                        writer.WriteStartElement("block");
                        writer.WriteAttributeString("name", location.Name);

                        // <children>
                        writer.WriteStartElement("children");

                        // <child name="Production - miniEPG">
                        writer.WriteStartElement("child");
                        writer.WriteAttributeString("name", adEvent.Name);

                        // <playMode type="interval" subtype="bytime" value="10" />
                        writer.WriteStartElement("playMode");
                        writer.WriteAttributeString("type", adEvent.PlayOutMethod);
                        writer.WriteAttributeString("subtype", adEvent.PlayOutSequence);
                        // only display in interval mode
                        if (adEvent.PlayOutMethod.Equals("interval", StringComparison.OrdinalIgnoreCase))
                            writer.WriteAttributeString("value", adEvent.PlayOutTimeSpan.ToString());

                        // <assets>
                        writer.WriteStartElement("assets");

                        for (var i = 0; i < adEvent.Resources.Count; i++)
                        {
                            var resource = adEvent.Resources[i];
                            var path = _leadingRelativePath + resource.Path.Split('/')[1];
                            var thumbnail = _leadingRelativePath + resource.ThumbnailPath.Split('/')[1];

                            // <asset value="./../asset/200M_544x120_201709.jpg" type="image">
                            writer.WriteStartElement("asset");
                            writer.WriteAttributeString("type", resource.MediaType);
                            writer.WriteAttributeString("value", thumbnail);
                            // only display if random
                            if (adEvent.PlayOutMethod.Equals("random", StringComparison.OrdinalIgnoreCase))
                                writer.WriteAttributeString("weights", resource.PlayoutWeight.ToString());

                            var actions = service.GetActionsAsync(adEvent.Id, resource.Sequence).Result;
                            adEvent.Resources[i].Actions = actions;

                            foreach (var action in actions)
                            {
                                if (action.Checked == 0)
                                    continue;
                                // <action value="./../asset/200M_1280x720_201709.jpg" type="image" parameter="none" code="blue"/> 
                                writer.WriteStartElement("action");
                                writer.WriteAttributeString("value", path);
                                writer.WriteAttributeString("type", action.Type);
                                writer.WriteAttributeString("parameter", action.Parameter);
                                writer.WriteAttributeString("code", action.Color);

                                // action end
                                writer.WriteEndElement();
                            }

                            // asset end
                            writer.WriteEndElement();
                        }

                        // assets end
                        writer.WriteEndElement();
                        // duration end
                        //                     writer.WriteEndElement();
                        // playMode end
                        writer.WriteEndElement();
                        // child end
                        writer.WriteEndElement();
                        // children end
                        writer.WriteEndElement();
                        // block end
                        writer.WriteEndElement();
                    }
                // entry end
                writer.WriteEndElement();
                // root end
                writer.WriteEndElement();
                // document end
                writer.WriteEndDocument();

                writer.Flush();
            }
            return adEvent;
        }

        /// <summary>
        /// UPLOAD all files to remote FTP server
        /// </summary>
        /// <param name="paths">media assets path</param>
        /// <param name="soList">so number, for example: 02</param>
        /// <returns></returns>
        public static async Task UploadFiles(List<string> paths, List<string> soList)
        {
            var errMsg = new StringBuilder();

            if (!Directory.Exists(XmlOutputDirectory))
                Directory.CreateDirectory(XmlOutputDirectory);
            if (!Directory.Exists(CombineDirectory))
                Directory.CreateDirectory(CombineDirectory);

            using (Session session = new Session())
            {
                var results = new List<TransferOperationResult>();
                var options = new TransferOptions
                {
                    TransferMode = TransferMode.Binary,
                    OverwriteMode = OverwriteMode.Overwrite
                };

                try
                {
                    session.Open(_options);

                    Console.WriteLine("---------- Upload ----------");

                    soList.ForEach(async s =>
                    {
                        // remote directory 
                        var remotePath = RemoteRootDirectory + "SO" + s + @"/";
                        // remotr file path to be merged
                        var remoteFilePath = remotePath + "data.xml";
                        // the local path that remote data.xml will be download and save at
                        var remoteXmlLocalPath = Path.Combine(XmlOutputDirectory, "remote");
                        // the full path after saving the remote file in local hard drive
                        var remoteXmlLocalFilePath = Path.Combine(remoteXmlLocalPath, "data.xml");
                        // the portal block xml that to be combined
                        var portalBlock = new StringBuilder();

                        try
                        {
                            // check if remote data.xml is existed, if yes, combine the remote and the local
                            session.GetFileInfo(remoteFilePath);

                            // download file from the remote directory
                            session.GetFiles(remotePath, remoteXmlLocalPath, false, options).Check();

                            // check if remote file is correctly downloaded
                            if (!File.Exists(remoteXmlLocalFilePath))
                            {
                                errMsg.AppendLine($"File not found {remoteXmlLocalFilePath}");
                                throw new FileNotFoundException($"File not found {remoteXmlLocalFilePath}");
                            }

                            // read download file and extract the portal block
                            using (var reader = new StreamReader(remoteXmlLocalFilePath))
                            {
                                bool isPortal = false;
                                while (!reader.EndOfStream)
                                {
                                    var line = await reader.ReadLineAsync();
                                    if (!isPortal && !line.Contains("<block name=\"portal\">"))
                                        continue;
                                    isPortal = true;

                                    portalBlock.Append(line);

                                    var index = -1;
                                    if ((index = line.IndexOf("</block>", StringComparison.Ordinal)) == -1)
                                        continue;

                                    if (index + 8 < line.Length)
                                        portalBlock.Remove(portalBlock.Length - line.Length + index + 8,
                                            line.Length - index);
                                    break;
                                }
                            }

                            if (!File.Exists(CombineDirectoryFilePath))
                                File.Create(CombineDirectoryFilePath);

                            // combine remote xml and local xml
                            using (var writer = new StreamWriter(CombineDirectoryFilePath, false))
                            {
                                using (var reader = new StreamReader(XmlOutputFilePath))
                                {
                                    while (!reader.EndOfStream)
                                    {
                                        var line = await reader.ReadLineAsync();

                                        var index = -1;
                                        if ((index = line.IndexOf("</block>", StringComparison.Ordinal)) == -1)
                                        {
                                            await writer.WriteLineAsync(line);
                                            continue;
                                        }
                                        await writer.WriteLineAsync(line.Substring(index, 8));
                                        await writer.WriteLineAsync(portalBlock.ToString());
                                        if (line.Length > index + 8)
                                            await writer.WriteLineAsync(line.Substring(8, line.Length - 8));
                                        await writer.WriteLineAsync(await reader.ReadToEndAsync());
                                        await writer.FlushAsync();
                                        break;
                                    }
                                }
                            }
                            // upload combine data.xml
                            results.Add(session.PutFiles(CombineDirectoryFilePath, remotePath, false, options));
                        }
                        catch (SessionRemoteException)
                        {
                            // remote data.xml does not existed, directly push local generated xml file
                            results.Add(session.PutFiles(XmlOutputFilePath, remotePath, false, options));
                        }
                        Console.WriteLine(remotePath);
                    });

                    // upload media assets
                    paths.ForEach(s =>
                    {
                        var path = Path.Combine(LocalAssetsDirectory, s);
                        if (!File.Exists(path))
                        {
                            errMsg.AppendLine($"File not found {path}");
                        }
                        else
                        {
                            results.Add(session.PutFiles(path, RemoteAssetsDirectory, false, options));
                            Console.WriteLine(s);
                        }
                    });

                    results.ForEach(s => s.Check());

                    if (errMsg.Length != 0)
                        throw new FileNotFoundException();

                    await LogService.WriteLogAsync(new Log
                    {
                        Action = "排程FTP上傳成功",
                        ActionTime = DateTime.Now,
                        UserId = 0 // SYSTEM
                    });
                    Console.WriteLine("----- SUCCESS -----");
                }
                catch (Exception e)
                {
                    await LogService.WriteLogAsync(new Log
                    {
                        Action = $"排程FTP上傳失敗 {errMsg}",
                        ActionTime = DateTime.Now,
                        UserId = 0 // SYSTEM
                    });
                    Console.WriteLine(errMsg);
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}