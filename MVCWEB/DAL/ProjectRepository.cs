using Dapper;
using MVCWEB.DAL.Abstract;
using MVCWEB.Data;
using MVCWEB.Models.Entities;

namespace MVCWEB.DAL
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DapperContext _dapperContext;
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(
            DapperContext dapperContext,
            ILogger<ProjectRepository> logger
            )
        {
            _dapperContext = dapperContext;
            _logger = logger;
        }
        public async Task CreateProject(int userId, Project project)
        {
            using var conn = _dapperContext.CreateConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction();
            try
            {
                // for the project insert
                string projectQuery = @"
                    INSERT INTO Project (Owner_id,Title,Description,MemberSize) VALUES 
                    (@UserId, @Title, @Desc, @Size); SELECT SCOPE_IDENTITY();";
                int projectId = await conn.ExecuteScalarAsync<int>(projectQuery,
                    new
                    {
                        UserId = userId,
                        project.Title,
                        Desc = project.Description,
                        Size = project.MemberSize
                    }, transaction);

                // categories of project
                string projectCategories = @"INSERT INTO ProjectCategories 
                                           ( Project_id, Category_id) VALUES 
                                           ( @Project_id, @Category_id)";
                await conn.ExecuteAsync(projectCategories,
                    project.Categories!.Select(e =>
                    new
                    {
                        Project_id = projectId,
                        e.Category_id
                    }).ToList()
                    , transaction);

                // skills requirement for project
                string projectSkills = @"INSERT INTO ProjectSkills
                                           ( Project_id, Skill_id) VALUES 
                                           ( @Project_id, @Skill_id)";
                await conn.ExecuteAsync(projectSkills,
                    project.Skills!.Select(e =>
                    new
                    {
                        Project_id = projectId,
                        e.Skill_id
                    }).ToList()
                    , transaction);

                string Role = "Testing"; // Todo : Make a role manager for owner of project
                // insert also the owwnerr
                string teamProject = @"INSERT INTO TeamMembers (Project_id, User_id, Role) VALUES (@ProjectId, @UserId, @Role)";
                await conn.ExecuteAsync(teamProject,
                    new { UserId = userId, ProjectId = projectId, Role = Role }

                    , transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                transaction.Rollback();
            }

        }

        public Task DisposeProject(int OwnerId)
        {
            throw new NotImplementedException();
        }

        public async Task<Project?> GetMainProject(int ProjectId)
        {
            using var conn = _dapperContext.CreateConnection();

            var query = @"
        SELECT 
            p.Project_id, 
            p.Title, 
            p.Description, 
            p.Status, 
            p.CreatedAt, 
            p.MemberSize,
            COUNT(DISTINCT tm.User_id) AS TotalMembers,
            STRING_AGG(c.Category_name, ', ') AS CategoryNames
            FROM Project p
            LEFT JOIN ProjectCategories pc ON pc.Project_id = p.Project_id
            LEFT JOIN Categories c ON c.Category_id = pc.Category_id
            LEFT JOIN TeamMembers tm ON tm.Project_id = p.Project_id
            WHERE p.Project_id = @ProjectId
            GROUP BY 
            p.Project_id, 
            p.Title, 
            p.Description, 
            p.Status, 
            p.CreatedAt, 
            p.MemberSize;";

            return await conn.QueryFirstOrDefaultAsync<Project>(query, new
            {
                ProjectId = ProjectId
            });
        }

        public Task<bool> IsProjectFull(int ProjectId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserInRequest(int UserId, int ProjectId)
        {
            using var conn = _dapperContext.CreateConnection();
            var query = @"
                     SELECT COUNT(1)
                     FROM JoinRequests
                     WHERE User_id = @UserId
                     AND Project_id = @ProjectId
                     AND Status = 'Pending'";
            var count = await conn.ExecuteScalarAsync<int>(query, new { UserId, ProjectId });
            return count > 0;
        }

        public async Task<bool> IsUserProjectMember(int UserId, int ProjectId)
        {
            using var conn = _dapperContext.CreateConnection();
            var query = @"
                SELECT COUNT(1)
                FROM TeamMembers tm
                WHERE tm.User_id = @UserId
                AND tm.Project_id = @ProjectId";

            var count = await conn.ExecuteScalarAsync<int>(query, new { UserId, ProjectId });
            return count > 0;
        }

        public async Task<bool> IsUserProjectOwner(int UserId, int ProjectId)
        {
            using var conn = _dapperContext.CreateConnection();
            var query = @"
                SELECT COUNT(1)
                FROM Project p
                WHERE p.Project_id = @ProjectId
                AND p.Owner_id = @UserId";

            var count = await conn.ExecuteScalarAsync<int>(query, new { UserId, ProjectId });
            return count > 0;
        }

        public async Task RequestToJoin(JoinRequests request)
        {
            using var conn = _dapperContext.CreateConnection();

            //var isMember = await IsUserProjectMember(request.User_id, request.Project_id);
            //if (isMember)
            //    throw new InvalidOperationException("User is already a member of this project.");

            //var existingRequestQuery = @"
            //        SELECT COUNT(1) 
            //        FROM JoinRequests 
            //        WHERE Project_id = @Project_id 
            //        AND User_id = @User_id 
            //        AND Status = 'Pending'";

            //var existingRequest = await conn.ExecuteScalarAsync<int>(existingRequestQuery, new
            //{
            //    request.Project_id,
            //    request.User_id
            //});

            //if (existingRequest > 0)
            //    throw new InvalidOperationException("User already has a pending request for this project.");

            // Insert join request
            var insertQuery = @"
                INSERT INTO JoinRequests (Project_id, User_id,Status)
                VALUES (@Project_id, @User_id,'Pending')";

            await conn.ExecuteAsync(insertQuery, new
            {
                request.Project_id,
                request.User_id
            });
        }
    }
}
