using KorakPoKorak.Application.DTOs;

namespace KorakPoKorak.Application.IServices
{
    public interface ILessonService
    {
        PagedResult<LessonDto> GetFiltered(LessonQueryParams q);
        LessonDto? GetById(int id);
        List<LessonDto> GetMy(int userId);
        List<LessonDto> GetRecent(int count);
        void Create(CreateLessonDto dto, int createdById);
        void Update(int id, UpdateLessonDto dto);
        void Delete(int id);
    }
}
