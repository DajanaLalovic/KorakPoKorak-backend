using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface ILessonRepository
    {
        (List<Lesson> Items, int Total) GetFiltered(LessonQueryParams q);
        Lesson? GetById(int id);
        List<Lesson> GetMy(int userId);
        List<Lesson> GetRecent(int count);
        void Add(Lesson lesson);
        void Update(Lesson lesson);
        void Delete(int id);
    }
}
