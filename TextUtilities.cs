using System.Text;

namespace QuickTXTSplitter
{
    internal static class TextUtilities
    {
        internal enum NormalizationCode
        {
            Foreign = '%',
            Symbol = '$' // todo: these and the [!...] things should be defined somewhere consistent and referenced instead of being hardcoded
        }

        internal static char NormalizeToSubdirectoryCharacter(Rune r)
        {
            if (r.IsBmp)
            {
                r = r.ToString().Normalize(NormalizationForm.FormKD).EnumerateRunes().First();

                if (r.IsAscii && char.ToLowerInvariant((char)r.Value) is char ch && char.IsAsciiLetterOrDigit(ch)) return ch;

                if (Rune.IsLetterOrDigit(r)) return (char)NormalizationCode.Foreign;
            }

            return (char)NormalizationCode.Symbol;
        }

        internal static bool IsForeign(this char c) => c == (char)NormalizationCode.Foreign;
        internal static bool IsSymbol(this char c) => c == (char)NormalizationCode.Symbol;

        internal static string Sanitize(this string name)
        {
            StringBuilder builder = new();

            foreach (char character in name)
            {
                switch (character)
                {
                    case '<':
                        builder.Append("[$LessThan]");
                        continue;
                    case '>':
                        builder.Append("[$GreaterThan]");
                        continue;
                    case ':':
                        builder.Append("[$Colon]");
                        continue;
                    case '"':
                        builder.Append("[$QuotationMark]");
                        continue;
                    case '/':
                        builder.Append("[$ForwardSlash]");
                        continue;
                    case '\\':
                        builder.Append("[$Backslash]");
                        continue;
                    case '|':
                        builder.Append("[$Pipe]");
                        continue;
                    case '?':
                        builder.Append("[$QuestionMark]");
                        continue;
                    case '*':
                        builder.Append("[$Asterisk]");
                        continue;
                    default:
                        break;
                }

                if (char.IsControl(character)) continue;

                builder.Append(character);
            }

            return builder.ToString();
        }
    }
}
