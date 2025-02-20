namespace Source.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength, bool withEllispis = false)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if (value.Length <= maxLength) return value;
            return withEllispis ?
                value.Substring(0, maxLength-3) + "..." : 
                value.Substring(0, maxLength);
        }
        
    }
}