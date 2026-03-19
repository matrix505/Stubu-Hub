using Dapper;
using Microsoft.Data.SqlClient;
using MVCWEB.DAL.Abstract;
using MVCWEB.Data;
using MVCWEB.Models.Entities;

namespace MVCWEB.DAL
{
    public class DiscussionRepository : IDiscussionRepository
    {
        private readonly DapperContext _dapperContext;
        public DiscussionRepository(
            DapperContext dapperContext
            ) {
            _dapperContext = dapperContext;
        }

        public async Task<bool> CreateDiscussion(Discussions ds)
        {
            using var conn = _dapperContext.CreateConnection();
            const string sql = @"
                INSERT INTO DiscussionTopics (Project_id, Title, Description, Creator_id, IsClosed)
                VALUES (@Project_id, @Title, @Description, @Creator_id, @IsClosed)
                ";
            var parameters = new
            {
                ds.Project_id,
                ds.Title,
                ds.Description,
                ds.Creator_id,
                IsClosed = false,
                CreatedAt = DateTime.UtcNow
            };

            int rowsAffected = await conn.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<List<Discussions>?> GetAllDiscussions(int ProjectId)
        {
            using var conn = _dapperContext.CreateConnection();
            const string sql = @"
                    SELECT 
                        dt.Topic_id,
                        dt.Project_id,
                        dt.Title,
                        dt.Description,
                        dt.Creator_id,
                        dt.CreatedAt,
                        CONCAT(u.FirstName,' ',u.LastName) AS CreatorName
                    FROM DiscussionTopics dt
                    INNER JOIN Users u ON u.User_id = dt.Creator_id
                    WHERE dt.Project_id = @ProjectId
                    ORDER BY dt.CreatedAt DESC";

            var result = await conn.QueryAsync<Discussions>(sql, new { ProjectId });
            return result.ToList();
        }
        public async Task<Discussions?> GetDiscussionById(int? TopicId)
        {
            using var conn = _dapperContext.CreateConnection();
            const string sql = @"
                    SELECT 
                        dt.Topic_id,
                        dt.Project_id,
                        dt.Title,
                        dt.Description,
                        dt.Creator_id,
                        dt.IsClosed,
                        dt.CreatedAt,
                        CONCAT(u.FirstName,' ',u.LastName) AS CreatorName,
                        p.Title as ProjectTitle

                        FROM DiscussionTopics dt
                        INNER JOIN Users u ON u.User_id = dt.Creator_id
                        INNER JOIN Project p on p.Project_id = dt.Project_id 
                        WHERE dt.Topic_id = @TopicId";
            var result = await conn.QueryFirstOrDefaultAsync<Discussions>(sql, new { TopicId });
            return result;

        }

        public Task<List<TopicMessages>?> GetTopicMessages(int TopicId)
        {
            throw new NotImplementedException();
        }
    }
}
