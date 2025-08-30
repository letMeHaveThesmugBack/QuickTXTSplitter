using System.Text;

namespace QuickTXTSplitter
{
    /// <summary>
    /// Utilities for interpreting and transforming text.
    /// </summary>
    internal static class TextUtilities
    {
        /// <summary>
        /// Codes representing inferred properties of a <see cref="Rune"/>.
        /// </summary>
        internal enum NormalizationCode
        {
            /// <summary>
            /// Represents a <see cref="Rune"/> which cannot be matched to any ASCII Latin alphanumeric lowercase <see langword="char"/>, but is recognized as a letter or digit
            /// and is within the Unicode BMP encoding range.
            /// </summary>
            Foreign = '%',

            /// <summary>
            /// Represents a <see cref="Rune"/> which is any of the following:
            /// <list type="bullet">
            /// <item>Cannot be matched to any ASCII Latin alphanumeric lowercase <see langword="char"/>.</item>
            /// <item>Is not recognized as a letter or digit.</item>
            /// <item>Is not within the Unicode BMP encoding range.</item>
            /// </list>
            /// </summary>
            Symbol = '$' // todo: these and the [!...] things should be defined somewhere consistent and referenced instead of being hardcoded
        }

        /// <summary>
        /// Normalizes and converts a <see cref="Rune"/> to the corresponding ASCII Latin alphanumeric lowercase <see langword="char"/>,
        /// or a <see langword="char"/> representing a <see cref="NormalizationCode"/>  for those <see cref="Rune"/>s which cannot otherwise be matched.
        /// </summary>
        /// <param name="r">The <see cref="Rune"/> to convert.</param>
        /// <returns>The resulting <see langword="char"/>.</returns>
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

        /// <summary>
        /// Determines if a <see langword="char"/> represents <see cref="NormalizationCode.Foreign"/>.
        /// </summary>
        /// <param name="c">The <see langword="char"/> to test.</param>
        /// <returns></returns>
        internal static bool IsForeign(this char c) => c == (char)NormalizationCode.Foreign;

        /// <summary>
        /// Determines if a <see langword="char"/> represents <see cref="NormalizationCode.Symbol"/>.
        /// </summary>
        /// <param name="c">The <see langword="char"/> to test.</param>
        /// <returns></returns>
        internal static bool IsSymbol(this char c) => c == (char)NormalizationCode.Symbol;

        /// <summary>
        /// Removes NTFS-invalid symbols from text.
        /// </summary>
        /// <param name="name">Text to sanitize.</param>
        /// <returns>Sanitized text.</returns>
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
