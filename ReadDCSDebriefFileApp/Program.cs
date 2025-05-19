using Utililites;

namespace ReadDCSDebriefFileApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string logFilePath = @"C:\Users\george s. lockwood\Saved Games\DCS\Missions\FA-18C\F18 Case Recoveries\debriefing.log";

            DCSDebriefFile.ReadReader readReader = new DCSDebriefFile.ReadReader(@".\LSOGRADETABLE.json");
            readReader.ReadCompleted += (list) =>
            {
                if( list == null ) return;

                foreach( DCSDebriefFile.LSOGradeInfo lsoGradeInfo in list )
                {
                    Logger.Log(lsoGradeInfo.LsoGradesComment);
                    Logger.Log(lsoGradeInfo.Translation);
                }

            };
            readReader.ReadFile(logFilePath);

        }
    }
}
//namespace Utililites
//{
//    public class Logger
//    {
//        public static void Log(object message)
//        {
//#if DEBUG
//            System.Diagnostics.Debug.WriteLine(message);
//#else
//            Console.WriteLine(message);
//#endif
//        }
//    }
//}