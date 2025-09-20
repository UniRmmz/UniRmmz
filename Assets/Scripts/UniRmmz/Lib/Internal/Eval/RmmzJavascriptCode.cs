using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRmmz
{
    public class RmmzJavascriptCode : IEquatable<RmmzJavascriptCode>
    {
        public List<string> Lines = new();

        public string GenerateCode()
        {
            if (Lines.Count == 1)
            {
                return EscapeString(Lines[0]);
            }
            else
            {
                var nl = System.Environment.NewLine;
                return nl + string.Join(nl, Lines.Select(line => $"\t\t\t\t{EscapeString(line)}"));    
            }
        }
        
        public static string NormalizeKey(string key)
        {
            return key.Replace("\r\n", "\n");// 改行コードを\nにそろえておく
        }
        
        public string GenerateKey()
        {
            if (Lines.Count == 1)
            {
                return Lines[0];
            }
            else
            {
                var nl = "\n";// 環境によらず、\nで統一
                return nl + string.Join(nl, Lines.Select(line => $"\t\t\t\t{line}"));    
            }
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
        
        public bool Equals(RmmzJavascriptCode other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return NormalizeKey(GenerateKey()) == NormalizeKey(other.GenerateKey());
        }
        
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            
            return Equals((RmmzJavascriptCode)obj);
        }

        public override int GetHashCode()
        {
            // 正規化されたキーを使用してハッシュコードを生成
            var normalizedKey = NormalizeKey(GenerateKey());
            return normalizedKey?.GetHashCode() ?? 0;
        }

        public static bool operator ==(RmmzJavascriptCode left, RmmzJavascriptCode right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(RmmzJavascriptCode left, RmmzJavascriptCode right)
        {
            return !(left == right);
        }
    }
}