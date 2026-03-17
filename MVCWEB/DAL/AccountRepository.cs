using Dapper;

using MVCWEB.DAL.Abstract;
using MVCWEB.Data;
using MVCWEB.Models.Entities;
using MVCWEB.Services.Abstract;
using MVCWEB.Services.Cache;

namespace MVCWEB.DAL
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DapperContext _dbContext;
        private readonly ICacheService _cache;
        
        public AccountRepository(
            DapperContext dapper,
            ICacheService cacheService
            )
        {
            _dbContext = dapper;
            _cache = cacheService;
        }
        public async Task<int> CreateUserAsync(Users User)
        {
            using var conn = _dbContext.CreateConnection();
            conn.Open();

            string query = @"
                INSERT INTO Users 
                (
                    FirstName,LastName,Username,Email,
                    HashedPassword,BirthDate,Country
                ) VALUES 
                (
                    @FirstName, @LastName, @Username, @Email, @HashedPassword, @BirthDate, @Country
                )";

            return await conn.ExecuteAsync(query, User);
        }
        public async Task<bool> EmailExistsAsync(string Email)
        {

            return await _cache.GetOrCreateAsync(
                key: CacheKeys.EmailExists(Email),
                factory:
                async () =>
                {
                    //Query 
                    using var conn = _dbContext.CreateConnection();
                    conn.Open();
                    string query = @"SELECT COUNT(1) FROM Users WHERE Email = @Email";
                    return await conn.ExecuteScalarAsync<bool>(query, new { Email = Email });

                },
                expiration: TimeSpan.FromMinutes(10)
                );
        }
        public async Task<bool> UsernameExistsAsync(string Username)
        {
            using var conn = _dbContext.CreateConnection();
            conn.Open();

            string query = @"SELECT COUNT(1) FROM Users WHERE Username = @Username";
            return await conn.ExecuteScalarAsync<bool>(query, new { Username = Username });
        }
        public async Task<Users?> FindByUsernameAsync(string Username)
        {
            using var conn = _dbContext.CreateConnection();
            conn.Open();

            string query = @"
                SELECT User_id,FirstName,LastName,HashedPassword, Email, AuthorizationRole 
                FROM Users WHERE Username = @Username ";
            return await conn.QueryFirstOrDefaultAsync<Users>(
                query,
                new { Username = Username }
                );
        }
        public async Task<Users?> FindByEmailAsync(string Email)
        {
            using var conn = _dbContext.CreateConnection();
            conn.Open();

            string query = @"
                SELECT User_id,FirstName,LastName,HashedPassword, Email, AuthorizationRole 
                FROM Users WHERE Email = @Email ";
            return await conn.QueryFirstOrDefaultAsync<Users>(
                query,
                new { Email = Email }
                );
        }

        public Task<bool> IsEmailVerified(string Email)
        {
            throw new NotImplementedException();
        }

        public Task SaveResetTokenAsync(string email, string token, DateTime expiry)
        {
            throw new NotImplementedException();
        }

        public Task ClearResetTokenAsync(string user_id)
        {
            throw new NotImplementedException();
        }
    }
}
