using PossumLabs.Specflow.Core.Variables;
using System.IO;

namespace PossumLabs.Specflow.Core.Files
{
    public interface IFile: IDomainObject
    {
        Stream Stream { get; }
    }
}