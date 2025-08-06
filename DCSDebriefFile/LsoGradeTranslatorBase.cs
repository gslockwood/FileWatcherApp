using DCSDebriefFileData;
using FileWatcher;
using System.Text;
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
        private static readonly string[] separator = [" ", "[", "]"];

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

        public static Dictionary<string, string> BuildDictionaryFromTabSeparatedString(string inputString)
        {
            // Initialize a new dictionary to store the key-value pairs.
            Dictionary<string, string> resultDictionary = [];

            // Split the input string into individual lines.
            // It handles both Windows-style (\r\n) and Unix-style (\n) newlines.
            // StringSplitOptions.RemoveEmptyEntries ensures that any empty lines (e.g., consecutive newlines)
            // are not processed, preventing potential issues with empty string parts.
            string[] lines = inputString.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Iterate through each line extracted from the input string.
            foreach( string line in lines )
            {
                // Split each line by the tab character ('\t').
                // StringSplitOptions.RemoveEmptyEntries ensures that if there are multiple
                // tabs together (e.g., "KEY\t\tVALUE"), empty strings between them are ignored.
                string[] parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);

                // Iterate through the parts array, taking two elements at a time:
                // one for the key and the subsequent one for the value.
                // The loop increments by 2 (i += 2) because each iteration consumes a key-value pair.
                for( int i = 0; i < parts.Length; i += 2 )
                {
                    // Ensure that there is a valid pair (key and value) available.
                    // i + 1 must be less than the total number of parts to ensure 'parts[i+1]' exists.
                    if( i + 1 < parts.Length )
                    {
                        // Get the key and trim any leading or trailing whitespace.
                        string key = parts[i].Trim();
                        // Get the value and trim any leading or trailing whitespace.
                        string value = parts[i + 1].Trim();

                        // Before adding, check if the extracted key is not null or empty.
                        // An empty key might result from malformed input and is generally undesirable in a dictionary.
                        if( !string.IsNullOrEmpty(key) )
                        {
                            // Add the key-value pair to the dictionary.
                            // If the key already exists, this operation will overwrite its existing value.
                            // If different behavior is desired for duplicate keys (e.g., throwing an exception,
                            // ignoring the new value, or collecting all values for a key),
                            // additional logic (like checking `!resultDictionary.ContainsKey(key)`) would be needed here.
                            resultDictionary[key] = value;
                        }
                    }
                    else
                    {
                        // This block executes if a line contains an odd number of tab-separated parts,
                        // meaning there's a key without a corresponding value at the very end of the line.
                        // It prints a warning to the console, indicating potential malformed input.
                        Console.WriteLine($"Warning: Found a key '{parts[i].Trim()}' without a corresponding value on line: '{line}'");
                    }
                }
            }

            return resultDictionary;
        }

        protected static void AddErrors(IList<DataPair> errors)
        {
            string moreValues = "AA\tAngling Approach\tHO\tHold Off\tOR\tOverrotate\tTMP\tToo Much Power\r\nACC\tAccelerate\tLIG\tLong In Groove\tOS\tOvershoot\tTMRD\tToo Much Rate of Descent\r\nAFU\tAll 'Fouled' Up\tLL\tLanded Left\tOSCB\tOvershoot Coming Back\tTMRR\tToo Much Right Rudder\r\nB\tFlat glideslope\tLLU\tLate LineUp\tP\tPower\tTTL\tTurned TooLate\r\nC\tClimbing\tLO\tLow\tPD\tPitching Deck\tTTS\tTurned TooSoon\r\nCB\tComing Back tolineup\tLR\tLanded Right\tPNU\tPulled Nose Up\tTWA\tTooWide Abeam\r\nCD\tComing Down\tLTR\tLeftTo Right\tROT\tRotate\tW\tWings\r\nCH\tChased\t35LU\tLineUp\tRUD\tRudder\tWU\tWrapped Up\r\nCO\tCome-On\tLUL\tLined Up Left\tRUF\tRough\tXCTL\tCross Control\r\nCU\tCocked Up\tLUR\tLined Up Right\tRWD\tRight Wing Down (DWR)\t1\tFly through glideslope (going up)\r\nDD\tDeck Down\tLWD\tLeft Wing Down (DWL)\tRR\tRight Rudder\t\\\tFly through glideslope (going down)\r\nDEC\tDecelerate\tN\tNose\tRTL\tRight\tTo Left\tLLWD\tLanded LeftWing Down\r\nDL\tDrifted Left\tNC\tNice Correction\tS\tSettle\tLRWD\tLanded Right Wing Down\r\nDN\tDropped Nose\tND\tNose Down\tSD\tSpotted Deck\tLNF\tLanded Nose First\r\nDR\tDrifted Right\tNEA\tNotEnough Attitude\tSHT\tShip's Turn\t\tOver the Top\r\nDU\tDeckup\tNEP\tNotEnough Power\tSKD\tSkid\t\t\r\nEG\tEased Gun\tNERD\tNot Enough Rateof Descent\tSLO\tSlow\t\t\r\nF\tFast\tNERR\tNot Enough Right Rudder\tSRD\tStopped RateofDescent\t\t\r\nFD\tFouled Deck\tNESA\tNotEnough Straight Away\tST\tSteep Turn\t\t\r\nGLI\tGliding Approach\t\tNo Hook\tTCA\tToo Close Abeam\t\t\r\nH\tHigh\tNSU\tNotSet Up\tTMA\tToo Much Attitude\t\t\r\n";

            Dictionary<string, string> myDictionary = BuildDictionaryFromTabSeparatedString(moreValues);
            foreach( KeyValuePair<string, string> entry in myDictionary )
            {
                IEnumerable<DataPair> found = errors.Where(x => x.LSOGrade != null && x.LSOGrade.Contains(entry.Key));
                if( found.Any() ) continue;

                DataPair dataPair = new()
                {
                    LSOGrade = entry.Key,
                    Translation = entry.Value
                };
                errors.Add(dataPair);
                //
            }

        }


        //protected IList<DataPair> GetGrades()
        protected LSOGradeAndErrors? GetGrades()
        {
            if( FileAccessChecker.IsFileAccessible(lsoGradeTableJson) )
            {
                Logger.Log($"'{lsoGradeTableJson}' is currently accessible.");
                string json = File.ReadAllText(lsoGradeTableJson);
                LSOGradeAndErrors? grades = System.Text.Json.JsonSerializer.Deserialize<LSOGradeAndErrors>(json);

                if( grades != null && grades.Errors != null )
                    AddErrors(grades.Errors);

                return grades;

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
            //if( !found.Any() ) return null;
            if( !found.Any() )
            {
                //"WOAFUTL"
                List<string> list = Utilities.StringParser.ParseStringIncrementally(score);
                if( list.Count != 0 )
                {
                    StringBuilder sb = new();
                    foreach( string item in list )
                    {
                        found = this.lsoGradeAndErrors.Errors.Where(x => x.LSOGrade != null && x.LSOGrade.Equals(item));
                        if( found.Any() )
                            sb.Append(found.First().Translation + " ");
                    }
                    return sb.ToString().TrimEnd();
                }
                else
                    return null;
            }

            return ( isSlightly ) ? $"Slightly {found.First().Translation}" : ( isVery ) ? $"Very {found.First().Translation}" : found.First().Translation;

        }
        public IList<LSOGradeError>? GetErrors(string errorStr)
        {
            if( string.IsNullOrWhiteSpace(errorStr) ) return null;

            string details = Regex.Replace(errorStr, @"\s+", " ");
            var list = details.Split(separator, StringSplitOptions.TrimEntries).ToList();//' '
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

            GC.SuppressFinalize(this);
        }

    }
}
