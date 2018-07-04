using System;
using System.Collections.Generic;
using System.Text;

namespace PossumLabs.Specflow.Slipka
{
    public class SlipkaInjection : CallTemplate
    {
        public SlipkaInjection():base()
        {
            Method = "GET";
            Duration = 0;
            EncodingHeader = new Header("Content-type", new string[] { "application/json", "charset=utf-8" });
            Response = new Message();
        }

        private Header EncodingHeader { get; }

        public List<string> Encoding
        {
            set
            {
                EncodingHeader.Values = value;
            }
        }

        public string Content
        {
            set
            {
                Response.Content = value;
            }
        }

        public string Path
        {
            set
            {
                Uri = $"^.*{value}$";
            }
        }
    }
}
