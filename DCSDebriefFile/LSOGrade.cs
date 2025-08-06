using System.Text;

namespace DCSDebriefFile
{
    public class LSOGrade
    {
        public string Grade { get; set; } = "UNK";
        public string ErrorStr { get; set; } = "UNK";

        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime DateTime { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public IList<LSOGradeError>? Errors { get; set; }

        public short Wire { get; set; } = 0;
        public string Pilot { get; set; } = "UNK";
        public string UnitType { get; set; } = "UNK";
        public string Carrier { get; set; } = "UNK";

        // for serialization
        public LSOGrade() { }
        public LSOGrade(string grade, string errorStr, IList<LSOGradeError>? errors, short wire)
        {
            Grade = grade;
            ErrorStr = errorStr;
            Errors = errors;
            Wire = wire;

            //if( wireCaught.Equals("UNK") )
            //    WireCaught = wire;
            //else
            //    WireCaught = $"Wire# {wireCaught}";
            ////WireCaught = $"Wire # {wireCaught} caught";
        }

        public class LSOGradeError(string error, string? translation)
        {
            public string? Error { get; } = error;
            public string? Translation { get; } = translation;

            public override string ToString()
            {
                return $"Error:{Error} Translation:{Translation}";

                //StringBuilder sb = new StringBuilder();
                //if( this.Error != null ) sb.AppendLine(Error);
                //if( this.Translation != null ) sb.AppendLine(Translation);
                //return sb.ToString();
            }

        }

        public override string ToString()
        {
            StringBuilder sb = new();

            if( this.Pilot != null ) sb.AppendLine(Pilot);
            if( this.UnitType != null ) sb.AppendLine(UnitType);
            if( this.Carrier != null ) sb.AppendLine(Carrier);
            sb.AppendLine(this.DateTime.ToString("G"));

            if( this.Grade != null ) sb.AppendLine(Grade);
            if( this.ErrorStr != null ) sb.AppendLine(ErrorStr);
            //if( this.WireCaught != null ) sb.AppendLine(WireCaught);
            sb.AppendLine($"Wire#{Wire.ToString()}");
            if( this.Errors != null )
            {
                foreach( LSOGradeError error in Errors )
                    sb.AppendLine("\t" + error.ToString());
            }

            return sb.ToString();
        }

    }
}
