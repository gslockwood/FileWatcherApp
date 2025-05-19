using System.ComponentModel; // For ISynchronizeInvoke
using Utililites;
namespace FileWatcher
{
    public class FileSystemEventWatcher : IDisposable
    {
        private FileSystemWatcher? _watcher;
        private Thread? _workerThread;
        private Mutex? _mutex;
        private bool _disposed = false;
        private string? _directoryToWatch;
        private string? filename;
        private ISynchronizeInvoke? _synchronizingObject; // For UI synchronization (WinForms)

        // Events to notify about file system changes.  These now use EventHandler
        public event EventHandler<FileSystemEventArgs>? FileChanged;
        public event EventHandler<FileSystemEventArgs>? FileCreated;
        public event EventHandler<FileSystemEventArgs>? FileDeleted;
        public event EventHandler<RenamedEventArgs>? FileRenamed;
        public event EventHandler<ErrorEventArgs>? FileError;

        /// <summary>
        /// Initializes a new instance of the FileSystemEventWatcher class.
        /// </summary>
        /// <param name="directoryToWatch">The directory to watch for changes.</param>
        /// <param name="synchronizingObject">
        ///     An object that implements ISynchronizeInvoke to marshal events back to a specific thread
        ///     (e.g., the UI thread in a WinForms application).  If null, events are raised on a thread
        ///     pool thread.
        /// </param>
        public FileSystemEventWatcher(string directoryToWatch, string? filename, ISynchronizeInvoke? synchronizingObject)
        {
            if( string.IsNullOrEmpty(directoryToWatch) )
            {
                throw new ArgumentException("Directory to watch cannot be null or empty.", nameof(directoryToWatch));
            }

            if( !Directory.Exists(directoryToWatch) )
            {
                throw new DirectoryNotFoundException($"The directory '{directoryToWatch}' does not exist.");
            }

            _directoryToWatch = directoryToWatch;
            this.filename = filename;
            _synchronizingObject = synchronizingObject; // Store the synchronizing object.

            _mutex = new Mutex(false, "FileSystemEventWatcherMutex_" + Guid.NewGuid()); // Give the Mutex a unique name.
        }

        /// <summary>
        /// Starts watching for file system changes in the specified directory.
        /// </summary>
        //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void StartWatching()
        {
            if( _disposed )
            {
                throw new ObjectDisposedException(nameof(FileSystemEventWatcher));
            }

            if( _workerThread != null && _workerThread.IsAlive )
            {
                return; // Already watching.
            }

            // Create and configure the FileSystemWatcher on the worker thread.
            _workerThread = new Thread(SetupAndWatch);
            _workerThread.Start();
            //
        }

        private void SetupAndWatch()
        {
            if( _directoryToWatch == null ) throw new NullReferenceException(nameof(_directoryToWatch));

            try
            {
                // Request the mutex.  This will block if it's held by another thread.
                if( _mutex != null && !_mutex.WaitOne(Timeout.Infinite) )
                {
                    // Handle the case where we fail to acquire the mutex.  This should not normally happen,
                    // but it's good practice to check.  Consider logging an error.
                    Logger.Log("Failed to acquire mutex in StartWatching."); // IMPORTANT: Replace with proper logging.
                    return; // IMPORTANT:  Exit the thread!  Otherwise, you'll have a thread running without the mutex.
                }

                try
                {
                    // Create the FileSystemWatcher.  Do this inside the thread that will use it.
                    _watcher = new FileSystemWatcher();
                    _watcher.Path = _directoryToWatch;
                    _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                    _watcher.Filter = "*.*";
                    _watcher.IncludeSubdirectories = true; // Watch subdirectories too.

                    // Set the SynchronizingObject.  This is crucial for WinForms!
                    _watcher.SynchronizingObject = _synchronizingObject;

                    // Subscribe to events.  Use the standard EventHandler.
                    _watcher.Changed += OnChanged;
                    _watcher.Created += OnCreated;
                    _watcher.Deleted += OnDeleted;
                    _watcher.Renamed += OnRenamed;
                    _watcher.Error += OnError;  // Add Error event handling

                    _watcher.EnableRaisingEvents = true; // Start watching.
                    Logger.Log($"Started watching directory: {_directoryToWatch} on thread {Thread.CurrentThread.ManagedThreadId}");

                    // Keep the thread alive.  The FileSystemWatcher will raise events on this thread.
                    // We don't use Thread.Sleep(Timeout.Infinite) because the FileSystemWatcher
                    // needs the thread to be responsive to events.  Instead, we can use a simple
                    // loop that waits for a signal from the StopWatching method.  The Mutex
                    // is used to ensure that the thread doesn't exit until we're done.
                    while( true )
                    {
                        // The thread will stay here, processing events.
                        Thread.Sleep(1000); // Sleep for a bit to reduce CPU usage, but still be responsive.
                        if( _disposed )
                            break;
                    }
                    Logger.Log($"Stopped watching directory: {_directoryToWatch} on thread {Thread.CurrentThread.ManagedThreadId}");
                }
                finally
                {
                    // Ensure the mutex is released, even if an exception occurs.
                    _mutex?.ReleaseMutex();
                }
            }
            catch( Exception ex )
            {
                // Handle exceptions during setup or watching.  IMPORTANT: Log the error!
                Logger.Log($"Exception in SetupAndWatch: {ex}");
                // Consider raising the Error event here, if appropriate.
                OnError(this, new ErrorEventArgs(ex)); // Raise the error event.
            }
        }

        /// <summary>
        /// Stops watching for file system changes and releases resources.
        /// </summary>
        public void StopWatching()
        {
            if( _disposed )
            {
                return; // Already stopped.
            }

            // Signal the worker thread to stop and wait for it to finish.
            _disposed = true; // Set the disposed flag, which will cause the worker thread to exit its loop.

            if( _watcher != null )
            {
                _watcher.EnableRaisingEvents = false; // Stop watcher from raising events.
                _watcher.Changed -= OnChanged;
                _watcher.Created -= OnCreated;
                _watcher.Deleted -= OnDeleted;
                _watcher.Renamed -= OnRenamed;
                _watcher.Error -= OnError;
                _watcher.Dispose(); // Release resources.
                _watcher = null;
            }

            if( _workerThread != null )
            {
                _workerThread.Join(); // Wait for the worker thread to terminate.
                _workerThread = null;
            }
            //IMPORTANT: Do NOT release the Mutex here.  The thread that ACQUIRED the mutex must release it.
            //The Mutex is released in the SetupAndWatch method.
        }

        // Event handlers for FileSystemWatcher events.  These now use EventHandler.
        private void OnChanged(object? sender, FileSystemEventArgs e)
        {
            if( this.filename != null && e.Name != null && !e.Name.Equals(this.filename) ) return;

            // Use the stored ISynchronizeInvoke object to marshal the event back to the UI thread, if available.
            if( _synchronizingObject != null && _synchronizingObject.InvokeRequired )
            {
                //_synchronizingObject.Invoke(new System.Reflection.MethodInvoker(() => FileChanged?.Invoke(this, e)), null);
            }
            else
            {
                FileChanged?.Invoke(this, e); // Raise the event directly.
            }
        }

        private void OnCreated(object? sender, FileSystemEventArgs e)
        {
            if( _synchronizingObject != null && _synchronizingObject.InvokeRequired )
            {
                //_synchronizingObject.Invoke(new System.Reflection.MethodInvoker(() => FileCreated?.Invoke(this, e)), null);
            }
            else
            {
                FileCreated?.Invoke(this, e);
            }
        }

        private void OnDeleted(object? sender, FileSystemEventArgs e)
        {
            if( _synchronizingObject != null && _synchronizingObject.InvokeRequired )
            {
                //_synchronizingObject.Invoke(new MethodInvoker(() => FileDeleted?.Invoke(this, e)), null);
            }
            else
            {
                FileDeleted?.Invoke(this, e);
            }
        }

        private void OnRenamed(object? sender, RenamedEventArgs e)
        {
            if( _synchronizingObject != null && _synchronizingObject.InvokeRequired )
            {
                //_synchronizingObject.Invoke(new MethodInvoker(() => FileRenamed?.Invoke(this, e)), null);
            }
            else
            {
                FileRenamed?.Invoke(this, e);
            }
        }

        private void OnError(object? sender, ErrorEventArgs e)
        {
            if( _synchronizingObject != null && _synchronizingObject.InvokeRequired )
            {
                //_synchronizingObject.Invoke(new MethodInvoker(() => FileError?.Invoke(this, e)), null);
            }
            else
            {
                FileError?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Releases all resources used by the FileSystemEventWatcher.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources used by the FileSystemEventWatcher.
        /// </summary>
        /// <param name="disposing">
        ///   true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if( !_disposed )
            {
                if( disposing )
                {
                    // Dispose of managed resources.
                    StopWatching(); // Stop the watcher and release resources.
                    if( _mutex != null )
                    {
                        _mutex.Dispose();
                        _mutex = null;
                    }
                }
                // Dispose of unmanaged resources (if any).
                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizes an instance of the FileSystemEventWatcher class.
        /// </summary>
        ~FileSystemEventWatcher()
        {
            Dispose(false); // Release unmanaged resources.
        }
    }
}
namespace Utililites
{
    public class Logger
    {
        public static void Log(object message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#else
            Console.WriteLine(message);
#endif
        }
    }
}