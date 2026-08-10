using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface IChildRepository
    {
        List<ChildProfile> GetByParent(int parentId);
        ChildProfile? GetByIdForParent(int childId, int parentId);
        void Add(ChildProfile child);
        void Update(ChildProfile child);
        void Delete(ChildProfile child);
    }
}
