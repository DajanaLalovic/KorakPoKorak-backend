using KorakPoKorak.Application.DTOs;

namespace KorakPoKorak.Application.IServices
{
    public interface IChildService
    {
        List<ChildProfileDto> GetMyChildren(int parentId);
        List<ChildProfileDto> GetAllActive();
        List<MentorStudentDto> GetAllForMentor();
        ChildProfileDto? GetById(int childId, int parentId);
        ChildProfileDto Create(CreateChildDto dto, int parentId);
        ChildProfileDto Update(int childId, UpdateChildDto dto, int parentId);
        void Delete(int childId, int parentId);
        ChildProfileDto UploadAvatar(int childId, int parentId, Stream content, string contentType, long length);
    }
}
