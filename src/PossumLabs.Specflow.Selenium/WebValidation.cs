using PossumLabs.Specflow.Core.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PossumLabs.Specflow.Selenium
{
    public class WebValidation : Validation
    {
        public WebValidation(Func<object, string> predicate, string text) : base(predicate, text)
        {
        }
    }
}
