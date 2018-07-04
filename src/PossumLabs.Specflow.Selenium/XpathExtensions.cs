using System;
using System.Collections.Generic;
using System.Text;

namespace PossumLabs.Specflow.Selenium
{
    public static class XpathExtensions
    {
        public static String XpathEncode(this string value)
        {
            if (!value.Contains("'"))
                return '\'' + value + '\'';

            else if (!value.Contains("\""))
                return '"' + value + '"';

            else
                return "concat('" + value.Replace("'", "',\"'\",'") + "')";
        }
    }
}
