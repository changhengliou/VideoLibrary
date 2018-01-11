using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using PointVideoGallery.Services;

namespace PointVideoGallery
{
    public class FileIndexConfig
    {
        public static void RegisterFileIndexService()
        {
            var fileIndexr =
                new FileIndexService(ConfigurationManager.AppSettings.Get("LibraryIndexBasePath"))
                {
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true,
                };
        }
    }
    public class FileIndexService : FileSystemWatcher, IService
    {
        // This Dictionary keeps the track of when an event occured last for a particular file
        private Dictionary<string, DateTime> _lastFileEvent;

        // Interval in Millisecond
        private int _interval;

        //Timespan created when interval is set
        private TimeSpan _recentTimeSpan;

        /// <summary>
        /// Interval, in milliseconds, within which events are considered "recent"
        /// </summary>
        public int Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                _recentTimeSpan = new TimeSpan(0, 0, 0, 0, value);
            }
        }

        public bool FilterRecentEvents { get; set; }

        public FileIndexService() : base()
        {
            InitializeMembers();
        }

        public FileIndexService(string path) : base(path)
        {
            InitializeMembers();
        }

        public FileIndexService(string path, string filter) : base(path, filter)
        {
            InitializeMembers();
        }

        public new event FileSystemEventHandler Changed;
        public new event FileSystemEventHandler Created;
        public new event FileSystemEventHandler Deleted;
        public new event RenamedEventHandler Renamed;

        protected new virtual void OnChanged(FileSystemEventArgs e)
        {
            if (Changed != null) Changed(this, e);
        }

        protected new virtual void OnCreated(FileSystemEventArgs e)
        {
            if (Created != null) Created(this, e);
        }

        protected new virtual void OnDeleted(FileSystemEventArgs e)
        {
            if (Deleted != null) Deleted(this, e);
        }

        protected new virtual void OnRenamed(RenamedEventArgs e)
        {
            if (Renamed != null) Renamed(this, e);
        }


        private void InitializeMembers()
        {
            Interval = 100;
            FilterRecentEvents = true;
            _lastFileEvent = new Dictionary<string, DateTime>();

            base.Created += OnCreated;
            base.Changed += OnChanged;
            base.Deleted += OnDeleted;
            base.Renamed += OnRenamed;
        }

        /// <summary>
        /// This method searches the dictionary to find out when the last event occured 
        /// for a particular file. If that event occured within the specified timespan
        /// it returns true, else false
        /// </summary>
        private bool HasAnotherFileEventOccuredRecently(string fileName)
        {
            bool retVal = false;

            if (FilterRecentEvents)
            {
                if (_lastFileEvent.ContainsKey(fileName))
                {
                    // If dictionary contains the filename, check how much time has elapsed
                    // since the last event occured. If the timespan is less that the 
                    // specified interval, set return value to true 
                    // and store current datetime in dictionary for this file
                    DateTime lastEventTime = _lastFileEvent[fileName];
                    DateTime currentTime = DateTime.Now;
                    TimeSpan timeSinceLastEvent = currentTime - lastEventTime;
                    retVal = timeSinceLastEvent < _recentTimeSpan;
                    _lastFileEvent[fileName] = currentTime;
                }
                else
                {
                    // If dictionary does not contain the filename, 
                    // no event has occured in past for this file, so set return value to false
                    // and annd filename alongwith current datetime to the dictionary
                    _lastFileEvent.Add(fileName, DateTime.Now);
                    retVal = false;
                }
            }

            return retVal;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (!HasAnotherFileEventOccuredRecently(e.FullPath))
                this.OnChanged(e);
            Trace.WriteLine($"NAME = {e.Name}, TYPE = {e.ChangeType}, PATH = {e.FullPath}");
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (!HasAnotherFileEventOccuredRecently(e.FullPath))
                this.OnCreated(e);
            Trace.WriteLine($"NAME = {e.Name}, TYPE = {e.ChangeType}, PATH = {e.FullPath}");
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (!HasAnotherFileEventOccuredRecently(e.FullPath))
                this.OnDeleted(e);
            Trace.WriteLine($"NAME = {e.Name}, TYPE = {e.ChangeType}, PATH = {e.FullPath}");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (!HasAnotherFileEventOccuredRecently(e.OldFullPath))
                this.OnRenamed(e);
            Trace.WriteLine(
                $"NAME = {e.OldName} => {e.Name}, TYPE = {e.ChangeType}, PATH = {e.OldFullPath} => {e.FullPath}");
        }
    }
}