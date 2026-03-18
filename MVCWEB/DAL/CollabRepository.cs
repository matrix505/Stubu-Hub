using Dapper;
using MVCWEB.DAL.Abstract;
using MVCWEB.Data;
using MVCWEB.Models;
using MVCWEB.Models.Entities;

namespace MVCWEB.DAL
{
    public class CollabRepository : ICollabRepository
    {
        private readonly DapperContext _dapperContext;
        private readonly ILogger<CollabRepository> _logger;
        public CollabRepository(
            DapperContext dapper,
            ILogger<CollabRepository> logger
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

        public async Task<PaginatedResult<Project>> GetOwnedProjects(int userId, int page, int pageSize, string? search)
        {
            using var conn = _dapperContext.CreateConnection();
            var OFFSET = (page - 1) * pageSize;

            var query = @"
            SELECT  p.Project_id, p.Title, p.Description, p.Status, p.CreatedAt, p.MemberSize,
            COUNT(DISTINCT tm.User_id) AS TotalMembers,
            STRING_AGG(c.Category_name, ', ') AS CategoryNames
            FROM Project p
            LEFT JOIN ProjectCategories pc ON pc.Project_id = p.Project_id
            LEFT JOIN Categories c ON c.Category_id = pc.Category_id
            LEFT JOIN TeamMembers tm ON tm.Project_id = p.Project_id
            WHERE  p.Owner_id = @UserId
            AND (@Search IS NULL OR p.Title LIKE '%' + @Search + '%' OR p.Description LIKE '%' + @Search + '%')
            GROUP BY  p.Project_id, p.Title, p.Description, p.Status, p.CreatedAt, p.MemberSize
            ORDER BY p.CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            SELECT COUNT(*) 
            FROM Project p
            WHERE  p.Owner_id = @UserId
            AND (@Search IS NULL OR p.Title LIKE '%' + @Search + '%' OR p.Description LIKE '%' + @Search + '%');";

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

        public async Task<Project> GetByIdAsync(int projectId)
        {
           try
            {
                using var conn = _dapperContext.CreateConnection();
                string query = @"
                SELECT p.Project_id, p.Title, p.Description,  p.Status,  p.CreatedAt, p.Owner_id, p.MemberSize, 
                STRING_AGG(c.Category_name, ', ') AS CategoryNames,
                CONCAT(u.FirstName,' ', u.LastName) AS OwnerName, 
                COUNT(DISTINCT tm.User_id) AS TotalMembers
                FROM Project p
                LEFT JOIN ProjectCategories pc ON pc.Project_id = p.Project_id
                LEFT JOIN Categories c ON c.Category_id = pc.Category_id
                LEFT JOIN TeamMembers tm ON tm.Project_id = p.Project_id
                LEFT JOIN Users u ON u.User_id = p.Owner_id 
                WHERE p.Project_id = @ProjectId
                GROUP BY p.Project_id,  p.Title,  p.Description,   p.Status,  p.CreatedAt,  p.Owner_id,
                p.MemberSize,  u.FirstName,  u.LastName";

                return await conn.QueryFirstOrDefaultAsync<Project>(query, new { ProjectId = projectId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

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
