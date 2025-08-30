using System.Security;
using System.Text;
using static QuickTXTSplitter.ErrorHandler;

namespace QuickTXTSplitter
{
    /// <summary>
    /// Builds the destination directory tree structure.
    /// </summary>
    internal static class DirectoryBuilder
    {
        internal const string ForeignSubdirectory = "[!NonLatin]";
        internal const string NoMatchSubdirectory = "[!NoMatch]";
        const string subdirectoryCharacters = "0123456789abcdefghijklmnopqrstuvwxyz$";
        const string operation = "attempting to build the destination directory tree structure";

        /// <summary>
        /// Builds the destination directory tree structure.
        /// </summary>
        /// <param name="destinationInfo">Directory into which to write.</param>
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

            catch (Exception ex)
            {
                WriteStandardizedErrorMessageAndExit(operation, ex);
            }
        }
    }
}
