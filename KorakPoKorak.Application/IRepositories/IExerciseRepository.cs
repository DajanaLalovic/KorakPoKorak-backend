using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface IExerciseRepository
    {
        (List<Exercise> Items, int Total) GetFiltered(ExerciseQueryParams q);
        Exercise? GetById(int id);
        List<Exercise> GetMy(int userId);
        List<Exercise> GetRecent(int count);
        void Add(Exercise exercise);
        void Update(Exercise exercise);
        void Delete(int id);
    }
}
