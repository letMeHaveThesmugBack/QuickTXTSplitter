using System.Security;
using System.Text;

namespace QuickTXTSplitter
{
    // 08/28/2025: This is verified working so far.
    internal static class DirectoryBuilder
    {
        internal const string ForeignSubdirectory = "[!NonLatin]";
        internal const string NoMatchSubdirectory = "[!NoMatch]";
        const string subdirectoryCharacters = "0123456789abcdefghijklmnopqrstuvwxyz$";

        internal static void BuildDirectories(DirectoryInfo destinationInfo)
        {
            try
            {
                StringBuilder builder = new();

                destinationInfo.CreateSubdirectory(ForeignSubdirectory);
                destinationInfo.CreateSubdirectory(NoMatchSubdirectory);

                DirectoryInfo firstSubdirectoryInfo;

                foreach (char first in subdirectoryCharacters)
                {
                    builder.Append(first);

                    firstSubdirectoryInfo = destinationInfo.CreateSubdirectory(builder.ToString());

                    foreach (char second in subdirectoryCharacters)
                    {
                        builder.Append(second);
                        firstSubdirectoryInfo.CreateSubdirectory(builder.ToString());
                        builder.Remove(builder.Length - 1, 1);
                    }

                    builder.Clear();
                }
            }

            catch (Exception ex) when (ex is ArgumentException or ArgumentNullException)
            {
                WriteStandardizedErrorMessageAndExit("An invalid argument was passed to a function", ex);
            }

            catch (DirectoryNotFoundException ex)
            {
                WriteStandardizedErrorMessageAndExit("Could not locate a directory", ex);
            }

            catch (PathTooLongException ex)
            {
                WriteStandardizedErrorMessageAndExit("A filepath was specified that was too long", ex);
            }

            catch (IOException ex)
            {
                WriteStandardizedErrorMessageAndExit("An input/output error occurred", ex);
            }

            catch (SecurityException ex)
            {
                WriteStandardizedErrorMessageAndExit("A security exception occurred", ex);
            }

            catch (NotSupportedException ex)
            {
                WriteStandardizedErrorMessageAndExit("An unsupported operation was attempted", ex);
            }

            catch (ArgumentOutOfRangeException ex)
            {
                WriteStandardizedErrorMessageAndExit("An out-of-range argument was passed to a function", ex);
            }
        }

        static void WriteStandardizedErrorMessageAndExit(string message, Exception ex)
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{message} while attempting to create the destination directory tree structure. Exiting.");
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = originalColor;

            Environment.Exit(1);
        }
    }
}
