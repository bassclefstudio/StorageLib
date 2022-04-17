﻿namespace BassClefStudio.Storage
{
    /// <summary>
    /// Extension methods for the <see cref="IStorageItem"/> and related interfaces.
    /// </summary>
    public static class StorageExtensions
    {
        #region Paths and Names

        /// <summary>
        /// Gets the <see cref="IStorageItem.Name"/> of the file without the extension.
        /// </summary>
        /// <param name="file">The <see cref="IStorageFile"/> to find the name of.</param>
        public static string GetNameWithoutExtension(this IStorageFile file)
            => Path.GetFileNameWithoutExtension(file.Name);

        /// <summary>
        /// Gets the path of a given <see cref="IStorageItem"/> relative to another <see cref="IStorageItem"/> on the same filesystem.
        /// </summary>
        /// <param name="item">The <see cref="IStorageItem"/> to find the path to.</param>
        /// <param name="relativeTo">An <see cref="IStorageItem"/> relative to <paramref name="item"/> from which the relative path will be calculated.</param>
        /// <returns>A <see cref="string"/> path relative to <paramref name="relativeTo"/> pointing to the location of <paramref name="item"/> on a path-based filesystem.</returns>
        /// <exception cref="StorageAccessException">The given <see cref="IStorageItem"/>s do not support <see cref="IStorageItem.GetPath"/> calls (see <see cref="IStorageItem.HasPath"/>).</exception>
        public static string GetRelativePath(this IStorageItem item, IStorageItem relativeTo)
            => Path.GetRelativePath(
                relativeTo.GetPath(),
                item.GetPath());

        #endregion
        #region Queries

        /// <summary>
        /// Checks to see if the file or folder at the given path is in the <see cref="IStorageFolder"/>.
        /// </summary>
        /// <param name="folder">The <see cref="IStorageFolder"/> folder to query.</param>
        /// <param name="name">The <see cref="string"/> name of the item in this <see cref="IStorageFolder"/>.</param>
        public static async Task<bool> ContainsItemAsync(this IStorageFolder folder, string name)
        {
            var allItems = await folder.GetItemsAsync();
            return allItems.Any(i => i.Name == name);
        }

        #endregion
        #region Copy and Move

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFile"/> into a new file located in the specified <see cref="IStorageFolder"/>.
        /// </summary>
        /// <param name="file">The existing file whose contents should be read.</param>
        /// <param name="destination">The destination folder to copy the <see cref="IStorageFile"/> into.</param>
        /// <param name="collisionOptions">Overrides the <see cref="CollisionOptions"/> behavior when creating the new file in the <paramref name="destination"/> folder.</param>
        /// <param name="fileName">Overrides the name of the destination file (default is <see cref="IStorageItem.Name"/> of the source <paramref name="file"/>).</param>
        public static async Task<IStorageFile> CopyToAsync(this IStorageFile file, IStorageFolder destination, CollisionOptions collisionOptions = CollisionOptions.RenameIfExists, string? fileName = null)
        {
            IStorageFile destinationFile = await destination.CreateFileAsync(fileName ?? file.Name, collisionOptions);
            await CopyContentsAsync(file, destinationFile);
            return destinationFile;
        }

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFile"/> into a new file located in the specified <see cref="IStorageFolder"/>, and then removes the source file.
        /// </summary>
        /// <param name="file">The existing file whose contents should be read.</param>
        /// <param name="destination">The destination folder to copy the <see cref="IStorageFile"/> into.</param>
        /// <param name="collisionOptions">Overrides the <see cref="CollisionOptions"/> behavior when creating the new file in the <paramref name="destination"/> folder.</param>
        /// <param name="fileName">Overrides the name of the destination file (default is <see cref="IStorageItem.Name"/> of the source <paramref name="file"/>).</param>
        public static async Task<IStorageFile> MoveToAsync(this IStorageFile file, IStorageFolder destination, CollisionOptions collisionOptions = CollisionOptions.RenameIfExists, string? fileName = null)
        {
            IStorageFile destinationFile = await destination.CreateFileAsync(fileName ?? file.Name, collisionOptions);
            await CopyContentsAsync(file, destinationFile);
            await file.RemoveAsync();
            return destinationFile;
        }

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFile"/> into the provided new <see cref="IStorageFile"/>.
        /// </summary>
        /// <param name="file">The existing file whose contents should be read.</param>
        /// <param name="destination">A new, empty file with write access, where the contents of <paramref name="file"/> will be copied.</param>
        public static async Task CopyContentsAsync(this IStorageFile file, IStorageFile destination)
        {
            using (var readFile = await file.OpenFileAsync(FileOpenMode.Read))
            {
                using (var readStream = readFile.GetReadStream())
                {
                    using (var writeFile = await destination.OpenFileAsync(FileOpenMode.ReadWrite))
                    {
                        using (var writeStream = writeFile.GetWriteStream())
                        {
                            await readStream.CopyToAsync(writeStream);
                            await writeStream.FlushAsync();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFile"/> into a new folder located in the specified <see cref="IStorageFolder"/>.
        /// </summary>
        /// <param name="folder">The existing file whose contents should be read.</param>
        /// <param name="destination">The destination folder to copy the <see cref="IStorageFile"/> into.</param>
        /// <param name="collisionOptions">Overrides the <see cref="CollisionOptions"/> behavior when creating the new file in the <paramref name="destination"/> folder.</param>
        /// <param name="folderName">Overrides the name of the destination file (default is <see cref="IStorageItem.Name"/> of the source <paramref name="folder"/>).</param>
        public static async Task<IStorageFolder> CopyToAsync(this IStorageFolder folder, IStorageFolder destination, CollisionOptions collisionOptions = CollisionOptions.RenameIfExists, string? folderName = null)
        {
            IStorageFolder destinationFolder = await destination.CreateFolderAsync(folderName ?? folder.Name, collisionOptions);
            await CopyContentsAsync(folder, destinationFolder);
            return destinationFolder;
        }

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFolder"/> into a new folder located in the specified <see cref="IStorageFolder"/>, and then removes the source folder.
        /// </summary>
        /// <param name="folder">The existing folder whose contents should be read.</param>
        /// <param name="destination">The destination folder to copy the <see cref="IStorageFile"/> into.</param>
        /// <param name="collisionOptions">Overrides the <see cref="CollisionOptions"/> behavior when creating the new file in the <paramref name="destination"/> folder.</param>
        /// <param name="folderName">Overrides the name of the destination file (default is <see cref="IStorageItem.Name"/> of the source <paramref name="folder"/>).</param>
        public static async Task<IStorageFolder> MoveToAsync(this IStorageFolder folder, IStorageFolder destination, CollisionOptions collisionOptions = CollisionOptions.RenameIfExists, string? folderName = null)
        {
            IStorageFolder destinationFolder = await destination.CreateFolderAsync(folderName ?? folder.Name, collisionOptions);
            await CopyContentsAsync(folder, destinationFolder);
            await folder.RemoveAsync();
            return destinationFolder;
        }

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFolder"/> into the provided new <see cref="IStorageFolder"/>.
        /// </summary>
        /// <param name="folder">The existing folder whose contents should be read.</param>
        /// <param name="destination">A new, empty folder with write access, where the contents of <paramref name="folder"/> will be copied.</param>
        public static async Task CopyContentsAsync(this IStorageFolder folder, IStorageFolder destination)
        {
            foreach(var item in await folder.GetItemsAsync())
            {
                if(item is IStorageFile file)
                {
                    await file.CopyToAsync(destination, CollisionOptions.FailIfExists);
                }
                else if(item is IStorageFolder dir)
                {
                    await dir.CopyToAsync(destination, CollisionOptions.FailIfExists);
                }
                else
                {
                    throw new StorageAccessException($"Attempted to copy an item that was neither a file or folder. Type \"{item?.GetType().Name}\"");
                }
            }
        }

        #endregion
    }
}
