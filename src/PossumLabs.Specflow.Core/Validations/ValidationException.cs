using System;
using System.Collections.Generic;
using System.Text;

namespace PossumLabs.Specflow.Core.Validations
{
    public class ValidationException : Exception
    {
        public ValidationException(string msg):base(msg)
        {
        }
    }
}
