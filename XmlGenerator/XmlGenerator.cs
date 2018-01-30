using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlGenerator
{
    public class XmlGenerator
    {
        public static void Main()
        {
            Generate();
        }
        public static async void Generate()
        {
            var setting = new XmlWriterSettings {Async = true, Encoding = Encoding.UTF8, Indent = true};
            var startDate = DateTime.Today;
            var eventName = "Production - miniEPG";
            var playoutMethod = "interval";
            var playoutSequence = "byasset";
            var playoutTimeSpan = "10";
            var videoSource = "ftp:AD3-M1L2.ts";
            var videoDuration = "600";
            var resourceType = "image";
            var resourcePath = "./../asset/B123.jpg";
            var resourceSeq = "1";
            using (var writer = XmlWriter.Create("test.xml", setting))
            {
                // <?xml version="1.0" encoding="utf-8"?>
                await writer.WriteStartDocumentAsync();
                // <root xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
                await writer.WriteStartElementAsync(null, "root", null);
                await writer.WriteAttributeStringAsync("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
                await writer.WriteAttributeStringAsync("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                
                // <entry time="2016/09/19 00:00:00">
                await writer.WriteStartElementAsync(null, "entry", null);
                await writer.WriteAttributeStringAsync(null, "time", null, startDate.ToString("yyyy-MM-dd HH:mm:ss"));
                
                // <block name="miniepg">
                await writer.WriteStartElementAsync(null, "block", null);
                await writer.WriteAttributeStringAsync(null, "name", null, "miniepg");

                // <children>
                await writer.WriteStartElementAsync(null, "children", null);

                // <child name="Production - miniEPG">
                await writer.WriteStartElementAsync(null, "child", null);
                await writer.WriteAttributeStringAsync(null, "name", null, eventName);

                // <playMode type="interval" subtype="bytime" value="10" />
                await writer.WriteStartElementAsync(null, "playMode", null);
                await writer.WriteAttributeStringAsync(null, "type", null, playoutMethod);
                await writer.WriteAttributeStringAsync(null, "subtype", null, playoutSequence);
                await writer.WriteAttributeStringAsync(null, "value", null, playoutTimeSpan);

                // <video value="ftp:AD3-M1L2.ts" />
                await writer.WriteStartElementAsync(null, "video", null);
                await writer.WriteAttributeStringAsync(null, "value", null, videoSource);

                // <duration value="600" />
                await writer.WriteStartElementAsync(null, "duration", null);
                await writer.WriteAttributeStringAsync(null, "value", null, videoDuration);

                // <assets>
                await writer.WriteStartElementAsync(null, "assets", null);

                // <asset type="image" value="./../asset/B123.jpg"/>1
                await writer.WriteStartElementAsync(null, "asset", null);
                await writer.WriteAttributeStringAsync(null, "type", null, resourceType);
                await writer.WriteAttributeStringAsync(null, "value", null, resourcePath);
                await writer.WriteStringAsync(resourceSeq);
                // asset end
                await writer.WriteEndElementAsync();
                // assets end
                await writer.WriteEndElementAsync();
                // duration end
                await writer.WriteEndElementAsync();
                // video end
                await writer.WriteEndElementAsync();
                // playMode end
                await writer.WriteEndElementAsync();
                // child end
                await writer.WriteEndElementAsync();
                // children end
                await writer.WriteEndElementAsync();
                // block end
                await writer.WriteEndElementAsync();
                // entry end
                await writer.WriteEndElementAsync();
                // root end
                await writer.WriteEndElementAsync();
                // document end
                await writer.WriteEndDocumentAsync();
            }
        }
    }
}