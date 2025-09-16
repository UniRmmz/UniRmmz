using System.Collections.Generic;
using System.Linq;

namespace UniRmmz
{
    public class RmmzJavascriptCode
    {
        public List<string> Lines = new();

        public string GenerateCode()
        {
            var nl = System.Environment.NewLine;
            return nl + string.Join(nl, Lines.Select(line => $"\t\t\t\t{EscapeString(line)}"));
        }
        
        public static string NormalizeKey(string key)
        {
            return key.Replace("\r\n", "\n");// 改行コードを\nにそろえておく
        }
        
        public string GenerateKey()
        {
            var nl = "\n";// 環境によらず、\nで統一
            return nl + string.Join(nl, Lines.Select(line => $"\t\t\t\t{line}"));
        }

        public void AddLine(string line)
        {
            Lines.Add(line);
        }

        public bool IsEmpty() => !Lines.Any();

        
        private static string EscapeString(string input)
        {
            return input?.Replace("\"", "\"\"") ?? "";
        }
    }
}