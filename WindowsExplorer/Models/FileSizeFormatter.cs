using System;

namespace WindowsExplorer.Models
{
    public static class FileSizeFormatter
    {
        private static readonly string[] _suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public static string FormatSize(long bytes)
        {
            var counter = 0;
            var number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return $"{number:n1} {_suffixes[counter]}";
        }
    }
}
