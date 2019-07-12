namespace WindowsExplorer.Models
{
    public class ObjectFromDisk
    {
        private long _size;
        public bool IsFile { get; set; }
        public string Name { get; set; }

        public long Size
        {
            get => _size;
            set
            {
                _size = value;
                SizeFormat = FileSizeFormatter.FormatSize(Size);
            }
        }

        public string SizeFormat { get; private set; }
    }
}
