using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class SessionEventArgs : EventArgs
    {
        public SessionEventArgs(Session session)
        {
            Session = session;
        }
        public Session Session { get; set; }
    }
}
