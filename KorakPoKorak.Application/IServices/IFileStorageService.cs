namespace KorakPoKorak.Application.IServices
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Saves a child avatar under the local uploads folder and returns a public relative URL
        /// (e.g. /uploads/children/{guid}.jpg).
        /// </summary>
        string SaveChildAvatar(Stream content, string extension);

        /// <summary>
        /// Deletes a previously stored local child avatar if the URL points at our uploads folder.
        /// </summary>
        void DeleteChildAvatar(string? avatarUrl);
    }
}
