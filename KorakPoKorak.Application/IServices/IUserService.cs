using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.IServices
{
    public interface IUserService
    {
        List<UserDto> GetAll();
        UserDto? GetById(int id);
        List<UserDto> GetMentors();
        void Create(CreateUserDto dto);
        void Update(int id, UpdateUserDto dto);
        void SetActive(int id, bool isActive);
        void ChangeRole(int id, UserRole role);
        void Delete(int id);
    }
}
