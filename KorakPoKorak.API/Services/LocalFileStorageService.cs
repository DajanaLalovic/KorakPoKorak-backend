using KorakPoKorak.Application.IServices;

namespace KorakPoKorak.API.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private const string ChildrenFolder = "uploads/children";
        private const string ContentFolder = "uploads/content";

        private readonly string _childrenAbsoluteFolder;
        private readonly string _contentAbsoluteFolder;

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
            _childrenAbsoluteFolder = Path.Combine(webRoot, ChildrenFolder);
            _contentAbsoluteFolder = Path.Combine(webRoot, ContentFolder);
            Directory.CreateDirectory(_childrenAbsoluteFolder);
            Directory.CreateDirectory(_contentAbsoluteFolder);
        }

        public string SaveChildAvatar(Stream content, string extension)
            => SaveToFolder(content, extension, _childrenAbsoluteFolder, ChildrenFolder);

        public void DeleteChildAvatar(string? avatarUrl)
        {
            if (string.IsNullOrWhiteSpace(avatarUrl))
                return;

            var relative = avatarUrl.TrimStart('/').Replace('\\', '/');
            var prefix = $"{ChildrenFolder}/";
            if (!relative.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return;

            var fileName = Path.GetFileName(relative);
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var absolutePath = Path.Combine(_childrenAbsoluteFolder, fileName);
            if (File.Exists(absolutePath))
                File.Delete(absolutePath);
        }

        public string SaveContentFile(Stream content, string extension)
            => SaveToFolder(content, extension, _contentAbsoluteFolder, ContentFolder);

        private static string SaveToFolder(Stream content, string extension, string absoluteFolder, string relativeFolder)
        {
            var safeExtension = extension.StartsWith('.')
                ? extension.ToLowerInvariant()
                : $".{extension.ToLowerInvariant()}";
            var fileName = $"{Guid.NewGuid():N}{safeExtension}";
            var absolutePath = Path.Combine(absoluteFolder, fileName);

            using (var fileStream = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                content.CopyTo(fileStream);
            }

            return $"/{relativeFolder}/{fileName}".Replace('\\', '/');
        }
    }
}
