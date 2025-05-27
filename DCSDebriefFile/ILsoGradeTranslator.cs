using static DCSDebriefFile.LSOGrade;

namespace DCSDebriefFile
{
    public interface ILsoGradeTranslator : IDisposable
    {
        event LsoGradeTranslator.RefreshEventHandler? Refresh;

        LSOGrade? GetLSOGrade(string lsoGrade);
        string? GetGradeTranslation(string lsoGrade);

        IList<LSOGradeError>? GetErrors(string errorStr);
        List<LSOGrade>? GetLSOGrades(string dcsBriefingLog);
    }
}