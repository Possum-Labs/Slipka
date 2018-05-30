using System.Collections.Generic;

namespace PossumLabs.Specflow.Slipka
{
    public class Message
    {
        public List<KeyValuePair<string, List<string>>> Headers { get; set; }
        public string Content { get; set; }
    }
}
