using PossumLabs.Specflow.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PossumLabs.Specflow.Core.Variables
{
    public interface IOnErrorNotified
    {
        void OnError(string name, ILog log);
    }
}
