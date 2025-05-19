using System.Globalization;
using System.Text.RegularExpressions;
using Utililites;
using static DCSDebriefFile.LSOGrade;

namespace DCSDebriefFile
{
    public class LsoGradeTranslatorLSOGradeFile : LsoGradeTranslatorBase
    {
        public LsoGradeTranslatorLSOGradeFile(string lsoGradeTableJson) : base(lsoGradeTableJson) { }

        public override LSOGrade? GetLSOGrade(string lsoGrade)
        {
            base.GetLSOGrade(lsoGrade);

            //again:
            lsoGrade = lsoGrade.ToUpper().Replace("_", " ").Replace("(", " ").Replace(")", " ");

            lsoGrade = lsoGrade.Replace(" #", "#").Replace("# ", "#");

            string wireNumber = "UNK";
            //string? grade;

            string pattern = @"WIRE#\s*(\d+)";
            Match match = Regex.Match(lsoGrade, pattern);
            if( match.Success )
            {
                wireNumber = match.Groups[1].Value;  // Extracts the wire number
                lsoGrade = Regex.Replace(lsoGrade, pattern, "").Trim(); // Removes the wire segment
            }

            //again:
            var array = lsoGrade.Split(',');
            if( array.Length == 5 )
            {
                //LSOStatement lsoStatement = new(grade, details, lsoGradeItemList, wireNumber);
                LSOGrade lsoStatement = new();

                string format = "yyyy-MM-dd HH:mm:ss";
                DateTime dateTime; if( DateTime.TryParseExact(array[0], format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime) )
                    lsoStatement.DateTime = dateTime;
                else
                    lsoStatement.DateTime = DateTime.Now;

                var eventId = array[1];
                lsoStatement.Pilot = array[2];
                lsoStatement.Carrier = array[3];

                if( wireNumber.Equals("UNK") )
                    lsoStatement.WireCaught = wireNumber;
                else
                    lsoStatement.WireCaught = $"Wire # {wireNumber} caught";


                lsoGrade = array[4];

                pattern = @"GRADE\s*?:\s*?([A-Z]{1,2}|[-]{3})\s*?(.*)";

                match = Regex.Match(lsoGrade, pattern);
                if( match.Success )
                {
                    lsoStatement.Grade = match.Groups[1].Value.Trim();  // Extracts the Grade (e.g., "C")

                    string details = match.Groups[2].Value; // Extracts the remaining part

                    //grade = $"{grade}: {GetGrade(grade)}";

                    details = details.Replace(':', ' ').Trim();
                    details = Regex.Replace(details, @"\s+", " ");

                    lsoStatement.ErrorStr = details;

                    IList<LSOGradeError>? lsoGradeItemList = GetErrors(details);

                    lsoStatement.Errors = lsoGradeItemList;

                    return lsoStatement;
                }

            }

            return null;
        }
        public override List<LSOGrade>? GetLSOGrades(string dcsBriefingLog)
        {
            //again:
            IEnumerable<string> lines = File.ReadAllLines(dcsBriefingLog);//.Skip(1);

            List<LSOGrade> lsoGradesInfo = [];

            //goto again;
            foreach( var line in lines )
            {
                Logger.Log("\t" + line);
                var array = line.Split(',');
                if( array.Length < 3 ) continue;

                Logger.Log("\t\t" + array[1]);
                Logger.Log("\t\t\t" + array[2]);

                if( !array[1].Equals("S_EVENT_LANDING_QUALITY_MARK") ) continue;

                LSOGrade? statement = GetLSOGrade(line);
                if( statement != null )
                    lsoGradesInfo.Add(statement);
            }

            //return lsoGradesInfo;

            //goto again;

            return lsoGradesInfo;

        }

    }
}
