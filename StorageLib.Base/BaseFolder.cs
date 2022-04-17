﻿namespace BassClefStudio.Storage.Base
{
    /// <summary>
    /// An <see cref="IStorageFolder"/> implementation that uses the .NET <see cref="DirectoryInfo"/> class for creating and managing a folder.
    /// </summary>
    public class BaseFolder : IStorageFolder
    {
        private DirectoryInfo Directory { get; }

        /// <inheritdoc/>
        public string Name => Directory.Name;

        /// <inheritdoc/>
        public bool HasPath { get; } = true;

        /// <summary>
        /// Creates a <see cref="BaseFolder"/> from the given directory.
        /// </summary>
        /// <param name="directory">The .NET <see cref="DirectoryInfo"/> directory.</param>
        internal BaseFolder(DirectoryInfo directory)
        {
            Directory = directory;
            if (!Directory.Exists)
            {
                Directory.Create();
            }
        }

        /// <inheritdoc/>
        public string GetPath() => Directory.FullName;

        /// <inheritdoc/>
        public async Task<IStorageFile> CreateFileAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting)
        {
            var info = new FileInfo(Path.Combine(Directory.FullName, name));
            if (info.Exists && options != CollisionOptions.Overwrite)
            {
                if (options == CollisionOptions.FailIfExists)
                {
                    throw new StorageAccessException($"The requested file {name} already exists.");
                }
                else if (options == CollisionOptions.OpenExisting)
                {
                    return new BaseFile(info);
                }
                else if (options == CollisionOptions.RenameIfExists)
                {
                    int num = 1;
                    while (info.Exists)
                    {
                        info = new FileInfo(Path.Combine(Directory.FullName, $"{name}_{num}"));
                        num++;
                    }
                    return new BaseFile(info);
                }
                else
                {
                    throw new StorageAccessException($"The given overwrite behavior {options} is not supported by BaseFolder.");
                }
            }
            else
            {
                using (var stream = info.CreateText())
                {
                    await stream.WriteAsync(string.Empty);
                    await stream.FlushAsync();
                }
                return new BaseFile(info);
            }
        }

        /// <inheritdoc/>
        public async Task<IStorageFolder> CreateFolderAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting)
        {
            var info = new DirectoryInfo(Path.Combine(Directory.FullName, name));
            if (info.Exists && options != CollisionOptions.Overwrite)
            {
                if (options == CollisionOptions.FailIfExists)
                {
                    throw new StorageAccessException($"The requested folder {name} already exists.");
                }
                else if (options == CollisionOptions.OpenExisting)
                {
                    return new BaseFolder(info);
                }
                else if (options == CollisionOptions.RenameIfExists)
                {
                    int num = 1;
                    while (info.Exists)
                    {
                        info = new DirectoryInfo(Path.Combine(Directory.FullName, $"{name}_{num}"));
                        num++;
                    }
                    return new BaseFolder(info);
                }
                else
                {
                    throw new StorageAccessException($"The given overwrite behavior {options} is not supported by BaseFolder.");
                }
            }
            else
            {
                info.Create();
                return new BaseFolder(info);
            }
        }

        /// <inheritdoc/>
        public async Task<IStorageFile> GetFileAsync(string relativePath)
        {
            var info = new FileInfo(Path.Combine(Directory.FullName, relativePath));
            if(info.Exists)
            {
                return new BaseFile(info);
            }
            else
            {
                throw new StorageAccessException($"The given file {Path.Combine(Directory.FullName, relativePath)} does not exist.");
            }
        }

        /// <inheritdoc/>
        public async Task<IStorageFolder> GetFolderAsync(string relativePath)
        {
            var info = new DirectoryInfo(Path.Combine(Directory.FullName, relativePath));
            if (info.Exists)
            {
                return new BaseFolder(info);
            }
            else
            {
                throw new StorageAccessException($"The given file {Path.Combine(Directory.FullName, relativePath)} does not exist.");
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IStorageItem>> GetItemsAsync()
        {
            return Directory.EnumerateFiles()
                .Select<FileInfo, IStorageItem>(f => new BaseFile(f))
                .Concat(
                Directory.EnumerateDirectories()
                    .Select<DirectoryInfo, IStorageItem>(d => new BaseFolder(d)));
        }

        /// <inheritdoc/>
        public async Task RemoveAsync()
        {
            Directory.Delete(true);
        }

        /// <inheritdoc/>
        public async Task RenameAsync(string desiredName)
        {
            string? folderName = Path.GetDirectoryName(Directory.FullName);
            if (folderName is null)
            {
                throw new StorageAccessException($"Could not find or create the desired folder {folderName}.");
            }

            await this.MoveToAsync(
                new BaseFolder(new DirectoryInfo(folderName)),
                CollisionOptions.FailIfExists,
                desiredName);
        }

        /// <inheritdoc/>
        public static bool operator ==(BaseFolder a, BaseFolder b) => a.Directory == b.Directory;
        /// <inheritdoc/>
        public static bool operator !=(BaseFolder a, BaseFolder b) => !(a == b);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is BaseFolder folder
                && this == folder;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
