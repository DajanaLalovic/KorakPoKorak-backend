using KorakPoKorak.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KorakPoKorak.Application.IServices
{
    public interface IUserService
    {
        List<UserDto> GetAll();
        UserDto GetById(int id);
        void Create(CreateUserDto dto);
        void Delete(int id);
    }
}