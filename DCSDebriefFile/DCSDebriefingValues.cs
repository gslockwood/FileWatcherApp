using System.Text.RegularExpressions;
using Utililites;

namespace DCSDebriefFile
{
    public class DCSDebriefingValues
    {
        private string text;

        public string? Callsign { get; private set; }
        public long Mission_file_mark { get; private set; }
        public string? Mission_file_path { get; private set; }
        public float Mission_time { get; private set; }
        public int Result { get; private set; }

        public Dictionary<string, List<Object>> Properties = [];

        public DCSDebriefingValues(string text)
        {
            this.text = text;

            SetValues(text);
            SetValueObjects(text);
            //
        }


        private void SetValueObjects(string text)
        {
            string pattern = @"^(\b\w+\b)\s*=\s*\{\s*[\s\S]*?\s*^\}";
            MatchCollection matches = Regex.Matches(text, pattern, RegexOptions.Multiline);

            if( matches.Count > 0 )
            {
                foreach( Match match in matches )
                {
                    if( match.Groups.Count == 2 )
                        Properties.Add(match.Groups[1].Value, CreateObjectList(match.Groups[0].Value));
                }

            }
        }

        private List<Object> CreateObjectList(string value)
        {
            List<Object> list = [];

            string pattern = @"\[\d+\]\s*=\s*(\{([\s\S]*?)\})";
            MatchCollection matches = Regex.Matches(value, pattern, RegexOptions.Multiline);
            foreach( Match match in matches )
            {
                string detailStr = match.Groups[2].Value;
                if( detailStr != null )
                {
                    Object obj = GetValueObject(detailStr);
                    //if( obj != null )
                    list.Add(obj);
                }
            }

            return list;
        }

        private Object GetValueObject(string value)
        {
            if( value.Contains("[\"ta\"]") )
                return new MissionStart(value);

            else if( value.Contains("place") )
                return new Place(value);

            else if( value.Contains("targetMissionID") )
                return new Target(value);

            else if( value.Contains("initiator_object_id") )
                return new Initiator(value);

            return new BasicObject(value);

        }

        public class Target : Initiator
        {
            public string? TargetName { get; }
            public string? TargetMissionID { get; }
            public string? TargetPilotName { get; }
            public string? Target_unit_type { get; }
            public long Target_object_id { get; }
            public int Target_ws_type1 { get; }
            public int Target_coalition { get; }

            public string? Weapon { get; }
            public long Weapon_type { get; }



            public Target(string valueStr) : base(valueStr)
            {
                if( Properties != null && ( Properties.Count > 0 ) )
                {
                    string? result = GetValue("target");
                    if( result != null )
                        TargetName = (string)result;

                    result = GetValue("targetMissionID");
                    if( result != null )
                        TargetMissionID = (string)result;

                    result = GetValue("targetPilotName");
                    if( result != null )
                        TargetPilotName = (string)result;

                    result = GetValue("target_coalition");
                    if( result != null )
                        Target_coalition = ToInt(result);

                    result = GetValue("target_object_id");
                    if( result != null )
                        Target_object_id = ToLong(result);

                    result = GetValue("target_unit_type");
                    if( result != null )
                        Target_unit_type = (string)result;

                    result = GetValue("target_ws_type1");
                    if( result != null )
                        Target_ws_type1 = ToInt(result);

                    result = GetValue("weapon");
                    if( result != null )
                        Weapon = (string)result;

                    result = GetValue("weapon_type");
                    if( result != null )
                        Weapon_type = ToLong(result);

                }
            }
        }
        public class MissionStart : BasicObject
        {
            public int TA { get; }

            public MissionStart(string valueStr) : base(valueStr)
            {
                if( Properties != null && ( Properties.Count > 0 ) )
                {
                    string? result = GetValue("ta");
                    if( result != null )
                        TA = ToInt(result);
                }
            }
        }
        public class Place : Initiator
        {
            public string? place { get; }

            public Place(string valueStr) : base(valueStr)
            {
                if( Properties != null && ( Properties.Count > 0 ) )
                {
                    string? result = GetValue("place");
                    if( result != null )
                        place = (string)result;
                }
            }
        }

        public class Initiator : BasicObject
        {
            public string? initiatorMissionID { get; }
            public string? initiatorPilotName { get; }
            public string? initiator_unit_type { get; }
            public long initiator_object_id { get; }
            public int initiator_ws_type1 { get; }
            public int initiator_coalition { get; }

            public Initiator(string valueStr) : base(valueStr)
            {
                if( Properties != null && ( Properties.Count > 0 ) )
                {
                    string? result = GetValue("initiatorMissionID");
                    if( result != null )
                        initiatorMissionID = (string)result;

                    result = GetValue("initiatorPilotName");
                    if( result != null )
                        initiatorPilotName = (string)result;

                    result = GetValue("initiator_coalition");
                    if( result != null )
                        initiator_coalition = ToInt(result);

                    result = GetValue("initiator_object_id");
                    if( result != null )
                        initiator_object_id = ToLong(result);

                    result = GetValue("initiator_unit_type");
                    if( result != null )
                        initiator_unit_type = (string)result;

                    result = GetValue("initiator_ws_type1");
                    if( result != null )
                        initiator_ws_type1 = ToInt(result);

                }
            }

        }

        public class BasicObject
        {
            public int Event_id { get; }
            public int Linked_event_id { get; }
            public float T { get; }
            public string? Type { get; }
            public string? Comment { get; }

            //public DateTime Time { get; }

            protected Dictionary<string, string> Properties { get; } = [];

            public BasicObject(string valueStr)
            {
                string pattern = @"\[""(\w+)""]\s*=\s*([""]?.*[""]?)[,]";
                MatchCollection matches = Regex.Matches(valueStr, pattern, RegexOptions.Multiline);
                if( matches.Count > 0 )
                {
                    foreach( Match match in matches )
                    {
                        if( match.Groups.Count == 3 )
                        {
                            //Logger.Log($"{match.Groups[1].Value}\t{match.Groups[2].Value}");

                            string name = match.Groups[1].Value;
                            string value = match.Groups[2].Value;

                            Properties.Add(name, value);
                        }

                    }

                    string? result = GetValue("event_id");
                    if( result != null )
                        Event_id = ToInt(result);

                    result = GetValue("t");
                    if( result != null )
                        T = ToFloat(result);

                    result = GetValue("type");
                    if( result != null )
                        Type = (string)result;

                    result = GetValue("linked_event_id");
                    if( result != null )
                        Linked_event_id = ToInt(result);

                    result = GetValue("comment");
                    if( result != null )
                        Comment = (string)result;
                    //
                }
            }

            protected string? GetValue(string v)
            {
                IEnumerable<KeyValuePair<string, string>> found = Properties.Where(x => x.Key != null && x.Key.Equals(v));
                if( found.Any() )
                {
                    KeyValuePair<string, string> result = found.First();

                    return result.Value.Replace("\"", "").ToString();
                }
                return null;

            }

        }

        private void SetValues(string text)
        {
            //string namePattern = @"(\w+)\s*=\s*(""[^""]*""|\d+)";
            string namePattern = @"(\w+)\s*=\s*(""[^""]*""|[-+]?\d*\.?\d+)";

            MatchCollection nameMatches = Regex.Matches(text, namePattern, RegexOptions.Multiline);
            if( nameMatches.Count > 0 )
                foreach( Match match in nameMatches )
                    if( match.Groups.Count == 3 )
                    {
                        //Logger.Log($"{match.Groups[01].Value}\t{match.Groups[2].Value}");
                        AddValue(match.Groups[1].Value, match.Groups[2].Value);
                    }
        }

        private void AddValue(string name, string value)
        {
            if( name == null ) return;
            if( value == null ) return;

            //Logger.Log($"{name}\t{value}");
            //Logger.Log($"{name}\t{value}");

            if( name.Equals("callsign") )
                Callsign = value;

            else if( name.Equals("mission_file_mark") )
                Mission_file_mark = ToLong(value);

            else if( name.Equals("mission_file_path") )
                Mission_file_path = value;

            else if( name.Equals("mission_time") )
                Mission_time = ToFloat(value);

            else if( name.Equals("result") )
                Result = ToInt(value);

        }

        private static float ToFloat(string s)
        {
            float n;
            if( !float.TryParse(s, out n) )
                return 0;
            return n;
        }

        private static int ToInt(string s)
        {
            int n;
            if( !int.TryParse(s, out n) )
                return 0;
            return n;
        }

        private static long ToLong(string s)
        {
            long n;
            if( !long.TryParse(s, out n) )
                return 0;
            return n;
        }

        public class SecondsToTimeConverter
        {
            public static DateTime TimeFromSecondsFromMidnight(float seconds)
            {
                if( seconds < 0 || seconds >= 86400 ) // 86400 is the number of seconds in a day.
                {
                    throw new ArgumentOutOfRangeException("seconds", "Seconds must be between 0 and 86399.");
                }

                TimeSpan timeOfDay = TimeSpan.FromSeconds(seconds);
                DateTime midnight = DateTime.Today; // Get today's date at midnight

                DateTime resultTime = midnight + timeOfDay;

                return resultTime;
            }

            internal static DateTime CalcTime(float totalMinutes)
            {
                try
                {
                    //float totalMinutes = 836.928f;
                    float secondsSinceMidnight = (int)( totalMinutes * 60f );

                    DateTime calculatedTime = SecondsToTimeConverter.TimeFromSecondsFromMidnight(secondsSinceMidnight);

                    DateTime utcDateTime = DateTime.SpecifyKind(calculatedTime, DateTimeKind.Utc).Date;

                    utcDateTime = utcDateTime.AddSeconds(secondsSinceMidnight);

                    TimeZoneInfo pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                    if( pacificZone.IsDaylightSavingTime(utcDateTime) )
                        utcDateTime = utcDateTime.AddHours(-1);
                    DateTime pacificDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, pacificZone);

                    Logger.Log($"PST DateTime: {pacificDateTime}");
                    return pacificDateTime;

                }
                catch( Exception )
                {
                    //throw ex;
                }

                throw new ArgumentOutOfRangeException(nameof(CalcTime));
            }
        }


        private const string missionStart = "mission start";
        private const string LANDINGQUALITYMARK = "landing quality mark";
        internal List<Object>? GetLSOGrades()
        {
            IEnumerable<KeyValuePair<string, List<Object>>> found = Properties.Where(x => x.Key != null && x.Key.Equals("events"));
            if( found.Any() )
            {
                List<Object> list = [];
                KeyValuePair<string, List<Object>> events = found.First();

                //for( int i = 0; i < events.Value.Count; i++ )
                //{ }

                for( int i = 0; i < events.Value.Count; i++ )
                {
                    Place? place = events.Value[i] as Place;
                    if( place != null && place.Comment != null && place.Type != null && place.Type.Equals(LANDINGQUALITYMARK) )
                        list.Add(place);

                }

                return list;

            }

            return null;
        }
    }

}