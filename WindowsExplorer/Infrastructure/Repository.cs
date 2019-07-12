using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using WindowsExplorer.Models;

namespace WindowsExplorer.Infrastructure
{
    public class Repository : IRepository
    {
        public DirectoryFromDisk[] GetAllDisk()
        {
            return DriveInfo.GetDrives().Where(w => w.IsReady).Select(s => new DirectoryFromDisk
            {
                Name = s.Name,
                Patch = s.Name,
                IsContainsSubDirectory = TryIsContainsDirectories(s.RootDirectory)
            }).ToArray();
        }

        public DirectoryFromDisk[] GetSubDirectory(string path)
        {
            var dir = new DirectoryInfo(path);

            if (dir.Exists)
            {
                return dir.GetDirectories().Select(s => new DirectoryFromDisk
                {
                    Name = s.Name,
                    Patch = s.FullName,
                    IsContainsSubDirectory = TryIsContainsDirectories(s)
                }).ToArray();
            }

            return Array.Empty<DirectoryFromDisk>();
        }

        public ObjectFromDisk[] GetObjectsFromDisk()
        {
            return DriveInfo.GetDrives()
                .Where(w => w.IsReady)
                .Select(s => new ObjectFromDisk{
                    IsFile = false,
                    Name = s.Name,
                    Size = s.TotalSize - s.TotalFreeSpace
                }).ToArray();
        }

        public ObjectFromDisk[] GetObjectsFromDirectory(DirectoryFromDisk directoryFromDisk, CancellationToken token)
        {
#if DEBUG
            Debug.Print("Start of calculation");
            var watch = Stopwatch.StartNew();
#endif
            token.ThrowIfCancellationRequested();

            var dirInfo = new DirectoryInfo(directoryFromDisk.Patch);

            var dirs = dirInfo.EnumerateDirectories().AsParallel().Select(directory => new ObjectFromDisk
            {
                IsFile = false,
                Name = directory.Name,
                Size = CalculateSizeForDirectory(directory, token)
            }).ToArray();

            var files = dirInfo.EnumerateFiles().Select(file => new ObjectFromDisk
            {
                IsFile = true,
                Name = file.Name,
                Size = file.Length
            }).ToArray();

            var result = dirs.Concat(files).ToArray();
#if DEBUG
            watch.Stop();
            Debug.Print($"Stop of calculation, calculation time {watch.Elapsed.TotalSeconds}");
#endif
            return result;
        }

        private bool TryIsContainsDirectories(DirectoryInfo directoryInfo)
        {
            try
            {
                return directoryInfo.EnumerateDirectories().Any();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                return false;
            }
        }

        private static long CalculateSizeForDirectory(DirectoryInfo directoryInfo, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var locker = new object();
            long sized = 0;
            try
            {
                sized += directoryInfo.EnumerateFiles().Sum(f => f.Length);

                foreach (var enumerateDirectory in directoryInfo.EnumerateDirectories().AsParallel())
                {
                    token.ThrowIfCancellationRequested();

                    if (enumerateDirectory.Attributes.HasFlag(FileAttributes.ReparsePoint))
                        continue;

                    lock (locker)
                        sized += CalculateSizeForDirectory(enumerateDirectory, token);
                }
            }
            catch (UnauthorizedAccessException) {}
            return sized;

        }
    }
}
