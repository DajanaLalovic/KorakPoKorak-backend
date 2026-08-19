using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface IContentBlockRepository
    {
        void ReplaceForLesson(int lessonId, IReadOnlyList<(MediaAsset Asset, int OrderIndex)> blocks);
        void ReplaceForExercise(int exerciseId, IReadOnlyList<(MediaAsset Asset, int OrderIndex)> blocks);
    }
}
