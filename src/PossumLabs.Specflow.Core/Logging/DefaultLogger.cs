using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PossumLabs.Specflow.Core.Logging
{
    public class DefaultLogger : ILog
    {
        public DefaultLogger(DirectoryInfo location)
        {
            Location = location;
        }

        protected virtual DirectoryInfo Location { get; }

        public void File(string name, byte[] data, string extension="txt")
        {
            
            var file = new FileInfo($"{Location.FullName}/{name}.{extension}");
            if (!file.Exists) // you may not want to overwrite existing files
            {
                using (Stream stream = file.OpenWrite())
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(data);
                }
            }
            else
            {
                throw new Exception($"This file '{name}' already exists in the log location {Location.FullName}");
            }
            Write($"{Open}a href=\"{file.RelativeFrom(Location)}\"{Close}{name}{Open}/a{Close}");
        }

        public void Message(string message)
            =>Write(message);

        public void Section(string section, string content)
            =>Write($"{Open}details{Close}{Open}summary{Close}{section}{Open}/summary{Close}{Open}p{Close}{content}{Open}/p{Close}{Open}/details{Close}");
        

        virtual protected string Open => "[[[";
        virtual protected string Close => "]]]";

        virtual protected void Write(string message)
            => Console.WriteLine(message);
    }
}
