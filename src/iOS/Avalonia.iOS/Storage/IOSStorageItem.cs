﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Logging;
using Avalonia.Platform.Storage;
using Foundation;

using UIKit;

#nullable enable

namespace Avalonia.iOS.Storage
{
    internal class IOSStorageItem : IStorageBookmarkItem
    {
        private readonly NSUrl _url;
        private readonly string _filePath;

        public IOSStorageItem(NSUrl url)
        {
            _url = url;

            using (var doc = new UIDocument(url))
            {
                _filePath = doc.FileUrl?.Path ?? url.FilePathUrl.Path;
                Name = doc.LocalizedName ?? Path.GetFileName(_filePath) ?? url.FilePathUrl.LastPathComponent;
            }
        }

        internal NSUrl Url => _url;

        public bool CanBookmark => true;

        public string Name { get; }

        public Task<StorageItemProperties> GetBasicPropertiesAsync()
        {
            var attributes = NSFileManager.DefaultManager.GetAttributes(_filePath, out var error);
            if (error is not null)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.IOSPlatform)?.
                    Log(this, "GetBasicPropertiesAsync returned an error: {ErrorCode} {ErrorMessage}", error.Code, error.LocalizedFailureReason);
            }
            return Task.FromResult(new StorageItemProperties(attributes?.Size, (DateTime)attributes?.CreationDate, (DateTime)attributes?.ModificationDate));
        }

        public Task<IStorageFolder?> GetParentAsync()
        {
            return Task.FromResult<IStorageFolder?>(new IOSStorageFolder(_url.RemoveLastPathComponent()));
        }

        public Task<string?> SaveBookmark()
        {
            try
            {
                _url.StartAccessingSecurityScopedResource();

                var newBookmark = _url.CreateBookmarkData(0, Array.Empty<string>(), null, out var bookmarkError);
                if (bookmarkError is not null)
                {
                    Logger.TryGet(LogEventLevel.Error, LogArea.IOSPlatform)?.
                        Log(this, "SaveBookmark returned an error: {ErrorCode} {ErrorMessage}", bookmarkError.Code, bookmarkError.LocalizedFailureReason);
                    return Task.FromResult<string?>(null);
                }

                return Task.FromResult<string?>(
                    newBookmark.GetBase64EncodedString(NSDataBase64EncodingOptions.None));
            }
            finally
            {
                _url.StopAccessingSecurityScopedResource();
            }
        }

        public bool TryGetFullPath([NotNullWhen(true)] out string path)
        {
            path = _filePath;
            return true;
        }
    }

    internal class IOSStorageFile : IOSStorageItem, IStorageBookmarkFile
    {
        public IOSStorageFile(NSUrl url) : base(url)
        {
        }

        public bool CanOpenRead => true;

        public bool CanOpenWrite => true;

        public Task<Stream> OpenRead()
        {
            return Task.FromResult<Stream>(new IOSSecurityScopedStream(Url, FileAccess.Read));
        }

        public Task<Stream> OpenWrite()
        {
            return Task.FromResult<Stream>(new IOSSecurityScopedStream(Url, FileAccess.Write));
        }
    }

    internal class IOSStorageFolder : IOSStorageItem, IStorageBookmarkFolder
    {
        public IOSStorageFolder(NSUrl url) : base(url)
        {
        }
    }
}
