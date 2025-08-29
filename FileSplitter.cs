using System.Text;
using System.Text.RegularExpressions;
using static QuickTXTSplitter.DirectoryBuilder;
using ConcurrentCollections;
using System.Collections.Immutable;

namespace QuickTXTSplitter
{
    internal static partial class FileSplitter
    {
        static readonly ConcurrentHashSet<string> nonAlphanumericFilenames = new(4, 1_500_000);
        static readonly ImmutableDictionary<char, ConcurrentHashSet<string>> alphanumericFilenames = new Dictionary<char, ConcurrentHashSet<string>>
        {
            {'0', new(4, 200_000) },
            {'1', new(4, 200_000) },
            {'2', new(4, 200_000) },
            {'3', new(4, 200_000) },
            {'4', new(4, 200_000) },
            {'5', new(4, 200_000) },
            {'6', new(4, 200_000) },
            {'7', new(4, 200_000) },
            {'8', new(4, 200_000) },
            {'9', new(4, 200_000) },
            {'a', new(4, 200_000) },
            {'b', new(4, 200_000) },
            {'c', new(4, 200_000) },
            {'d', new(4, 200_000) },
            {'e', new(4, 200_000) },
            {'f', new(4, 200_000) },
            {'g', new(4, 200_000) },
            {'h', new(4, 200_000) },
            {'i', new(4, 200_000) },
            {'j', new(4, 200_000) },
            {'k', new(4, 200_000) },
            {'l', new(4, 200_000) },
            {'m', new(4, 200_000) },
            {'n', new(4, 200_000) },
            {'o', new(4, 200_000) },
            {'p', new(4, 200_000) },
            {'q', new(4, 200_000) },
            {'r', new(4, 200_000) },
            {'s', new(4, 200_000) },
            {'t', new(4, 200_000) },
            {'u', new(4, 200_000) },
            {'v', new(4, 200_000) },
            {'w', new(4, 200_000) },
            {'x', new(4, 200_000) },
            {'y', new(4, 200_000) },
            {'z', new(4, 200_000) },
        }.ToImmutableDictionary();

        internal static void Split(string sourceFilepath, ArgParser.ParsedArgs args)
        {
            DirectoryInfo destinationInfo = args.Destination;
            Regex regex = args.Regex;
            int capturingGroup = args.CapturingGroup;
            string prefix = args.Prefix;

            IEnumerable<string> sourceLines = File.ReadLines(sourceFilepath);

            StreamWriter? splitFileWriter = null;
            string splitFilepath = string.Empty;

            foreach (string line in File.ReadLines(sourceFilepath))
            {
                if (splitFileWriter is null)
                {
                    if (!TrySplitAtLine(line, out splitFilepath)) splitFilepath = Path.Combine(destinationInfo.FullName, NoMatchSubdirectory, $"{prefix}{NoMatchSubdirectory}{Guid.NewGuid()}.txt");
                    splitFileWriter = new(splitFilepath);
                }

                else if (TrySplitAtLine(line, out splitFilepath))
                {
                    splitFileWriter?.Dispose();

                    splitFileWriter = new(splitFilepath);
                }

                splitFileWriter.WriteLine(line);
            }

            splitFileWriter?.Dispose();

            bool TrySplitAtLine(string line, out string filepath)
            {
                string title;

                Match match = regex.Match(line);

                if (match.Success && match.Groups.Count >= capturingGroup + 1)
                {
                    title = match.Groups[capturingGroup].Value.TrimStart();
                    string titleLower = title.ToLower();

                    List<char> chars = [.. from rune in titleLower.EnumerateRunes()
                                           select TextUtilities.NormalizeToSubdirectoryCharacter(rune)];

                    string subdirectory = chars.Count switch
                    {
                        0 => Path.Combine("$", "$$"),
                        1 => !chars[0].IsForeign() ? Path.Combine($"{chars[0]}", $"{chars[0]}{chars[0]}") : ForeignSubdirectory,
                        _ => !chars[0].IsForeign() && !chars[1].IsForeign()
                                                        ? Path.Combine($"{chars[0]}", $"{chars[0]}{chars[1]}")
                                                        : ForeignSubdirectory,
                    };

                    title = title.Sanitize();
                    StringBuilder filenameBuilder = new($"{prefix}{title}.txt");

                    ConcurrentHashSet<string> existingFilenameSet = subdirectory[0] switch
                    {
                        '$' or '[' => nonAlphanumericFilenames,
                        _ => alphanumericFilenames[subdirectory[0]],
                    };

                    while (!existingFilenameSet.Add(filenameBuilder.ToString()))
                    {
                        Regex regex = IncrementalFilenameRegex();
                        Match filenameMatch = regex.Match(filenameBuilder.ToString());
                        int index = filenameMatch.Success ? int.Parse(filenameMatch.Groups[2].Value) + 1 : 1;

                        filenameBuilder.Clear();

                        filenameBuilder.Append(filenameMatch.Groups[1].Value)
                            .Append("[#")
                            .Append(index)
                            .Append("].txt");
                    } // todo: this does not work. it just does empty then [#...]. figure ot why the first capturing group is being lost

                    filepath = Path.Combine(destinationInfo.FullName, subdirectory, filenameBuilder.ToString());
                    return true;
                }

                else
                {
                    filepath = string.Empty;
                    return false;
                }
            }
        }

        [GeneratedRegex("""^(.*)(?=\[#(\d+)\]\.txt$)""")]
        private static partial Regex IncrementalFilenameRegex();
    }
}
