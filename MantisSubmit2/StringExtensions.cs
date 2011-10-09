using System;

namespace MantisSubmit2
{
    public static class StringExtensions
    {
        public static int CalculateSimilarity(this string thisStr, string str)
        {
            int similarity = 0;
            for (int i = 0; i < Math.Min(thisStr.Length, str.Length); i++)
            {
                if (thisStr[i] == str[i])
                    similarity++;
                else
                    break;
            }
            return similarity;
        }
    }
}
