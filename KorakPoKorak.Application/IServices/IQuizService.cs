using KorakPoKorak.Application.DTOs;

namespace KorakPoKorak.Application.IServices
{
    public interface IQuizService
    {
        PagedResult<QuizDto> GetFiltered(QuizQueryParams q);
        QuizDto? GetById(int id);
        List<QuizDto> GetMy(int userId);
        QuizDto Create(CreateQuizDto dto, int createdById);
        QuizDto Update(int id, UpdateQuizDto dto);
        void Delete(int id);
    }
}
