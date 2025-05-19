using DCSDebriefFile;
using Utililites;

namespace FileWatcherApp
{
    class Program
    {
        private static ReadReader? readReader;
        static void Main(string[] args)
        {
            //string logFilePath = @"C:\Users\george s. lockwood\Saved Games\DCS\Missions\FA-18C\F18 Case Recoveries\debriefing.log";

            //if( File.Exists(logFilePath) )
            //{
            //    Logger.Log($"File: {logFilePath} doesn't exist.");
            //    return;
            //}

            FileWatcher.FileSystemEventWatcher watcher = new FileWatcher.FileSystemEventWatcher(@"C:\Users\george s. lockwood\Saved Games\DCS\Missions\FA-18C\F18 Case Recoveries", "debriefing.log", null);

            // Subscribe to the events.
            watcher.FileChanged += Watcher_FileChanged;
            //watcher.FileChanged += (sender, FileSystemEventArgs) => { };
            //watcher.FileCreated += OnFileCreated;
            //watcher.FileDeleted += OnFileDeleted;
            //watcher.FileRenamed += OnFileRenamed;
            //watcher.FileError += OnFileError;

            watcher.StartWatching(); // Start the watcher.

            readReader = new DCSDebriefFile.ReadReader(@".\LSOGRADETABLE.json");
            readReader.ReadCompleted += (list) =>
            {
                if( TimeChecker.IsLessThan30SecondsSinceLastExecution(5) ) return;

                if( list == null || list.Count == 0 ) return;
                ProcessLSOList(list);
            };
            Logger.Log("Watching for changes. Press any key to exit.");
            Console.ReadKey();
        }

        public class TimeChecker
        {
            private static DateTime lastExecutionTime = DateTime.MinValue;
            //private static  TimeSpan threshold = TimeSpan.FromSeconds(30);

            public static bool IsLessThan30SecondsSinceLastExecution(int seconds)
            {
                TimeSpan threshold = TimeSpan.FromSeconds(seconds);

                DateTime now = DateTime.Now;
                TimeSpan difference = now - lastExecutionTime;

                if( difference < threshold )
                    return true;

                else
                {
                    lastExecutionTime = now;
                    return false;
                }
            }
        }
        private static void Watcher_FileChanged(object? sender, FileSystemEventArgs e)
        {
            if( e == null ) return;
            string inputFilePath = e.FullPath;

            Logger.Log($"File {inputFilePath} has been changed at {DateTime.Now}.");

            readReader?.ReadFile(inputFilePath);

        }
        private static void ProcessLSOList(List<LSOGradeInfo> list)
        {
            foreach( DCSDebriefFile.LSOGradeInfo lsoGradeInfo in list )
            {
                Logger.Log($"{lsoGradeInfo.LsoGradesComment}\n{lsoGradeInfo.Translation}\n\n");
                Logger.Log($"{lsoGradeInfo.LsoGradesComment}\n{lsoGradeInfo.Translation}\n\n");
            }

        }
    }

}
//namespace Utililites
//{
//    public class Logger
//    {
//        public static void Log(object message)
//        {
//#if DEBUG
//            System.Diagnostics.Debug.WriteLine(message);
//#else
//            Console.WriteLine(message);
//#endif
//        }
//    }
//}