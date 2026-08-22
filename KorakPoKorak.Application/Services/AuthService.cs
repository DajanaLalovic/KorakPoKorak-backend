using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IRoleRepository _roleRepo;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IChildRepository _childRepo;

        public AuthService(
            IUserRepository repo,
            IRoleRepository roleRepo,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IChildRepository childRepo)
        {
            _repo = repo;
            _roleRepo = roleRepo;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _childRepo = childRepo;
        }

        public AuthResponseDto Login(LoginDto dto)
        {
            var user = _repo.GetByEmail(dto.Email);

            if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = _tokenService.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.RoleName.ToString()
            };

            if (user.Role.RoleName == UserRole.Parent)
                response.Children = _childRepo.GetByParent(user.Id).Select(ChildService.MapToSummary).ToList();

            return response;
        }

        public AuthResponseDto Register(RegisterDto dto)
        {
            if (_repo.GetByEmail(dto.Email) != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var roleType = UserRole.Child;
            if (!string.IsNullOrWhiteSpace(dto.Role) &&
                Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var parsedRole))
                roleType = parsedRole;

            var role = _roleRepo.GetByRoleType(roleType)
                ?? throw new InvalidOperationException($"{roleType} role is not configured in the database.");

            var firstName = dto.FirstName ?? string.Empty;
            var lastName = dto.LastName ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(dto.Name) && string.IsNullOrWhiteSpace(firstName))
            {
                var parts = dto.Name.Trim().Split(' ', 2);
                firstName = parts[0];
                lastName = parts.Length > 1 ? parts[1] : string.Empty;
            }

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                Phone = dto.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                RoleId = role.Id,
                Role = role
            };

            _repo.Add(user);

            var token = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.RoleName.ToString()
            };
        }
    }
}
