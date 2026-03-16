using Dapper;
using MVCWEB.DAL.Abstract;
using MVCWEB.Data;
using MVCWEB.Models;
using MVCWEB.Models.Entities;

namespace MVCWEB.DAL
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DapperContext _dapperContext;
        private readonly ILogger<ProjectRepository> _logger;
        public ProjectRepository(
            DapperContext dapper,
            ILogger<ProjectRepository> logger
            ) {
            _dapperContext = dapper;
            _logger = logger;
        }
        public async Task<PaginatedResult<Project>> BrowseAllProjects(int page, int pageSize, string? search)
        {
            using var conn = _dapperContext.CreateConnection();
            var OFFSET = (page - 1) * pageSize;

            var query = @"
                SELECT p.Project_id, p.Title, p.Description, p.Status, p.CreatedAt, p.MemberSize,
                COUNT(DISTINCT tm.User_id) AS TotalMembers,
                STRING_AGG(c.Category_name, ', ') AS CategoryNames
                FROM Project p

                LEFT JOIN ProjectCategories pc ON pc.Project_id = p.Project_id

                LEFT JOIN Categories c ON c.Category_id = pc.Category_id
                LEFT JOIN TeamMembers tm ON tm.Project_id = p.Project_id
                WHERE (@Search IS NULL
                OR p.Title LIKE '%' + @Search + '%'
                OR p.Description LIKE '%' + @Search + '%')
                GROUP BY p.Project_id, p.Title, p.Description, p.Status, p.CreatedAt, p.MemberSize
                ORDER BY p.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*) FROM Project
                WHERE (@Search IS NULL
                OR Title LIKE '%' + @Search + '%'
                OR Description LIKE '%' + @Search + '%');";

            using var multi = await conn.QueryMultipleAsync(query, new
            { Offset = OFFSET, PageSize = pageSize, Search = search  }
            );
            var items = (await multi.ReadAsync<Project>()).ToList();
            var total = await multi.ReadFirstAsync<int>();

            return new PaginatedResult<Project>() { 
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
            };
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
                    new {
                        UserId = userId,
                        project.Title,
                        Desc = project.Description,
                        Size = project.MemberSize }, transaction);

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
                    ,transaction);

                string Role = "Testing"; // Todo : Make a role manager for owner of project
                // insert also the owwnerr
                string teamProject = @"INSERT INTO TeamMembers (Project_id, User_id, Role) VALUES (@ProjectId, @UserId, @Role)";
                await conn.ExecuteAsync(teamProject, 
                    new { UserId = userId, ProjectId = projectId, Role = Role}
                    
                    , transaction);

                transaction.Commit();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                transaction.Rollback();
            }
           
        }

        public Task DisposeProject(int ownerId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedResult<Project>> GetJoinedProjects(
            int userId, 
            int page, 
            int pageSize, 
            string search)
        {
            using var conn = _dapperContext.CreateConnection();
            var OFFSET = (page - 1) * pageSize;

            var query = @"
                SELECT p.Project_id, p.Title, p.Description, p.Status, p.CreatedAt,
                STRING_AGG(c.Category_name, ', ') AS CategoryNames,
                CONCAT(u.FirstName,' ', u.LastName) as OwnerName
                FROM Project p
                
                INNER JOIN TeamMembers tm ON tm.Project_id = p.Project_id
                LEFT JOIN ProjectCategories pc ON pc.Project_id = p.Project_id
                LEFT JOIN Categories c ON c.Category_id = pc.Category_id
                LEFT JOIN Users u ON u.User_id = p.Owner_id

                WHERE 
                tm.User_id = @UserId AND
                p.Owner_id != @UserId AND
                (@Search IS NULL
                OR p.Title LIKE '%' + @Search + '%'
                OR p.Description LIKE '%' + @Search + '%')
                GROUP BY p.Project_id, p.Title, p.Description, p.Status, p.CreatedAt, u.FirstName, u.LastName
                ORDER BY p.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(DISTINCT p.Project_id) 
                FROM Project p
                INNER JOIN TeamMembers tm ON tm.Project_id = p.Project_id
                
                WHERE tm.User_id = @UserId AND
                (@Search IS NULL
                OR Title LIKE '%' + @Search + '%'
                OR Description LIKE '%' + @Search + '%');";

            using var multi = await conn.QueryMultipleAsync(query, new
            { 
                //query paramterss
                Offset = OFFSET, 
                PageSize = pageSize, 
                Search = search,
                UserId = userId
            
            }
            );
            var items = (await multi.ReadAsync<Project>()).ToList();
            var total = await multi.ReadFirstAsync<int>();

            return new PaginatedResult<Project>()
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
            };  
        }

        public Task<PaginatedResult<Project>> GetOwnedProjects(int userId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<Project?> GetByIdAsync(int projectId)
        {
            using var conn = _dapperContext.CreateConnection();
            string query = @"
                SELECT p.Project_id, p.Title, p.Description, p.Status, p.CreatedAt, p.Owner_id,
                STRING_AGG(c.Category_name, ', ') AS CategoryNames,
                CONCAT(u.FirstName,' ', u.LastName) as OwnerName, 
                COUNT(DISTINCT tm.User_id) AS TotalMembers
                FROM Project p

                LEFT JOIN ProjectCategories pc ON pc.Project_id = p.Project_id
                LEFT JOIN Categories c ON c.Category_id = pc.Category_id
                LEFT JOIN TeamMembers tm ON tm.Project_id = p.Project_id
                LEFT JOIN Users u ON u.User_id = p.Owner_id 
                WHERE p.Project_id = @ProjectId
                GROUP BY p.Project_id, p.Title, p.Description, p.Status, p.CreatedAt, p.Owner_id,
                u.FirstName, u.LastName";
            
            return await conn.QueryFirstOrDefaultAsync<Project>(query, new { ProjectId = projectId });

        }

        public async Task<List<TeamMembers>> GetProjectTeamMembers(int projectId)
        {
            using var conn = _dapperContext.CreateConnection();
            conn.Open();

            string query = @"
            SELECT u.User_id AS User_id,
                   u.Email AS Email,
                   CONCAT(u.FirstName,' ', u.LastName) AS Fullname
            FROM TeamMembers AS tm
            INNER JOIN Users u ON u.User_id = tm.User_id
            WHERE tm.Project_id = @ProjectId";

            return (await conn.QueryAsync<TeamMembers>(query,
                new { ProjectId = projectId })).ToList();

        }

        public async Task<List<Categories>> GetAllAvailableCategories()
        {
            using var conn = _dapperContext.CreateConnection();
            string query = @"SELECT * FROM Categories";

            return (await conn.QueryAsync<Categories>(query)).ToList();

        }

        public async Task<List<Skills>> GetAllAvailableSkills() 
        {
            using var conn = _dapperContext.CreateConnection();
            string query = @"SELECT * FROM Skills";

            return (await conn.QueryAsync<Skills>(query)).ToList();
        }
    }
}
