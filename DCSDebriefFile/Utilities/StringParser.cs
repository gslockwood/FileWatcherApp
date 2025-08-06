using Utililites;

namespace DCSDebriefFile.Utilities
{

    public class StringParser
    {
        public static List<string> ParseStringIncrementally(string input)
        {
            List<string> list = [];
            if( string.IsNullOrEmpty(input) )
            {
                Logger.Log("Input string cannot be null or empty.");
                return list;
            }

            List<string> results = new List<string>();

            // Parse by 2-letter groupings
            //Logger.Log("--- 2-letter groupings ---");
            for( int i = 0; i <= input.Length - 2; i++ )
            {
                string sub = input.Substring(i, 2);
                results.Add(sub);
                list.Add(sub);
            }

            // Parse by 3-letter groupings
            //Logger.Log("\n--- 3-letter groupings ---");
            for( int i = 0; i <= input.Length - 3; i++ )
            {
                string sub = input.Substring(i, 3);
                results.Add(sub);
                list.Add(sub);
            }

            // Parse by 4-letter groupings
            //Logger.Log("\n--- 4-letter groupings ---");
            for( int i = 0; i <= input.Length - 4; i++ )
            {
                string sub = input.Substring(i, 4);
                results.Add(sub);
                list.Add(sub);
            }

            // If you wanted all results in a single list:
            // Logger.Log("\n--- All results in a single list ---");
            // foreach (string result in results)
            // {
            //     Logger.Log(result);
            // }

            return list;

        }

        //public static void Main(string[] args)
        //{
        //    string myString = "WOAFUTL";
        //    ParseStringIncrementally(myString);

        //    Logger.Log("\n--- Example with a shorter string (won't show 4-letter) ---");
        //    ParseStringIncrementally("ABC");
        //}
    }
}
