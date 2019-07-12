using System.Threading;
using WindowsExplorer.Models;

namespace WindowsExplorer.Infrastructure
{
    public interface IRepository
    {
        DirectoryFromDisk[] GetAllDisk();
        DirectoryFromDisk[] GetSubDirectory(string path);
        ObjectFromDisk[] GetObjectsFromDisk();
        ObjectFromDisk[] GetObjectsFromDirectory(DirectoryFromDisk directoryFromDisk, CancellationToken token);
    }
}
