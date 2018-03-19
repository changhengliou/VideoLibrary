using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using PointVideoGallery.Models;
using WebGrease.Css.Extensions;

namespace PointVideoGallery.Services
{
    public class XmlGenService
    {
        private static string _leadingRelativePath = ConfigurationManager.AppSettings["XmlAssetsLeadingRelativePath"];

        // where media assets store locally
        private static readonly string LocalAssetsDirectory = ConfigurationManager.AppSettings["LibraryIndexBasePath"];

        // the local directory where xml file generated locally
        private static readonly string XmlOutputDirectory = ConfigurationManager.AppSettings["XmlOutputPath"];

        // where pre-download xml to be store
        private static readonly string DownloadPackageDirectory = Path.Combine(XmlOutputDirectory, "package");

        // where pre-download image to be store
        private static readonly string DownloadPackageImageDirectory = Path.Combine(DownloadPackageDirectory, "asset");

        // temp xml file name
        private static readonly string TempXmlFileName = "temp.xml";

        // temp xml path
        private static readonly string DownloadPackageTempXmlPath =
            Path.Combine(DownloadPackageDirectory, TempXmlFileName);

        // zip output directory
        private static readonly string ZipPackageOutPath = Path.Combine(XmlOutputDirectory, "package.zip");

        public static async Task<AdEvent> GenerateAsync(int id, MemoryStream stream)
        {
            AdService service = new AdService();
            var adEvent = await service.GetAdEventByIdAsync(id);

            var setting = new XmlWriterSettings {Async = true, Encoding = Encoding.UTF8, Indent = true};
            var writer = XmlWriter.Create(stream, setting);

            await GenerateXmlAsync(writer, adEvent);

            writer.Dispose();

            return adEvent;
        }

        private static async Task<AdEvent> GenerateAsync(AdEvent adEvent, string outputPath)
        {
            var setting = new XmlWriterSettings {Async = true, Encoding = Encoding.UTF8, Indent = true};
            var writer = XmlWriter.Create(outputPath, setting);

            await GenerateXmlAsync(writer, adEvent);

            writer.Dispose();
            return adEvent;
        }

        private static async Task<AdEvent> GenerateXmlAsync(XmlWriter writer, AdEvent adEvent)
        {
            var service = new AdService();
            var startDate = DateTime.Today;

            // <?xml version="1.0" encoding="utf-8"?>
            await writer.WriteStartDocumentAsync();
            // <root xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
            await writer.WriteStartElementAsync(null, "root", null);
            await writer.WriteAttributeStringAsync("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
            await writer.WriteAttributeStringAsync("xmlns", "xsi", null,
                "http://www.w3.org/2001/XMLSchema-instance");

            // <entry time="2016/09/19 00:00:00">
            await writer.WriteStartElementAsync(null, "entry", null);
            await writer.WriteAttributeStringAsync(null, "time", null, startDate.ToString("yyyy/MM/dd HH:mm:ss"));

            if (adEvent?.LocationTags != null)
                foreach (LocationTag location in adEvent.LocationTags)
                {
                    // <block name="miniepg">
                    await writer.WriteStartElementAsync(null, "block", null);
                    await writer.WriteAttributeStringAsync(null, "name", null, location.Name);

                    // <children>
                    await writer.WriteStartElementAsync(null, "children", null);

                    // <child name="Production - miniEPG">
                    await writer.WriteStartElementAsync(null, "child", null);
                    await writer.WriteAttributeStringAsync(null, "name", null, adEvent.Name);

                    // <playMode type="interval" subtype="bytime" value="10" />
                    await writer.WriteStartElementAsync(null, "playMode", null);
                    await writer.WriteAttributeStringAsync(null, "type", null, adEvent.PlayOutMethod);
                    await writer.WriteAttributeStringAsync(null, "subtype", null, adEvent.PlayOutSequence);
                    // only display in interval mode
                    if (adEvent.PlayOutMethod.Equals("interval", StringComparison.OrdinalIgnoreCase))
                        await writer.WriteAttributeStringAsync(null, "value", null,
                            adEvent.PlayOutTimeSpan.ToString());

                    // <duration value="600" />
                    await writer.WriteStartElementAsync(null, "duration", null);
                    await writer.WriteAttributeStringAsync(null, "value", null, adEvent.EventTimeSpan.ToString());

                    // <assets>
                    await writer.WriteStartElementAsync(null, "assets", null);

                    foreach (ResourceEvent resource in adEvent.Resources)
                    {
                        var path = _leadingRelativePath + resource.Path.Split('/')[1];
                        var thumbnail = _leadingRelativePath + resource.ThumbnailPath.Split('/')[1];

                        // <asset value="./../asset/200M_544x120_201709.jpg" type="image">
                        await writer.WriteStartElementAsync(null, "asset", null);
                        await writer.WriteAttributeStringAsync(null, "type", null, resource.MediaType);
                        await writer.WriteAttributeStringAsync(null, "value", null, thumbnail);
                        // only display if random
                        if (adEvent.PlayOutMethod.Equals("random", StringComparison.OrdinalIgnoreCase))
                            await writer.WriteAttributeStringAsync(null, "weights", null,
                                resource.PlayoutWeight.ToString());

                        var actions = await service.GetActionsAsync(adEvent.Id, resource.Sequence);
                        resource.Actions = actions;

                        foreach (var action in actions)
                        {
                            if (action.Checked == 0)
                                continue;
                            // <action value="./../asset/200M_1280x720_201709.jpg" type="image" parameter="none" code="blue"/> 
                            await writer.WriteStartElementAsync(null, "action", null);
                            await writer.WriteAttributeStringAsync(null, "value", null, _leadingRelativePath + action.Action);
                            await writer.WriteAttributeStringAsync(null, "type", null, action.Type);
                            await writer.WriteAttributeStringAsync(null, "parameter", null, action.Parameter);
                            await writer.WriteAttributeStringAsync(null, "code", null, action.Color);

                            // action end
                            await writer.WriteEndElementAsync();
                        }

                        // asset end
                        await writer.WriteEndElementAsync();
                    }

                    // assets end
                    await writer.WriteEndElementAsync();
                    // duration end
                    await writer.WriteEndElementAsync();
                    // playMode end
                    await writer.WriteEndElementAsync();
                    // child end
                    await writer.WriteEndElementAsync();
                    // children end
                    await writer.WriteEndElementAsync();
                    // block end
                    await writer.WriteEndElementAsync();
                }
            // entry end
            await writer.WriteEndElementAsync();
            // root end
            await writer.WriteEndElementAsync();
            // document end
            await writer.WriteEndDocumentAsync();

            await writer.FlushAsync();
            return adEvent;
        }

        public static async Task<FileStream> GenerateDownloadPackageAsync(DateTime date)
        {
            AdService adService = new AdService();
            ScheduleService service = new ScheduleService();
            List<ScheduleEvent> events = service.GetSchedulesByDateAsync(date).Result;

            // cleanup before proceed
            if (Directory.Exists(DownloadPackageDirectory))
                Directory.Delete(DownloadPackageDirectory, true);
            if (Directory.Exists(DownloadPackageImageDirectory))
                Directory.Delete(DownloadPackageImageDirectory, true);

            // download
            Directory.CreateDirectory(DownloadPackageDirectory);
            Directory.CreateDirectory(DownloadPackageImageDirectory);

            List<string> paths = new List<string>();

            foreach (var scheduleEvent in events)
            {
                var adEvent = await adService.GetAdEventByIdAsync(scheduleEvent.EventId);

                adEvent = await GenerateAsync(adEvent, Path.Combine(DownloadPackageDirectory, "temp.xml"));

                adEvent.SoSettings.ForEach(s =>
                {
                    var destDirectory = Path.Combine(DownloadPackageDirectory, $"SO{s.Code}");
                    var destPath = Path.Combine(destDirectory, "data.xml");

                    if (!Directory.Exists(destDirectory))
                        Directory.CreateDirectory(destDirectory);

                    adEvent.Resources.ForEach(e =>
                    {
                        paths.Add(e.Path.Replace("/", @"\"));
                        paths.AddRange(from obj in e.Actions
                            where obj.Type.Equals("image", StringComparison.OrdinalIgnoreCase) &&
                                  !string.IsNullOrWhiteSpace(obj.Action)
                            select obj.Action.Replace("/", @"\"));
                    });

                    if (File.Exists(destPath))
                        File.Delete(destPath);
                    File.Copy(DownloadPackageTempXmlPath, destPath);
                });
            }

            paths.GroupBy(s => s).Select(z => z.First()).ToList().ForEach(path =>
            {
                var sourcePath = Path.Combine(LocalAssetsDirectory, path);
                var source = Path.GetFileName(sourcePath);
                var destPath = Path.Combine(DownloadPackageImageDirectory, source);

                if (File.Exists(destPath))
                    File.Delete(destPath);
                File.Copy(sourcePath, Path.Combine(destPath));
            });

            if (File.Exists(DownloadPackageTempXmlPath))
                File.Delete(DownloadPackageTempXmlPath);

            if (File.Exists(ZipPackageOutPath))
                File.Delete(ZipPackageOutPath);

            ZipFile.CreateFromDirectory(DownloadPackageDirectory, ZipPackageOutPath);
            return new FileStream(ZipPackageOutPath, FileMode.Open);
        }
    }
}