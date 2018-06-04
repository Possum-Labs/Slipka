using System;
using System.Collections.Generic;
using System.Text;

namespace PossumLabs.Specflow.Core
{
    public class Validation
    {
        public Validation(Func<object, string> predicate, string text)
        {
            Predicate = predicate;
            Text = text;
        }

        public Func<object, string> Predicate { get; private set; }
        public string Text { get; private set; }

        public Exception Validate(object o)
        {
            var msg = Predicate.Invoke(o);
            if (!string.IsNullOrWhiteSpace(msg))
                return new ValidationException(msg);
            return null;
        }

        public void Invoke(object o)
        {
            var msg = Predicate.Invoke(o);
            if (!string.IsNullOrWhiteSpace(msg))
                throw new ValidationException(msg);
        }
    }
}
