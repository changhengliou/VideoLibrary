using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using PointVideoGallery.Services;
using PointVideoGallery.Utils;

namespace PointVideoGallery
{
    public class FileIndexConfig
    {
        public static void RegisterFileIndexService()
        {
            var path = ConfigurationManager.AppSettings.Get("LibraryIndexBasePath");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("Path: " + path + " does't exist. Please check LibraryIndexBasePath in the web.config.");

            var fileIndexer = new FileSystemWatcher(ConfigurationManager.AppSettings.Get("LibraryIndexBasePath"))
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };

            fileIndexer.Changed += OnChanged;
            fileIndexer.Renamed += OnRenamed;
            fileIndexer.Deleted += OnDeleted;
            fileIndexer.Created += OnCreated;

            Helper.GetFilesInDirectory(path);
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Trace.WriteLine($"NAME = {e.Name}, TYPE = {e.ChangeType}, PATH = {e.FullPath}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Trace.WriteLine($"NAME = {e.Name}, TYPE = {e.ChangeType}, PATH = {e.FullPath}");
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Trace.WriteLine($"NAME = {e.Name}, TYPE = {e.ChangeType}, PATH = {e.FullPath}");
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Trace.WriteLine(
                $"NAME = {e.OldName} => {e.Name}, TYPE = {e.ChangeType}, PATH = {e.OldFullPath} => {e.FullPath}");
        }
    }
}