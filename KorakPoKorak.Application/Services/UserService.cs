using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IRoleRepository _roleRepo;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository repo, IRoleRepository roleRepo, IPasswordHasher passwordHasher)
        {
            _repo = repo;
            _roleRepo = roleRepo;
            _passwordHasher = passwordHasher;
        }

        public List<UserDto> GetAll()
        {
            return _repo.GetAll().Select(MapToDto).ToList();
        }

        public UserDto? GetById(int id)
        {
            var user = _repo.GetById(id);
            return user == null ? null : MapToDto(user);
        }

        public void Create(CreateUserDto dto)
        {
            var role = _roleRepo.GetByRoleType(dto.Role)
                ?? throw new InvalidOperationException($"Role '{dto.Role}' not found.");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                Phone = dto.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                RoleId = role.Id
            };

            _repo.Add(user);
        }

        public List<UserDto> GetMentors()
        {
            return _repo.GetAll()
                .Where(u => u.Role.RoleName == UserRole.Mentor)
                .Select(MapToDto)
                .ToList();
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        private static UserDto MapToDto(User u) => new()
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Phone = u.Phone,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            Role = new UserRoleDto
            {
                Id = u.Role.Id,
                RoleName = u.Role.RoleName.ToString(),
                Description = u.Role.Description
            }
        };
    }
}
