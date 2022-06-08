﻿using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace Avalonia.Platform.Storage
{
    /// <summary>
    /// Manipulates storage items (files and folders) and their contents, and provides information about them
    /// </summary>
    [Unstable]
    public interface IStorageItem
    {
        /// <summary>
        /// Gets the name of the item including the file name extension if there is one.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full file-system path of the item, if the item has a path.
        /// </summary>
        bool TryGetFullPath([NotNullWhen(true)] out string? path);

        /// <summary>
        /// Gets the basic properties of the current item.
        /// </summary>
        Task<StorageItemProperties> GetBasicPropertiesAsync();

        /// <summary>
        /// Returns true is item can be bookmarked and reused later.
        /// </summary>
        bool CanBookmark { get; }
        
        /// <summary>
        /// Saves items to a bookmark.
        /// </summary>
        /// <returns>
        /// Returns identifier of a bookmark. Can be null if OS denied request.
        /// </returns>
        Task<string?> SaveBookmark();

        /// <summary>
        /// Gets the parent folder of the current storage item.
        /// </summary>
        Task<IStorageFolder?> GetParentAsync();
    }
}
