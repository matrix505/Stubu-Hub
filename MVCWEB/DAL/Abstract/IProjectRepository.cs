using MVCWEB.Models;
using MVCWEB.Models.Entities;

namespace MVCWEB.DAL.Abstract
{
    public interface IProjectRepository
    {
        Task<PaginatedResult<Project>> BrowseAllProjects(int page, int pageSize, string? search);
        Task<PaginatedResult<Project>> GetOwnedProjects(int userId,int page, int pageSize);
        Task<PaginatedResult<Project>> GetJoinedProjects(int userId,int page, int pageSize, string? search);
        Task<Project> GetByIdAsync(int projectId);
        Task<List<TeamMembers>> GetProjectTeamMembers(int projectId);
        Task CreateProject(int userId,Project project);
        Task DisposeProject(int ownerId,int projectId);
        Task<List<Categories>> GetAllAvailableCategories();
        Task<List<Skills>> GetAllAvailableSkills();

    }
}
