using System.Collections.Generic;

namespace PossumLabs.Specflow.Slipka
{
    public class Message
    {
        public List<Header> Headers { get; set; }
        public string Content { get; set; }
    }
}
