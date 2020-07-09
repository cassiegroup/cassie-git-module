using System;
namespace cassie.git.module
{
    public static class Extensions
    {
        public static string TrimSuffix(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            else
            {
                return s;
            }
        }
        
    }
}