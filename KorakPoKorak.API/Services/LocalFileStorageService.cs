using KorakPoKorak.Application.IServices;

namespace KorakPoKorak.API.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private const string RelativeFolder = "uploads/children";
        private readonly string _absoluteFolder;

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            _absoluteFolder = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), RelativeFolder);
            Directory.CreateDirectory(_absoluteFolder);
        }

        public string SaveChildAvatar(Stream content, string extension)
        {
            var safeExtension = extension.StartsWith('.') ? extension.ToLowerInvariant() : $".{extension.ToLowerInvariant()}";
            var fileName = $"{Guid.NewGuid():N}{safeExtension}";
            var absolutePath = Path.Combine(_absoluteFolder, fileName);

            using (var fileStream = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                content.CopyTo(fileStream);
            }

            return $"/{RelativeFolder}/{fileName}".Replace('\\', '/');
        }

        public void DeleteChildAvatar(string? avatarUrl)
        {
            if (string.IsNullOrWhiteSpace(avatarUrl))
                return;

            var relative = avatarUrl.TrimStart('/').Replace('\\', '/');
            var prefix = $"{RelativeFolder}/";
            if (!relative.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return;

            var fileName = Path.GetFileName(relative);
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var absolutePath = Path.Combine(_absoluteFolder, fileName);
            if (File.Exists(absolutePath))
                File.Delete(absolutePath);
        }
    }
}
