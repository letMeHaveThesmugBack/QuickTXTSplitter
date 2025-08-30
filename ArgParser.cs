using System.Text.RegularExpressions;
using static QuickTXTSplitter.ErrorHandler;

namespace QuickTXTSplitter
{
    /// <summary>
    /// Parser for command-line arguments.
    /// </summary>
    internal static class ArgParser
    {
        const string operation = "parse command-line arguments";


        /// <summary>
        /// Represents an ArgParser state resulting from the last argument parsed.
        /// </summary>
        internal enum ParseState
        {
            None,
            Source,
            Destination,
            Regex,
            CapturingGroup,
            Prefix
        }

        /// <summary>
        /// Container for values parsed from command-line arguments.
        /// </summary>
        /// <param name="Source">The directory containing the source file(s). Defaults to the current working directory.</param>
        /// <param name="Destination">The directory into which the split files will be written. Defaults to the current working directory.</param>
        /// <param name="Regex">A regular expression which, upon successfully matching to a line of a source file, determines where to produce a split. Required.</param>
        /// <param name="CapturingGroup">If <paramref name="Regex"/> matches, this is the index of the capturing group whose value shall be used as the title of the resulting split file. Defaults to <c>0</c>.</param>
        /// <param name="Prefix"><see langword="string"/> to prepend to the split file name.</param>
        internal record ParsedArgs(DirectoryInfo Source, DirectoryInfo Destination, Regex Regex, int CapturingGroup, string Prefix);

        /// <summary>
        /// Parses command-line arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>A <see cref="ParsedArgs"/> containing the parsed arguments.</returns>
        internal static ParsedArgs Parse(string[] args)
        {
            try
            {
                ParseState state = ParseState.None;

                DirectoryInfo? source = null;
                DirectoryInfo? destination = null;
                Regex? regex = null;
                int capturingGroup = 0;
                string prefix = string.Empty;

                foreach (string arg in args)
                {
                    if (arg.StartsWith('-'))
                    {
                        string option = arg.ToLower()[1..];

                        state = option switch
                        {
                            "-source" or "s" => ParseState.Source,
                            "-dest" or "d" => ParseState.Destination,
                            "-regex" or "r" => ParseState.Regex,
                            "-capturingGroup" or "c" => ParseState.CapturingGroup,
                            "-prefix" or "p" => ParseState.Prefix,
                            _ => ParseState.None,
                        };
                    }

                    else
                    {
                        InterpretOption(arg); // Workbook: inline

                        state = ParseState.None;
                    }
                }

                void InterpretOption(string arg)
                {
                    switch (state)
                    {
                        case ParseState.Source:
                            source = new(Path.GetFullPath(arg));
                            break;
                        case ParseState.Destination:
                            destination = new(Path.GetFullPath(arg));
                            break;
                        case ParseState.Regex:
                            regex = new Regex(arg);
                            break;
                        case ParseState.CapturingGroup:
                            if (!int.TryParse(arg, out capturingGroup)) capturingGroup = 0; // TryParse normally sets out to 0 if it fails, but this just ensures it definitely gives 0 on failure in case anything strange happens.
                            break;
                        case ParseState.Prefix:
                            prefix = arg;
                            break;
                        default:
                            throw new ArgumentException($"""Unexpected argument "{arg}".""");
                    }
                }

                return regex is null
                    ? throw new ArgumentException("""The "--regex (-r)" argument is required.""")
                    : new(source ?? new(Directory.GetCurrentDirectory()), destination ?? new(Directory.GetCurrentDirectory()), regex, capturingGroup, prefix);
            }

            catch (Exception ex)
            {
                WriteStandardizedErrorMessageAndExit(operation, ex);
                throw; // This exists to satisfy the compiler that complains that not all paths return a value, and cannot ever be reached because the above function exits the program.
            }
        }
    }
}