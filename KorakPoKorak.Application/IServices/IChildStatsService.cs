using KorakPoKorak.Application.DTOs;

namespace KorakPoKorak.Application.IServices
{
    public interface IChildStatsService
    {
        ChildStatsDto GetStats(int childId, int parentId);
    }
}
