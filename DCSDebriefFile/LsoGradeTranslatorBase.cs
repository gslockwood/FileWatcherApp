using DCSDebriefFileData;
using FileWatcher;
using System.Text.RegularExpressions;
using Utililites;
using static DCSDebriefFile.LSOGrade;

namespace DCSDebriefFile
{
    public class LsoGradeTranslatorBase : ILsoGradeTranslator
    {
        public delegate void RefreshEventHandler();
        public event RefreshEventHandler? Refresh;

        protected LSOGradeAndErrors? lsoGradeAndErrors;
        //protected IList<DataPair> Grades = [];

        protected readonly FileSystemEventWatcher? watcher;
        protected readonly string lsoGradeTableJson;

        protected readonly TimeChecker timeChecker = new();

        public LsoGradeTranslatorBase(string lsoGradeTableJson)
        {
            if( !File.Exists(lsoGradeTableJson) ) throw new FileNotFoundException(lsoGradeTableJson);

            this.lsoGradeTableJson = lsoGradeTableJson;

            lsoGradeAndErrors = GetGrades();

            FileInfo fileInfo = new(lsoGradeTableJson);
            if( string.IsNullOrEmpty(fileInfo.DirectoryName) ) throw new FileNotFoundException(fileInfo.DirectoryName);

            watcher = new FileSystemEventWatcher(fileInfo.DirectoryName, fileInfo.Name, null);

            // Subscribe to the events.
            watcher.FileChanged += (sender, FileSystemEventArgs) =>
            {
                if( timeChecker.IsLessThanSecondsSinceLastExecution(5) ) return;

                lsoGradeAndErrors = GetGrades();
                Refresh?.Invoke();
            };
            //watcher.FileCreated += OnFileCreated;
            //watcher.FileDeleted += OnFileDeleted;
            //watcher.FileRenamed += OnFileRenamed;
            //watcher.FileError += OnFileError;

            watcher.StartWatching();
        }


        public class LSOGradeAndErrors
        {
            public IList<DataPair>? Grades { get; set; }
            public IList<DataPair>? Errors { get; set; }
        }

        //public class Grade
        //{
        //    public string LSOGrade { get; set; }
        //    public string Translation { get; set; }
        //}

        //public class Error
        //{
        //    public string LSOGrade { get; set; }
        //    public string Translation { get; set; }
        //}


        //protected IList<DataPair> GetGrades()
        protected LSOGradeAndErrors? GetGrades()
        {
            if( FileAccessChecker.IsFileAccessible(lsoGradeTableJson) )
            {
                Logger.Log($"'{lsoGradeTableJson}' is currently accessible.");
                string json = File.ReadAllText(lsoGradeTableJson);
                return System.Text.Json.JsonSerializer.Deserialize<LSOGradeAndErrors>(json);

                //IList<DataPair>? list = System.Text.Json.JsonSerializer.Deserialize<IList<DataPair>>(json);
                //if( list == null ) return [];
                //return new List<DataPair>(list);
            }

            else
            {
                try
                {
                    FileAccessChecker.WaitForFileAccess(lsoGradeTableJson, 10000); // Wait up to 10 seconds

                    try
                    {
                        //string json = File.ReadAllText(lsoGradeTableJson);
                        //IList<DataPair>? list = System.Text.Json.JsonSerializer.Deserialize<IList<DataPair>>(json);
                        //if( list == null ) return [];

                        //return new List<DataPair>(list);

                        string json = File.ReadAllText(lsoGradeTableJson);
                        return System.Text.Json.JsonSerializer.Deserialize<LSOGradeAndErrors>(json);

                    }
                    catch( IOException ex )
                    {
                        Logger.Log($"IOException reading file after it became accessible: {ex.Message}");
                    }
                    catch( Exception ex )
                    {
                        Logger.Log($"Exception reading file after it became accessible: {ex.Message}");
                    }
                }
                catch( TimeoutException ex )
                {
                    Logger.Log(ex.Message);
                }
                catch( Exception ex )
                {
                    Logger.Log(ex.Message);
                }



            }

            return null;

        }
        private string? GetGrade(string score)
        {
            if( lsoGradeAndErrors == null ) return "";
            if( lsoGradeAndErrors.Grades == null ) return "";

            IEnumerable<DataPair> found = this.lsoGradeAndErrors.Grades.Where(x => x.LSOGrade != null && x.LSOGrade.Contains(score));
            if( !found.Any() ) return null;

            return found.First().Translation;
            //
        }

        private string? GetError(string score)
        {
            if( lsoGradeAndErrors == null ) return "";
            if( lsoGradeAndErrors.Errors == null ) return "";

            bool isSlightly = false;
            bool isVery = false;

            if( score.Contains('(') ) isSlightly = true;
            if( score.Contains('_') ) isVery = true;

            score = Regex.Replace(score, "[()_]", "");

            IEnumerable<DataPair> found = this.lsoGradeAndErrors.Errors.Where(x => x.LSOGrade != null && x.LSOGrade.Contains(score));
            if( !found.Any() ) return null;

            return ( isSlightly ) ? $"Slightly {found.First().Translation}" : ( isVery ) ? $"Very {found.First().Translation}" : found.First().Translation;

        }
        public IList<LSOGradeError>? GetErrors(string errorStr)
        {
            if( string.IsNullOrWhiteSpace(errorStr) ) return null;

            string details = Regex.Replace(errorStr, @"\s+", " ");
            var list = details.Split(new string[] { " ", "[", "]" }, StringSplitOptions.TrimEntries).ToList();//' '
            if( list.Count == 0 ) throw new NullReferenceException("No suitable Grade given");

            IList<LSOGradeError> lsoGradeItemList = [];
            foreach( string score in list )
            {
                if( string.IsNullOrWhiteSpace(score) || score.Contains("WIRE") ) continue;

                string? translation = GetError(score);

                if( string.IsNullOrWhiteSpace(translation) )
                    translation = $"*** Unable to translate: {score} ***";

                lsoGradeItemList.Add(new(score, translation));

            }

            return lsoGradeItemList;
            //return null; 
        }

        public virtual LSOGrade? GetLSOGrade(string lsoGrade)
        {
            if( string.IsNullOrWhiteSpace(lsoGrade) )
                throw new NullReferenceException("No grade provided.");

            lsoGradeAndErrors ??= GetGrades();
            //if( !Grades.Any() ) Grades = GetGrades();

            return null;

        }

        public string? GetGradeTranslation(string lsoGrade)
        {
            if( string.IsNullOrWhiteSpace(lsoGrade) )
                return "No grade provided.";

            lsoGradeAndErrors ??= GetGrades();
            //if( !Grades.Any() ) Grades = GetGrades();

            string? grade = GetGrade(lsoGrade);

            if( string.IsNullOrWhiteSpace(grade) )
                grade = $"*** Unable to translate: {lsoGrade} ***";

            return grade;
            //
        }
        public virtual List<LSOGrade>? GetLSOGrades(string dcsBriefingLog)
        {
            return null;
        }


        public void Dispose()
        {
            if( watcher != null )
            {
                watcher.StopWatching();
                watcher.Dispose();
                //watcher = null;
            }
        }

    }
}
