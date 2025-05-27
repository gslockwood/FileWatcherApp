using System.Globalization;
using System.Text.RegularExpressions;
using Utililites;
using static DCSDebriefFile.LSOGrade;

namespace DCSDebriefFile
{
    public class LsoGradeTranslatorLSOGradeFile : LsoGradeTranslatorBase
    {
        public LsoGradeTranslatorLSOGradeFile(string lsoGradeTableJson) : base(lsoGradeTableJson) { }

        const short eventIdIndex = 1;
        const short pilotIndex = 2;
        const short unitTypeIndex = 3;
        const short carrierIndex = 4;
        const short lsoGradeIndex = 5;

        public override LSOGrade? GetLSOGrade(string lsoGrade)
        {
            base.GetLSOGrade(lsoGrade);

            //again:
            //lsoGrade = lsoGrade.ToUpper().Replace("_", " ").Replace("(", " ").Replace(")", " ");

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
            if( array.Length == 6 )
            {
                //LSOStatement lsoStatement = new(grade, details, lsoGradeItemList, wireNumber);
                LSOGrade lSOGrade = new();

                string format = "yyyy-MM-dd HH:mm:ss";
                if( DateTime.TryParseExact(array[0], format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime) )
                    lSOGrade.DateTime = dateTime;
                else
                    lSOGrade.DateTime = DateTime.Now;

                //var eventId = array[eventIdIndex];
                lSOGrade.Pilot = array[pilotIndex];
                lSOGrade.UnitType = array[unitTypeIndex];
                lSOGrade.Carrier = array[4];

                if( wireNumber.Equals("UNK") )
                    lSOGrade.WireCaught = wireNumber;
                else
                    lSOGrade.WireCaught = $"Wire# {wireNumber}";
                //lSOGrade.WireCaught = $"Wire # {wireNumber} caught";


                lsoGrade = array[lsoGradeIndex];

                //agag:
                //pattern = @"GRADE\s*?:\s*?([A-Z]{1,2}|[-]{3})\s*?(.*)";

                pattern = @"GRADE\s*?:\s*?([_]?[A-Z]{1,2}[_]?|[-]{3})\s*?(.*)";

                match = Regex.Match(lsoGrade, pattern);
                if( match.Success )
                {
                    lSOGrade.Grade = match.Groups[1].Value.Trim();  // Extracts the Grade (e.g., "C")

                    string details = match.Groups[2].Value; // Extracts the remaining part

                    //grade = $"{grade}: {GetGrade(grade)}";

                    details = details.Replace(':', ' ').Trim();
                    details = Regex.Replace(details, @"\s+", " ");

                    lSOGrade.ErrorStr = details;

                    IList<LSOGradeError>? lsoGradeItemList = GetErrors(details);

                    lSOGrade.Errors = lsoGradeItemList;

                    return lSOGrade;
                }

                //goto agag;

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
