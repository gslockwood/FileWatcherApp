using DCSDebriefFile;

namespace DCSDebriefForm.Model
{
    public class Settings
    {
        public string? DcsBriefingLogFileName { get; set; }
        //public string? DcsBriefingLogDirectoryName { get; set; }
        public IDictionary<DateTime, LSOGrade>? Data { get; set; }
    }


}
