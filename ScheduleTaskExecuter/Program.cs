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

namespace ScheduleTaskExecuter
{
    class Program
    {
        private static string _leadingRelativePath = ConfigurationManager.AppSettings["XmlAssetsLeadingRelativePath"];
        private static string _xmlOutputPath = ConfigurationManager.AppSettings["XmlPath"];

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Environment.Exit(-1);
                return;
            }
            int id;
            if (Int32.TryParse(args[0], out id))
                Generate(id);
            else
                Console.WriteLine("INVALID INPUT");
        }


        public static void Generate(int id)
        {
            AdService service = new AdService();
            var adEvent = service.GetAdEventByIdAsync(id).Result;

            var setting = new XmlWriterSettings {Async = false, Encoding = Encoding.UTF8, Indent = true};
            var startDate = DateTime.Today;

            using (var writer = XmlWriter.Create(_xmlOutputPath, setting))
            {
                // <?xml version="1.0" encoding="utf-8"?>
                writer.WriteStartDocument();
                // <root xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
                writer.WriteStartElement("root");
                writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");

                // <entry time="2016/09/19 00:00:00">
                writer.WriteStartElement("entry");
                writer.WriteAttributeString("time", startDate.ToString("yyyy-MM-dd HH:mm:ss"));

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

                        foreach (var resource in adEvent.Resources)
                        {
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
        }
    }
}