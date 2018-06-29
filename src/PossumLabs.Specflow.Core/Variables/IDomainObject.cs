using System;
using System.Collections.Generic;
using System.Text;

namespace PossumLabs.Specflow.Core.Variables
{
    public interface IDomainObject :IValueObject
    {
        string LogFormat();
    }
}
