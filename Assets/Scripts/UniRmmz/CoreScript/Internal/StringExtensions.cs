namespace UniRmmz
{
    public static class StringExtensions
    {
        public static string RmmzFormat(this string self, object arg0)
        {
            return self.Replace("%1", arg0.ToString());
        }
        
        public static string RmmzFormat(this string self, object arg0, object arg1)
        {
            return self.Replace("%1", arg0.ToString()).Replace("%2", arg1.ToString());
        }
        
        public static string RmmzFormat(this string self, object arg0, object arg1, object arg2)
        {
            return self.Replace("%1", arg0.ToString()).Replace("%2", arg1.ToString()).Replace("%3", arg2.ToString());
        }
    }
}