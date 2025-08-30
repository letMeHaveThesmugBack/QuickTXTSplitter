using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace QuickTXTSplitter
{
    /// <summary>
    /// Provides standardized error handling.
    /// </summary>
    internal static class ErrorHandler
    {
        /// <summary>
        /// Writes a standardized error message to the console for various types of <see cref="Exception"/>s, and exits the program.
        /// Writes a line describing the type of error and what operation threw the <see cref="Exception"/>, followed by the <see cref="Exception"/> message.
        /// All lines are written in <see cref="ConsoleColor.Red"/>.
        /// </summary>
        /// <param name="operation">A description of what operation was being performed when the <see cref="Exception"/> was thrown.</param>
        /// <param name="ex">The thrown <see cref="Exception"/>.</param>
        internal static void WriteStandardizedErrorMessageAndExit(string operation, Exception ex)
        {
            StringBuilder messageBuilder = ex switch
            {
                ArgumentException or ArgumentNullException or ArgumentOutOfRangeException => new("An invalid argument was passed to a function while "),
                DirectoryNotFoundException => new("Could not locate a directory while "),
                PathTooLongException => new("A filepath was specified that was too long while "),
                IOException => new("An input/output error occurred while "),
                SecurityException => new("A security exception occurred while "),
                NotSupportedException => new("An unsupported operation was attempted while "),
                RegexMatchTimeoutException => new("A regular expression match timeout occurred while "),
                UnauthorizedAccessException => new("An attempt was made to access an unauthorized item while "),
                OverflowException => new("An overflow occurred while "),
                FormatException => new("An invalid format was used while "),
                _ => new("An unknown error occurred while "),
            };

            messageBuilder.Append(operation).AppendLine(". Exiting.");

            ConsoleColor originalColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(messageBuilder.ToString());
            Console.ForegroundColor = originalColor;

            Environment.Exit(1);
        }
    }
}