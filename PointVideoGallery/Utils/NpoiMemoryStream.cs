using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Utils
{
    public class NpoiMemoryStream : MemoryStream
    {
        public NpoiMemoryStream()
        {
            AllowClose = true;
        }

        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}