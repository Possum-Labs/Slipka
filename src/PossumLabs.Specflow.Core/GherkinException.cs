using System;
using System.Collections.Generic;
using System.Text;

namespace PossumLabs.Specflow.Core
{
    public class GherkinException : Exception
    {
        public GherkinException(string message) : base(message)
        {
        }
    }
}
