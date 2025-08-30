using System.Text.RegularExpressions;

namespace QuickTXTSplitter
{
    internal static class ArgParser
    /// <summary>
    /// Provides command-line argument parsing functionality for QuickTXTSplitter.
    /// </summary>
    {
        internal enum ParseState
        {
            None,
            Source,
            Destination,
            Regex,
            CapturingGroup,
            Prefix
        }

        internal record ParsedArgs(DirectoryInfo Source, DirectoryInfo Destination, Regex Regex, int CapturingGroup, string Prefix);

        internal static ParsedArgs Parse(string[] args)
    /// <summary>
    /// Parses the command-line arguments and returns the parsed result.
    /// </summary>
    /// <param name="args">The array of command-line arguments.</param>
    /// <returns>A ParsedArgs record containing the parsed arguments.</returns
        {
            string currentArg;

            ParseState state = ParseState.None;

            DirectoryInfo? source = null;
            DirectoryInfo? destination = null;
            Regex? regex = null;
            int capturingGroup = 0;
            string prefix = string.Empty;

            for (int i = 0; i < args.Length; i++)
            {
                currentArg = args[i];

                if (currentArg.StartsWith('-'))
                {
                    string option = args[i].ToLower()[1..];

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
                    InterpretOption();

                    state = ParseState.None;
                }
            }

            void InterpretOption()
            {
                switch (state)
                {
                    case ParseState.Source:
                        source = new(Path.GetFullPath(currentArg));
                        break;
                    case ParseState.Destination:
                        destination = new(Path.GetFullPath(currentArg));
                        break;
                    case ParseState.Regex:
                        regex = new Regex(currentArg);
                        break;
                    case ParseState.CapturingGroup:
                        if (!int.TryParse(currentArg, out capturingGroup)) capturingGroup = 0; // TryParse normally sets out to 0 if it fails, but this just ensures it definitely gives 0 on failure in case anything strange happens.
                        break;
                    case ParseState.Prefix:
                        prefix = currentArg;
                        break;
                    default:
                        // do something
                        break;
                }
            }

            if (regex is null) throw new NotImplementedException("say something about requiring the regex");

            return new(source ?? new(Directory.GetCurrentDirectory()), destination ?? new(Directory.GetCurrentDirectory()), regex, capturingGroup, prefix);
        }
    }
}