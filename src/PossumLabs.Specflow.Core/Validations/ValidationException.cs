using System;
using System.Collections.Generic;
using System.Text;

namespace PossumLabs.Specflow.Core
{
    public class ValidationException : Exception
    {
        public ValidationException(string msg):base(msg)
        {

        }
    }
}
