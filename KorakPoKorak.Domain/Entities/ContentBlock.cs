namespace KorakPoKorak.Domain.Entities
{
    /// <summary>
    /// Ordered content block attached to a Lesson or Exercise (frontend ContentBlock / MediaAssign).
    /// Exactly one of LessonId / ExerciseId is set.
    /// </summary>
    public class ContentBlock
    {
        public int Id { get; set; }

        public int? LessonId { get; set; }
        public Lesson? Lesson { get; set; }

        public int? ExerciseId { get; set; }
        public Exercise? Exercise { get; set; }

        public int OrderIndex { get; set; }

        public int MediaAssetId { get; set; }
        public MediaAsset MediaAsset { get; set; } = null!;
    }
}
