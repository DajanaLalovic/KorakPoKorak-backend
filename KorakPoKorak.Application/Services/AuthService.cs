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
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository repo,
            IRoleRepository roleRepo,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IChildRepository childRepo,
            IEmailService emailService)
        {
            _repo = repo;
            _roleRepo = roleRepo;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _childRepo = childRepo;
            _emailService = emailService;
        }

        public AuthResponseDto Login(LoginDto dto)
        {
            var user = _repo.GetByEmail(dto.Email);

            if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (!user.IsActive)
                throw new InvalidOperationException("Your account is not activated. Please check your email.");

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

        public RegisterResponseDto Register(RegisterDto dto)
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

            var activationToken = Guid.NewGuid().ToString("N");

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                Phone = dto.Phone,
                IsActive = false,
                ActivationToken = activationToken,
                ActivationTokenExpires = DateTime.UtcNow.AddHours(24),
                CreatedAt = DateTime.UtcNow,
                RoleId = role.Id,
                Role = role
            };

            _repo.Add(user);
            _emailService.SendActivationEmail(user.Email, user.FirstName, activationToken, dto.Language);

            return new RegisterResponseDto
            {
                Email = user.Email,
                Message = "Check your email to confirm your account."
            };
        }

        public void ActivateAccount(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Activation token is missing.");

            var user = _repo.GetByActivationToken(token)
                ?? throw new KeyNotFoundException("This activation link is invalid.");

            if (user.IsActive)
                return;

            if (user.ActivationTokenExpires.HasValue && user.ActivationTokenExpires.Value < DateTime.UtcNow)
                throw new InvalidOperationException("This activation link has expired. Please register again.");

            user.IsActive = true;
            user.ActivationToken = null;
            user.ActivationTokenExpires = null;
            _repo.Update(user);
        }

        public void ForgotPassword(ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return;

            var user = _repo.GetByEmail(dto.Email);
            if (user == null)
                return;

            var resetToken = Guid.NewGuid().ToString("N");
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(24);
            _repo.Update(user);

            _emailService.SendPasswordResetEmail(user.Email, user.FirstName, resetToken, dto.Language);
        }

        public void ResetPassword(ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token))
                throw new InvalidOperationException("Reset token is missing.");

            if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
                throw new InvalidOperationException("Password must be at least 6 characters.");

            var user = _repo.GetByPasswordResetToken(dto.Token)
                ?? throw new KeyNotFoundException("This reset link is invalid.");

            if (user.PasswordResetTokenExpires.HasValue && user.PasswordResetTokenExpires.Value < DateTime.UtcNow)
                throw new InvalidOperationException("This reset link has expired. Please request a new one.");

            user.PasswordHash = _passwordHasher.Hash(dto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;
            _repo.Update(user);
        }
    }
}
