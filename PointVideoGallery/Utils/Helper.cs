using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
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

        public static int GetUserId(HttpRequestMessage request)
        {
            var claim = request.GetOwinContext().Authentication.User.FindFirst(s => s.Type == ClaimTypes.NameIdentifier);
            return Convert.ToInt32(claim.Value);
        }
    }
}