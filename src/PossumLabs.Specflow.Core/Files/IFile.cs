using System.IO;

namespace PossumLabs.Specflow.Core.Files
{
    public interface IFile: IValueObject
    {
        Stream Stream { get; }
    }
}