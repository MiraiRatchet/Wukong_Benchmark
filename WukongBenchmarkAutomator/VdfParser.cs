using System;
using System.Collections.Generic;
using System.Text;

namespace WukongBenchmarkAutomator
{
    internal static class VdfParser
    {
        public static Dictionary<string, object> Parse(string text)
        {
            int pos = 0;
            SkipWhitespace(text, ref pos);

            // 1. Читаем имя корневого ключа ("libraryfolders" или "AppState")
            string rootKey = ParseQuotedString(text, ref pos);
            SkipWhitespace(text, ref pos);

            // 2. Читаем блок с содержимым
            Dictionary<string, object> rootBody = ParseBlock(text, ref pos);

            // 3. Возвращаем словарь, в котором один ключ: rootKey → rootBody
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            result[rootKey] = rootBody;
            return result;
        }

        private static Dictionary<string, object> ParseBlock(string text, ref int pos)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            SkipWhitespace(text, ref pos);
            Expect(text, ref pos, '{');
            SkipWhitespace(text, ref pos);

            while (pos < text.Length && text[pos] != '}')
            {
                string key = ParseQuotedString(text, ref pos);
                SkipWhitespace(text, ref pos);

                if (pos < text.Length && text[pos] == '{')
                {
                    dict[key] = ParseBlock(text, ref pos);
                }
                else
                {
                    string value = ParseQuotedString(text, ref pos);
                    dict[key] = value;
                }

                SkipWhitespace(text, ref pos);
            }

            Expect(text, ref pos, '}');
            return dict;
        }

        private static string ParseQuotedString(string text, ref int pos)
        {
            Expect(text, ref pos, '"');
            var sb = new StringBuilder();

            while (pos < text.Length && text[pos] != '"')
            {
                if (text[pos] == '\\' && pos + 1 < text.Length)
                {
                    pos++;
                    char esc = text[pos];
                    if (esc == '\\') sb.Append('\\');
                    else if (esc == '"') sb.Append('"');
                    else if (esc == 'n') sb.Append('\n');
                    else if (esc == 't') sb.Append('\t');
                    else if (esc == 'r') sb.Append('\r');
                    else sb.Append(esc);
                }
                else
                {
                    sb.Append(text[pos]);
                }
                pos++;
            }

            Expect(text, ref pos, '"');
            return sb.ToString();
        }

        private static void SkipWhitespace(string text, ref int pos)
        {
            while (pos < text.Length && char.IsWhiteSpace(text[pos]))
                pos++;
        }

        private static void Expect(string text, ref int pos, char c)
        {
            if (pos >= text.Length || text[pos] != c)
            {
                string found = pos < text.Length
                    ? "'" + text[pos] + "'"
                    : "EOF (конец файла)";
                throw new FormatException(
                    "Ожидался символ '" + c + "' в позиции " + pos +
                    ", но получен " + found +
                    ". Первые 80 символов файла: \"" +
                    (text.Length > 80 ? text.Substring(0, 80) : text) + "\"");
            }
            pos++;
        }
    }
}