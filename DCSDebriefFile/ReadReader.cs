namespace DCSDebriefFile
{
    public class ReadReader : IDisposable
    {
        public delegate void ReadCompletedEventHandler(List<LSOGrade>? lsoGradesInfo);
        public event ReadCompletedEventHandler? ReadCompleted;
        public event LsoGradeTranslator.RefreshEventHandler? Refresh;

        private string dcsBriefingLog;

        private readonly DCSDebriefFile.ILsoGradeTranslator lsoGradeTranslator;

        //private DCSDebriefingValues? dcsDebriefingValues;

        public ReadReader(string dcsBriefingLog, string lsoGradeTableJson)
        {
            this.dcsBriefingLog = dcsBriefingLog;

            //lsoGradeTranslator = new(lsoGradeTableJson);
            //lsoGradeTranslator = new DCSDebriefFile.LsoGradeTranslator(lsoGradeTableJson);

            lsoGradeTranslator = new DCSDebriefFile.LsoGradeTranslatorLSOGradeFile(lsoGradeTableJson);

            lsoGradeTranslator.Refresh += () =>
            {
                ReadFile(this.dcsBriefingLog);
                Refresh?.Invoke();
            };

            //ReadFile(this.dcsBriefingLog);// testing only

        }

        public void ReadFile(string dcsBriefingLog)
        {
            if( lsoGradeTranslator == null ) throw new NullReferenceException(nameof(lsoGradeTranslator));

            if( File.Exists(dcsBriefingLog) )
            {
                this.dcsBriefingLog = dcsBriefingLog;

                List<LSOGrade>? lsoGrades = lsoGradeTranslator.GetLSOGrades(dcsBriefingLog);

                ReadCompleted?.Invoke(lsoGrades);
                //
            }

        }

        public string? GeTTranslation(string grade)
        {
            return lsoGradeTranslator.Translate(grade);
        }

        public LSOGrade? GetLSOGrade(string grade)
        {
            return lsoGradeTranslator.GetLSOGrade(grade);
        }

        public void Dispose()
        {
            lsoGradeTranslator?.Dispose();
        }

        public IList<LSOGrade.LSOGradeError>? GetErrors(string errorStr)
        {
            return lsoGradeTranslator.GetErrors(errorStr);
        }
    }
}
