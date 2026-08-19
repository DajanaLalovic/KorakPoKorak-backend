using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KorakPoKorak.Infrastructure.Repositories
{
    public class ContentBlockRepository : IContentBlockRepository
    {
        private readonly AppDbContext _context;

        public ContentBlockRepository(AppDbContext context)
        {
            _context = context;
        }

        public void ReplaceForLesson(int lessonId, IReadOnlyList<(MediaAsset Asset, int OrderIndex)> blocks)
        {
            Replace(lessonId, null, blocks);
        }

        public void ReplaceForExercise(int exerciseId, IReadOnlyList<(MediaAsset Asset, int OrderIndex)> blocks)
        {
            Replace(null, exerciseId, blocks);
        }

        private void Replace(
            int? lessonId,
            int? exerciseId,
            IReadOnlyList<(MediaAsset Asset, int OrderIndex)> blocks)
        {
            var existing = _context.ContentBlocks
                .Include(b => b.MediaAsset)
                .Where(b => lessonId.HasValue
                    ? b.LessonId == lessonId
                    : b.ExerciseId == exerciseId)
                .ToList();

            if (existing.Count > 0)
            {
                var assets = existing.Select(b => b.MediaAsset).ToList();
                _context.ContentBlocks.RemoveRange(existing);
                _context.MediaAssets.RemoveRange(assets);
            }

            foreach (var (asset, orderIndex) in blocks)
            {
                _context.MediaAssets.Add(asset);
                _context.ContentBlocks.Add(new ContentBlock
                {
                    LessonId = lessonId,
                    ExerciseId = exerciseId,
                    OrderIndex = orderIndex,
                    MediaAsset = asset
                });
            }

            _context.SaveChanges();
        }
    }
}
