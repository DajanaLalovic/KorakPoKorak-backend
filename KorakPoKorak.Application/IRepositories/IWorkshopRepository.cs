using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface IWorkshopRepository
    {
        (List<Workshop> Items, int Total) GetFiltered(WorkshopQueryParams q);
        (int Total, int Published, int Draft) GetCounts();
        Workshop? GetById(int id);
        List<Workshop> GetMy(int userId);
        int CountDistinctStudents(int createdById);
        int CountActiveEnrollments(int workshopId);
        List<Enrollment> GetActiveEnrollments(int workshopId);
        List<Workshop> GetRecent(int count);
        List<Lesson> GetWorkshopLessons(int workshopId);
        List<Exercise> GetWorkshopExercises(int workshopId);
        void Add(Workshop workshop, List<int> lessonIds, List<int> exerciseIds, List<int> contributorIds);
        void Update(Workshop workshop, List<int> lessonIds, List<int> exerciseIds, List<int> contributorIds);
        void Delete(int id);
        void PatchStatus(int id, WorkshopStatus status);
        void AttachLesson(int workshopId, int lessonId);
        void DetachLesson(int workshopId, int lessonId);
        void AttachExercise(int workshopId, int exerciseId);
        void DetachExercise(int workshopId, int exerciseId);
    }
}
