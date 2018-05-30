using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PossumLabs.Specflow.Core
{
    public class ParserRules
    {
        public static Regex IsVariable = new Regex(@"[a-zA-W]\w*(\.[a-zA-Z]\w*)*");
        public static Regex IsValidMappedHeader = new Regex(@"[a-zA-W]\w*([\. ][a-zA-Z]\w*)*");
        public static string VaraibleKey = "var";
    }
}
