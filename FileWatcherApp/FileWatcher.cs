//namespace FileWatcherApp
//{
//    public class FileWatcher
//    {
//        private readonly string _filePath;
//        private DateTime _lastWriteTime;

//        // Define a delegate for the file changed event
//        public delegate void FileChangedEventHandler(object sender, EventArgs e);

//        // Define the event
//        public event FileChangedEventHandler? FileChanged;

//        public FileWatcher(string filePath)
//        {
//            if( string.IsNullOrEmpty(filePath) )
//            {
//                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
//            }

//            _filePath = filePath;
//            if( File.Exists(_filePath) )
//            {
//                _lastWriteTime = File.GetLastWriteTimeUtc(_filePath);
//            }
//            else
//            {
//                Logger.Log($"Warning: File '{_filePath}' not found. Will start monitoring when it's created.");
//                _lastWriteTime = DateTime.MinValue; // Initialize to a very old date
//            }
//        }

//        public void StartWatching()
//        {
//            Logger.Log($"Watching file: {_filePath}");
//            while( true )
//            {
//                try
//                {
//                    if( File.Exists(_filePath) )
//                    {
//                        DateTime currentWriteTime = File.GetLastWriteTimeUtc(_filePath);
//                        if( currentWriteTime > _lastWriteTime )
//                        {
//                            _lastWriteTime = currentWriteTime;
//                            OnFileChanged();
//                        }
//                    }
//                    // Check for changes every second (adjust as needed)
//                    System.Threading.Thread.Sleep(1000);
//                }
//                catch( FileNotFoundException )
//                {
//                    Logger.Log($"Warning: File '{_filePath}' no longer exists. Waiting for it to reappear.");
//                    _lastWriteTime = DateTime.MinValue; // Reset last write time
//                    System.Threading.Thread.Sleep(5000); // Wait longer if the file disappears
//                }
//                catch( Exception ex )
//                {
//                    Logger.Log($"An error occurred: {ex.Message}");
//                    System.Threading.Thread.Sleep(5000); // Wait after an error
//                }
//            }
//        }

//        protected virtual void OnFileChanged()
//        {
//            LuaFileEventArgs luaFileEventArgs = new(_filePath);
//            // Raise the event if there are any subscribers
//            FileChanged?.Invoke(this, luaFileEventArgs);
//        }
//    }

//    public class LuaFileEventArgs : EventArgs
//    {
//        public string FilePath { get; }
//        public LuaFileEventArgs(string filePath)
//        {
//            FilePath = filePath;
//        }

//    }

//}