namespace RetroDb.Data.Extensions
{
    public static class StringExtensions
    {
        public static string WrapInQuotes(this string str) => "\"" + str + "\"";
    }
}
