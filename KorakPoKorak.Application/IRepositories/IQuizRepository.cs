using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface IQuizRepository
    {
        (List<Quiz> Items, int Total) GetFiltered(QuizQueryParams q);
        Quiz? GetById(int id);
        List<Quiz> GetMy(int userId);
        void Add(Quiz quiz);
        void Update(Quiz quiz);
        void Delete(int id);
    }
}
