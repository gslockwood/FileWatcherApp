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

        protected IList<Grade> Grades = [];

        protected readonly FileSystemEventWatcher? watcher;
        protected readonly string lsoGradeTableJson;

        protected readonly TimeChecker timeChecker = new();

        public LsoGradeTranslatorBase(string lsoGradeTableJson)
        {
            if( !File.Exists(lsoGradeTableJson) ) throw new FileNotFoundException(lsoGradeTableJson);

            this.lsoGradeTableJson = lsoGradeTableJson;

            Grades = GetGrades();

            FileInfo fileInfo = new FileInfo(lsoGradeTableJson);
            if( string.IsNullOrEmpty(fileInfo.DirectoryName) ) throw new FileNotFoundException(fileInfo.DirectoryName);

            watcher = new FileSystemEventWatcher(fileInfo.DirectoryName, fileInfo.Name, null);

            // Subscribe to the events.
            watcher.FileChanged += (sender, FileSystemEventArgs) =>
            {
                if( timeChecker.IsLessThanSecondsSinceLastExecution(5) ) return;

                Grades = GetGrades();
                Refresh?.Invoke();
            };
            //watcher.FileCreated += OnFileCreated;
            //watcher.FileDeleted += OnFileDeleted;
            //watcher.FileRenamed += OnFileRenamed;
            //watcher.FileError += OnFileError;

            watcher.StartWatching();
        }

        protected IList<Grade> GetGrades()
        {
            if( FileAccessChecker.IsFileAccessible(lsoGradeTableJson) )
            {
                Logger.Log($"'{lsoGradeTableJson}' is currently accessible.");
                string json = File.ReadAllText(lsoGradeTableJson);
                IList<Grade>? list = System.Text.Json.JsonSerializer.Deserialize<IList<Grade>>(json);
                if( list == null ) return [];

                return new List<Grade>(list);
            }

            else
            {
                try
                {
                    FileAccessChecker.WaitForFileAccess(lsoGradeTableJson, 10000); // Wait up to 10 seconds

                    try
                    {
                        string json = File.ReadAllText(lsoGradeTableJson);
                        IList<Grade>? list = System.Text.Json.JsonSerializer.Deserialize<IList<Grade>>(json);
                        if( list == null ) return [];

                        return new List<Grade>(list);
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

            return Grades;

        }
        protected string? GetGrade(string score)
        {
            IEnumerable<Grade> found = Grades.Where(x => x.LSOGrade != null && x.LSOGrade.Contains(score));
            if( !found.Any() ) return null;
            return found.First().Translation;
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

                string? translation = GetGrade(score);

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

            if( !Grades.Any() ) Grades = GetGrades();

            return null;

        }

        public string? Translate(string lsoGrade)
        {
            if( string.IsNullOrWhiteSpace(lsoGrade) )
                return "No grade provided.";

            if( !Grades.Any() ) Grades = GetGrades();

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
