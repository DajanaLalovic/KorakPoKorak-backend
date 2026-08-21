using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.IServices
{
    public interface IWorkshopService
    {
        PagedResult<WorkshopDto> GetFiltered(WorkshopQueryParams q);
        WorkshopSummaryDto GetSummary();
        WorkshopDto? GetById(int id);
        List<WorkshopDto> GetMy(int userId);
        MentorStudentCountDto GetMyStudentCount(int userId);
        List<WorkshopStudentDto> GetStudents(int workshopId);
        List<WorkshopDto> GetRecent(int count);
        List<LessonDto> GetLessons(int workshopId);
        List<ExerciseDto> GetExercises(int workshopId);
        WorkshopStatsDto GetStats(int id);
        void Create(CreateWorkshopDto dto, int createdById);
        void Update(int id, UpdateWorkshopDto dto);
        void Delete(int id);
        void PatchStatus(int id, WorkshopStatus status);
        void AttachLesson(int workshopId, int lessonId);
        void DetachLesson(int workshopId, int lessonId);
        void AttachExercise(int workshopId, int exerciseId);
        void DetachExercise(int workshopId, int exerciseId);
    }
}
