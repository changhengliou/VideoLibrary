using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Utils
{
    public class Helper
    {
        public static void GetFilesInDirectory(string dirPath)
        {
//            string[] fileEntries = Directory.GetFiles(dirPath);
//            foreach (string fileName in fileEntries)
//                Trace.WriteLine("Processed file '{0}'.", fileName);
//
//            // Recurse into subdirectories of this directory.
//            string[] subdirectoryEntries = Directory.GetDirectories(dirPath);
//            foreach (string subdirectory in subdirectoryEntries)
//                GetFilesInDirectory(subdirectory);
            Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
        }
    }
}