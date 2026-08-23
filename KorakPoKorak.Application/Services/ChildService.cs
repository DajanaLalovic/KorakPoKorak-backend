using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    public class ChildService : IChildService
    {
        private const int MaxNotesLength = 1000;
        private const long MaxAvatarBytes = 2 * 1024 * 1024; // 2 MB

        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        private readonly IChildRepository _repo;
        private readonly IFileStorageService _fileStorage;

        public ChildService(IChildRepository repo, IFileStorageService fileStorage)
        {
            _repo = repo;
            _fileStorage = fileStorage;
        }

        public List<ChildProfileDto> GetMyChildren(int parentId)
        {
            return _repo.GetByParent(parentId).Select(MapToDto).ToList();
        }

        public List<ChildProfileDto> GetAllActive()
        {
            return _repo.GetAllActive().Select(MapToDto).ToList();
        }

        public List<MentorStudentDto> GetAllForMentor()
        {
            return _repo.GetAllActive().Select(MapToMentorDto).ToList();
        }

        public ChildProfileDto? GetById(int childId, int parentId)
        {
            var child = _repo.GetByIdForParent(childId, parentId);
            return child == null ? null : MapToDto(child);
        }

        public ChildProfileDto Create(CreateChildDto dto, int parentId)
        {
            ValidateProfile(dto.FirstName, dto.LastName, dto.DateOfBirth, dto.Gender, dto.Notes);

            var child = new ChildProfile
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Notes = NormalizeNotes(dto.Notes),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ParentId = parentId
            };

            _repo.Add(child);
            return MapToDto(child);
        }

        public ChildProfileDto Update(int childId, UpdateChildDto dto, int parentId)
        {
            ValidateProfile(dto.FirstName, dto.LastName, dto.DateOfBirth, dto.Gender, dto.Notes);

            var child = _repo.GetByIdForParent(childId, parentId)
                ?? throw new KeyNotFoundException($"Child with id {childId} not found.");

            child.FirstName = dto.FirstName.Trim();
            child.LastName = dto.LastName.Trim();
            child.DateOfBirth = dto.DateOfBirth;
            child.Gender = dto.Gender;
            child.Notes = NormalizeNotes(dto.Notes);
            child.IsActive = dto.IsActive;

            _repo.Update(child);
            return MapToDto(child);
        }

        public void Delete(int childId, int parentId)
        {
            var child = _repo.GetByIdForParent(childId, parentId)
                ?? throw new KeyNotFoundException($"Child with id {childId} not found.");

            var oldAvatar = child.AvatarUrl;
            _repo.Delete(child);
            _fileStorage.DeleteChildAvatar(oldAvatar);
        }

        public ChildProfileDto UploadAvatar(int childId, int parentId, Stream content, string contentType, long length)
        {
            var child = _repo.GetByIdForParent(childId, parentId)
                ?? throw new KeyNotFoundException($"Child with id {childId} not found.");

            var extension = ValidateAndResolveExtension(content, contentType, length);

            if (content.CanSeek)
                content.Position = 0;

            var oldAvatar = child.AvatarUrl;
            var newUrl = _fileStorage.SaveChildAvatar(content, extension);
            child.AvatarUrl = newUrl;
            _repo.Update(child);

            if (!string.Equals(oldAvatar, newUrl, StringComparison.OrdinalIgnoreCase))
                _fileStorage.DeleteChildAvatar(oldAvatar);

            return MapToDto(child);
        }

        private static void ValidateProfile(
            string firstName,
            string lastName,
            DateOnly? dateOfBirth,
            ChildGender? gender,
            string? notes)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.");

            if (dateOfBirth.HasValue && dateOfBirth.Value > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("Date of birth cannot be in the future.");

            if (gender.HasValue && !Enum.IsDefined(typeof(ChildGender), gender.Value))
                throw new ArgumentException("Gender value is invalid.");

            if (notes != null && notes.Length > MaxNotesLength)
                throw new ArgumentException($"Notes cannot exceed {MaxNotesLength} characters.");
        }

        private static string? NormalizeNotes(string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                return null;

            return notes.Trim();
        }

        private static string ValidateAndResolveExtension(Stream content, string contentType, long length)
        {
            if (length <= 0)
                throw new ArgumentException("Avatar file is empty.");

            if (length > MaxAvatarBytes)
                throw new ArgumentException("Avatar file must be 2 MB or smaller.");

            if (string.IsNullOrWhiteSpace(contentType) || !AllowedContentTypes.Contains(contentType))
                throw new ArgumentException("Avatar must be a JPEG, PNG, or WebP image.");

            var header = new byte[12];
            var read = content.Read(header, 0, header.Length);
            if (read < 4)
                throw new ArgumentException("Avatar file is invalid.");

            if (IsJpeg(header))
                return ".jpg";
            if (IsPng(header))
                return ".png";
            if (IsWebp(header, read))
                return ".webp";

            throw new ArgumentException("Avatar content does not match an allowed image format.");
        }

        private static bool IsJpeg(byte[] header) =>
            header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;

        private static bool IsPng(byte[] header) =>
            header.Length >= 8
            && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
            && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A;

        private static bool IsWebp(byte[] header, int read) =>
            read >= 12
            && header[0] == (byte)'R' && header[1] == (byte)'I' && header[2] == (byte)'F' && header[3] == (byte)'F'
            && header[8] == (byte)'W' && header[9] == (byte)'E' && header[10] == (byte)'B' && header[11] == (byte)'P';

        private static ChildProfileDto MapToDto(ChildProfile c) => new()
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            DateOfBirth = c.DateOfBirth,
            Gender = c.Gender,
            AvatarUrl = c.AvatarUrl,
            Notes = c.Notes,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            ParentId = c.ParentId
        };

        private static MentorStudentDto MapToMentorDto(ChildProfile c) => new()
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            DateOfBirth = c.DateOfBirth,
            Gender = c.Gender,
            AvatarUrl = c.AvatarUrl,
            Notes = c.Notes,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            ParentId = c.ParentId,
            ParentName = c.Parent != null
                ? $"{c.Parent.FirstName} {c.Parent.LastName}".Trim()
                : string.Empty,
            Workshops = (c.Enrollments ?? [])
                .Where(e => e.Status != EnrollmentStatus.Withdrawn)
                .OrderByDescending(e => e.EnrolledAt)
                .Select(e => new MentorStudentWorkshopDto
                {
                    EnrollmentId = e.Id,
                    WorkshopId = e.WorkshopId,
                    Title = e.Workshop?.Title ?? string.Empty,
                    Status = e.Status,
                    EnrolledAt = e.EnrolledAt
                }).ToList()
        };

        internal static ChildSummaryDto MapToSummary(ChildProfile c) => new()
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            AvatarUrl = c.AvatarUrl,
            IsActive = c.IsActive
        };
    }
}
