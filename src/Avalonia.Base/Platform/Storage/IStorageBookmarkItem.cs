using Avalonia.Metadata;

namespace Avalonia.Platform.Storage
{
    [Unstable]
    public interface IStorageBookmarkItem : IStorageItem
    {
    }

    [Unstable]
    public interface IStorageBookmarkFile : IStorageFile, IStorageBookmarkItem
    {
    }

    [Unstable]
    public interface IStorageBookmarkFolder : IStorageFolder, IStorageBookmarkItem
    {

    }
}
