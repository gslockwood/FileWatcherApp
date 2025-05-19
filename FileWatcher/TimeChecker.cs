namespace FileWatcher
{
    public class TimeChecker
    {
        private static DateTime lastExecutionTime = DateTime.MinValue;
        //private static  TimeSpan threshold = TimeSpan.FromSeconds(30);

        //static
        public bool IsLessThanSecondsSinceLastExecution(int seconds)
        {
            TimeSpan threshold = TimeSpan.FromSeconds(seconds);

            DateTime now = DateTime.Now;
            TimeSpan difference = now - lastExecutionTime;

            if( difference < threshold )
                return true;

            else
            {
                lastExecutionTime = now;
                return false;
            }
        }
    }

}
