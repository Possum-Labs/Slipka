using PossumLabs.Specflow.Core;
using PossumLabs.Specflow.Slipka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PossumLabs.Specflow.Slipka.ValueObjects
{

    public class CallCollection : List<Call>, IValueObject
    {
        public CallCollection(IEnumerable<Call> calls):base(calls)
        {

        }
    }
}
