using MVCWEB.Models.Entities;

namespace MVCWEB.DAL.Abstract
{
    public interface IProjectRepository
    {
        Task CreateProject(int userId, Project project);
        Task DisposeProject(int projectId);
        Task<Project?> GetMainProject(int ProjectId);
        Task<bool> IsUserProjectMember(int UserId, int ProjectId);
        Task<bool> IsUserProjectOwner(int UserId, int ProjectId);
        Task<bool> IsUserInRequest(int UserId, int ProjectId);

        Task<bool> IsProjectFull(int ProjectId);

        Task RequestToJoin(JoinRequests request);
        Task<List<JoinRequests>?> ViewJoinRequests(int ProjectId);
        Task<bool> AcceptJoinRequest(int RequestId);
        Task<bool> RejectJoinRequest(int RequestId);

        Task<List<TopicMessages>> GetDiscussionMessages(int ProjectId, int TopicId);

        Task<bool> LeaveProject(int UserId, int ProjectId);
        
    }
}
