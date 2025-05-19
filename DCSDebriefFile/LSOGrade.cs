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

        public string WireCaught { get; set; } = "UNK";
        public string Pilot { get; set; } = "UNK";
        public string Carrier { get; set; } = "UNK";

        // for serialization
        public LSOGrade() { }
        public LSOGrade(string grade, string errorStr, IList<LSOGradeError>? errors, string wireCaught)
        {
            Grade = grade;
            ErrorStr = errorStr;
            Errors = errors;
            if( wireCaught.Equals("UNK") )
                WireCaught = wireCaught;
            else
                WireCaught = $"Wire # {wireCaught} caught";
        }

        public class LSOGradeError
        {
            public string? Error { get; }
            public string? Translation { get; }
            public LSOGradeError(string error, string? translation)
            {
                Error = error;
                Translation = translation;
            }

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
            StringBuilder sb = new StringBuilder();

            if( this.Pilot != null ) sb.AppendLine(Pilot);
            if( this.Carrier != null ) sb.AppendLine(Carrier);
            sb.AppendLine(this.DateTime.ToString("G"));

            if( this.Grade != null ) sb.AppendLine(Grade);
            if( this.ErrorStr != null ) sb.AppendLine(ErrorStr);
            if( this.WireCaught != null ) sb.AppendLine(WireCaught);
            if( this.Errors != null )
            {
                foreach( LSOGradeError error in Errors )
                    sb.AppendLine("\t" + error.ToString());
            }

            return sb.ToString();
        }

    }
}
