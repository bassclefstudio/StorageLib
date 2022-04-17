namespace BassClefStudio.Storage
{
    /// <summary>
    /// A service that can query and abstract over the platform's filesystem.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// A reference to the <see cref="IStorageFolder"/> where an app can store app data in the form of files.
        /// </summary>
        IStorageFolder AppDataFolder { get; }

        /// <summary>
        /// A reference to the <see cref="IStorageFolder"/> where an app can store temporary files/cache in the platform-specific file store. This folder may be periodically cleaned by the system.
        /// </summary>
        IStorageFolder TempFolder { get; }

        /// <summary>
        /// Prompts the user to open a file from their local filesystem, and returns an <see cref="IStorageFile"/> reference to that file.
        /// </summary>
        /// <param name="settings">A <see cref="StorageDialogSettings"/> describing the appearance and filters of the storage dialog.</param>
        /// <exception cref="StorageAccessException">Accessing the selected file failed, or the prompt threw an exception while browsing the filesystem.</exception>
        /// <returns>An <see cref="IStorageFile"/> object if the operation succeeded, and null if the operation is canceled gracefully by the user. If any other error occurs opening the file, a <see cref="StorageAccessException"/> is thrown.</returns>
        Task<IStorageFile> RequestFileOpenAsync(StorageDialogSettings settings);

        /// <summary>
        /// Prompts the user to save a file to their local filesystem, and returns an <see cref="IStorageFile"/> reference to that file.
        /// </summary>
        /// <param name="settings">A <see cref="StorageDialogSettings"/> describing the appearance and filters of the storage dialog.</param>
        /// <exception cref="StorageAccessException">Accessing the selected file failed, or the prompt threw an exception while browsing the filesystem.</exception>
        /// <returns>An <see cref="IStorageFile"/> object if the operation succeeded, and null if the operation is canceled gracefully by the user. If any other error occurs opening the file, a <see cref="StorageAccessException"/> is thrown.</returns>
        Task<IStorageFile> RequestFileSaveAsync(StorageDialogSettings settings);

        /// <summary>
        /// Prompts the user to select a folder/directory from their local filesystem, and returns an <see cref="IStorageFolder"/> reference to that folder.
        /// </summary>
        /// <param name="settings">A <see cref="StorageDialogSettings"/> describing the appearance and filters of the storage dialog.</param>
        /// <exception cref="StorageAccessException">Accessing the selected folder failed, or the prompt threw an exception while browsing the filesystem.</exception>
        /// <returns>An <see cref="IStorageFile"/> object if the operation succeeded, and null if the operation is canceled gracefully by the user. If any other error occurs opening the file, a <see cref="StorageAccessException"/> is thrown.</returns>
        Task<IStorageFolder> RequestFolderAsync(StorageDialogSettings settings);
    }

    /// <summary>
    /// A set of settings describing how a file dialog should behave.
    /// </summary>
    /// <param name="OverrideSelectText">Overrides the default text displayed to the user when they choose the file or folder. If null, the default is used.</param>
    /// <param name="ShownFileTypes">An array of <see cref="string"/> file types, not including the preceding dot, that the storage dialog should display (e.g. ["cs", "csproj"]). If empty, all files are shown.</param>
    public record StorageDialogSettings(string? OverrideSelectText, string[] ShownFileTypes);
}
