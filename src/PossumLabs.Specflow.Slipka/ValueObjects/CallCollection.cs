using PossumLabs.Specflow.Core;
using PossumLabs.Specflow.Core.Variables;
using PossumLabs.Specflow.Slipka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PossumLabs.Specflow.Slipka.ValueObjects
{

    public class CallCollection : List<CallRecord>, IValueObject
    {
        public CallCollection(IEnumerable<CallRecord> calls):base(calls)
        {

        }
    }
}
