using KorakPoKorak.Application.DTOs;

namespace KorakPoKorak.Application.IServices
{
    public interface IExerciseService
    {
        PagedResult<ExerciseDto> GetFiltered(ExerciseQueryParams q);
        ExerciseDto? GetById(int id);
        List<ExerciseDto> GetMy(int userId);
        List<ExerciseDto> GetRecent(int count);
        void Create(CreateExerciseDto dto, int createdById);
        void Update(int id, UpdateExerciseDto dto);
        void Delete(int id);
    }
}
