using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Avalonia.Platform.Storage.FileIO
{
    public class BclStorageFile : IStorageBookmarkFile
    {
        private readonly FileInfo _fileInfo;

        public BclStorageFile(FileInfo fileInfo)
        {
            _fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            if (!_fileInfo.Exists)
            {
                throw new ArgumentException("File must exist", nameof(fileInfo));
            }
        }

        public bool CanOpenRead => true;

        public bool CanOpenWrite => true;

        public string Name => _fileInfo.Name;

        public virtual bool CanBookmark => true;

        public Task<StorageItemProperties> GetBasicPropertiesAsync()
        {
            var props = new StorageItemProperties(
                (ulong)_fileInfo.Length,
                _fileInfo.CreationTimeUtc,
                _fileInfo.LastAccessTimeUtc);
            return Task.FromResult(props);
        }

        public Task<IStorageFolder?> GetParentAsync()
        {
            if (_fileInfo.Directory is { } directory)
            {
                return Task.FromResult<IStorageFolder?>(new BclStorageFolder(directory));
            }
            return Task.FromResult<IStorageFolder?>(null);
        }

        public Task<Stream> OpenRead()
        {
            return Task.FromResult<Stream>(_fileInfo.OpenRead());
        }

        public Task<Stream> OpenWrite()
        {
            return Task.FromResult<Stream>(_fileInfo.OpenWrite());
        }
        
        public virtual Task<string?> SaveBookmark()
        {
            return Task.FromResult<string?>(_fileInfo.FullName);
        }

        public bool TryGetFullPath([NotNullWhen(true)] out string? path)
        {
            path = _fileInfo.FullName;
            return _fileInfo.Directory is not null;
        }
    }
}
