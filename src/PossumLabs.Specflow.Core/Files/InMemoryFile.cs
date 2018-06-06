using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PossumLabs.Specflow.Core.Files
{
    public class InMemoryFile : IFile
    {
        public InMemoryFile(byte[] data)
        {
            Data = data;
        }

        private byte[] Data { get; }

        public virtual Stream Stream { get => new MemoryStream(Data); }
    }
}
