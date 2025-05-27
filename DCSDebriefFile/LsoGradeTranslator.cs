using System.Text.RegularExpressions;

namespace DCSDebriefFile
{
    public class LsoGradeTranslator : LsoGradeTranslatorBase
    {
        //public delegate void RefreshEventHandler();
        //public event RefreshEventHandler? Refresh;

        //private IList<Grade> Grades = [];

        //private readonly FileSystemEventWatcher? watcher;
        //private readonly string lsoGradeTableJson;

        //readonly TimeChecker timeChecker = new();

        public LsoGradeTranslator(string lsoGradeTableJson) : base(lsoGradeTableJson) { }

        //private IList<Grade> GetGrades()
        //{
        //    if( FileAccessChecker.IsFileAccessible(lsoGradeTableJson) )
        //    {
        //        Logger.Log($"'{lsoGradeTableJson}' is currently accessible.");
        //        string json = File.ReadAllText(lsoGradeTableJson);
        //        IList<Grade>? list = System.Text.Json.JsonSerializer.Deserialize<IList<Grade>>(json);
        //        if( list == null ) return [];

        //        return new List<Grade>(list);
        //    }

        //    else
        //    {
        //        try
        //        {
        //            FileAccessChecker.WaitForFileAccess(lsoGradeTableJson, 10000); // Wait up to 10 seconds

        //            try
        //            {
        //                string json = File.ReadAllText(lsoGradeTableJson);
        //                IList<Grade>? list = System.Text.Json.JsonSerializer.Deserialize<IList<Grade>>(json);
        //                if( list == null ) return [];

        //                return new List<Grade>(list);
        //            }
        //            catch( IOException ex )
        //            {
        //                Logger.Log($"IOException reading file after it became accessible: {ex.Message}");
        //            }
        //            catch( Exception ex )
        //            {
        //                Logger.Log($"Exception reading file after it became accessible: {ex.Message}");
        //            }
        //        }
        //        catch( TimeoutException ex )
        //        {
        //            Logger.Log(ex.Message);
        //        }
        //        catch( Exception ex )
        //        {
        //            Logger.Log(ex.Message);
        //        }



        //    }

        //    return Grades;

        //}

        //private string? GetGrade(string score)
        //{
        //    IEnumerable<Grade> found = Grades.Where(x => x.LSOGrade != null && x.LSOGrade.Contains(score));
        //    if( !found.Any() ) return null;
        //    return found.First().Translation;
        //}

        //public override IList<LSOGradeItem>? GetErrors(string errorStr)
        //{
        //    //if( string.IsNullOrWhiteSpace(errorStr) )                return null;

        //    //string details = Regex.Replace(errorStr, @"\s+", " ");
        //    //var list = details.Split(new string[] { " ", "[", "]" }, StringSplitOptions.TrimEntries).ToList();//' '
        //    //if( list.Count == 0 ) throw new NullReferenceException("No suitable Grade given");

        //    //IList<LSOGradeItem> lsoGradeItemList = [];
        //    //foreach( string score in list )
        //    //{
        //    //    if( string.IsNullOrWhiteSpace(score) || score.Contains("WIRE") ) continue;

        //    //    string? translation = GetGrade(score);

        //    //    if( string.IsNullOrWhiteSpace(translation) )
        //    //        translation = $"*** Unable to translate: {score} ***";

        //    //    lsoGradeItemList.Add(new(score, translation));

        //    //}

        //    //return lsoGradeItemList;
        //    //
        //}


        public override LSOGrade? GetLSOGrade(string lsoGrade)
        {
            base.GetLSOGrade(lsoGrade);

            //again:
            lsoGrade = lsoGrade.ToUpper().Replace("_", " ").Replace("(", " ").Replace(")", " ");

            lsoGrade = lsoGrade.Replace(" #", "#").Replace("# ", "#");

            string wireNumber = "UNK";
            string? grade;

            string pattern = @"WIRE#\s*(\d+)";
            Match match = Regex.Match(lsoGrade, pattern);
            if( match.Success )
            {
                wireNumber = match.Groups[1].Value;  // Extracts the wire number
                lsoGrade = Regex.Replace(lsoGrade, pattern, "").Trim(); // Removes the wire segment
            }


            //pattern = @"GRADE\s*?:\s*?([A-Z]{1,2})\s*?(.*)";

            pattern = @"GRADE\s*?:\s*?([A-Z]{1,2}|[-]{3})\s*?(.*)";

            match = Regex.Match(lsoGrade, pattern);
            if( match.Success )
            {
                grade = match.Groups[1].Value.Trim();  // Extracts the Grade (e.g., "C")
                string details = match.Groups[2].Value; // Extracts the remaining part

                //grade = $"{grade}: {GetGrade(grade)}";

                details = details.Replace(':', ' ').Trim();
                details = Regex.Replace(details, @"\s+", " ");

                //IList<LSOGradeError>? lsoGradeItemList = GetErrors(details);

                LSOGrade lSOGrade = new(grade, details, GetErrors(details), wireNumber);

                return lSOGrade;

            }

            return null;
            //
        }

        public override List<LSOGrade>? GetLSOGrades(string dcsBriefingLog)
        {
            var text = File.ReadAllText(dcsBriefingLog);
            text += Environment.NewLine;

            DCSDebriefingValues dcsDebriefingValues = new(text);
            List<Object>? lsoGrades = dcsDebriefingValues.GetLSOGrades();
            if( lsoGrades != null && lsoGrades.Count > 0 )
            {
                DCSDebriefingValues.Place first = (DCSDebriefingValues.Place)lsoGrades.First();
                float timeDuration = first.T;
                DateTime startOfMission = DateTime.Now.AddSeconds(-1 * timeDuration);

                List<LSOGrade> lsoGradesInfo = [];
                foreach( DCSDebriefingValues.Place place in lsoGrades )
                {
                    string? lsoGradesComment = place.Comment;
                    if( lsoGradesComment == null ) continue;

                    //DateTime startTime = DCSDebriefingValues.SecondsToTimeConverter.CalcTime(place.T);
                    DateTime startTime = startOfMission.AddSeconds(place.T);

                    //Utililites.Logger.Log($"{startTime:G}");

                    if( place.Comment != null )
                    {
                        LSOGrade? lSOGrade = this.GetLSOGrade(place.Comment);
                        if( lSOGrade != null )
                        {
                            lSOGrade.DateTime = startTime;
                            if( place.place != null )
                                lSOGrade.Carrier = place.place;
                            if( place.initiatorPilotName != null )
                                lSOGrade.Pilot = place.initiatorPilotName;

                            lsoGradesInfo.Add(lSOGrade);
                        }
                    }

                }

                return lsoGradesInfo;

            }

            return null;
        }

    }
}
