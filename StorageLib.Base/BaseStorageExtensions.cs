namespace BassClefStudio.Storage.Base
{
    /// <summary>
    /// Provides extension methods for managing and creating <see cref="IStorageItem"/>s from .NET Core <see cref="FileInfo"/> and <see cref="DirectoryInfo"/> references.
    /// </summary>
    public static class BaseStorageExtensions
    {
        /// <summary>
        /// Creates an <see cref="IStorageFile"/> reference to the given <see cref="FileInfo"/> file.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/> file object to convert.</param>
        /// <returns>The resulting <see cref="IStorageFile"/>.</returns>
        public static IStorageFile AsFile(this FileInfo file)
            => new BaseFile(file);

        /// <summary>
        /// Creates an <see cref="IStorageFile"/> reference to the given <see cref="string"/> path.
        /// </summary>
        /// <param name="path">The <see cref="string"/> path of the file in the filesystem.</param>
        /// <returns>The resulting <see cref="IStorageFile"/>.</returns>
        public static IStorageFile AsFile(this string path)
            => new BaseFile(new FileInfo(path));

        /// <summary>
        /// Creates an <see cref="IStorageFolder"/> reference to the given <see cref="DirectoryInfo"/> file.
        /// </summary>
        /// <param name="dir">The <see cref="DirectoryInfo"/> directory object to convert.</param>
        /// <returns>The resulting <see cref="IStorageFolder"/>.</returns>
        public static IStorageFolder AsFolder(this DirectoryInfo dir)
            => new BaseFolder(dir);

        /// <summary>
        /// Creates an <see cref="IStorageFolder"/> reference to the given <see cref="string"/> path.
        /// </summary>
        /// <param name="path">The <see cref="string"/> path of the folder in the filesystem.</param>
        /// <returns>The resulting <see cref="IStorageFolder"/>.</returns>
        public static IStorageFolder AsFolder(this string path)
            => new BaseFolder(new DirectoryInfo(path));
    }
}
