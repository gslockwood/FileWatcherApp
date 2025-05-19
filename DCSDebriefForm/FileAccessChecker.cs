using Utililites;

namespace DCSDebriefForm
{
    public class FileAccessChecker
    {
        public static bool IsFileAccessible(string filePath, int timeoutMilliseconds = -1, int retryIntervalMilliseconds = 100)
        {
            DateTime startTime = DateTime.Now;

            while( timeoutMilliseconds == -1 || ( DateTime.Now - startTime ).TotalMilliseconds < timeoutMilliseconds )
            {
                try
                {
                    // Attempt to open the file with read-only access
                    using( FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None) )
                    {
                        // If we get here, the file is accessible
                        return true;
                    }
                }
                catch( IOException ex )
                {
                    // Check if the exception is due to the file being used by another process
                    if( IsFileInUseException(ex) )
                    {
                        // Wait for the retry interval
                        Thread.Sleep(retryIntervalMilliseconds);
                    }
                    else
                    {
                        // It's some other IO exception, the file might not be accessible for other reasons
                        return false;
                    }
                }
                catch( Exception )
                {
                    // Handle other potential exceptions
                    return false;
                }
            }

            // Timeout occurred, the file was not accessible within the specified time
            return false;
        }

        private static bool IsFileInUseException(IOException ex)
        {
            int errorCode = System.Runtime.InteropServices.Marshal.GetHRForException(ex) & 0xFFFF;
            // Error code 32 (0x20) corresponds to "The process cannot access the file because it is being used by another process."
            return errorCode == 32;
        }

        public static void WaitForFileAccess(string filePath, int timeoutMilliseconds = -1, int retryIntervalMilliseconds = 500)
        {
            DateTime startTime = DateTime.Now;

            while( !IsFileAccessible(filePath) )
            {
                if( timeoutMilliseconds != -1 && ( DateTime.Now - startTime ).TotalMilliseconds >= timeoutMilliseconds )
                {
                    throw new TimeoutException($"Timeout waiting for file '{filePath}' to become accessible.");
                }
                Thread.Sleep(retryIntervalMilliseconds);
            }

            Logger.Log($"File '{filePath}' is now accessible.");
        }

        public static void MainX(string[] args)
        {
            string filePath = "test.txt"; // Replace with your file path

            // Create a dummy file for testing
            File.WriteAllText(filePath, "This is a test file.");

            // Simulate another process holding the file briefly
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    using( FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.None) )
                    {
                        Logger.Log("Simulating another process holding the file...");
                        Thread.Sleep(3000); // Hold for 3 seconds
                        Logger.Log("Simulating another process releasing the file...");
                    }
                }
                catch( Exception ex )
                {
                    Logger.Log($"Error in simulating process: {ex.Message}");
                }
            });

            Logger.Log($"Checking if '{filePath}' is accessible...");
            if( IsFileAccessible(filePath) )
            {
                Logger.Log($"'{filePath}' is currently accessible.");
            }
            else
            {
                Logger.Log($"'{filePath}' is NOT currently accessible (likely in use).");

                try
                {
                    Logger.Log($"Waiting for '{filePath}' to become accessible...");
                    WaitForFileAccess(filePath, 10000); // Wait up to 10 seconds
                                                        // Now you can work with the file
                    try
                    {
                        string content = File.ReadAllText(filePath);
                        Logger.Log($"Successfully read file content: {content}");
                        File.Delete(filePath); // Clean up the test file
                    }
                    catch( IOException ex )
                    {
                        Logger.Log($"Error reading file after it became accessible: {ex.Message}");
                    }
                }
                catch( TimeoutException ex )
                {
                    Logger.Log(ex.Message);
                }
            }
        }
    }
}

