using KorakPoKorak.Application.DTOs;

namespace KorakPoKorak.Application.IServices
{
    public interface IUserService
    {
        List<UserDto> GetAll();
        UserDto? GetById(int id);
        List<UserDto> GetMentors();
        void Create(CreateUserDto dto);
        void Delete(int id);
    }
}
