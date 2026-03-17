using MVCWEB.Models.Entities;

namespace MVCWEB.DAL.Abstract
{
    public interface IAccountRepository 
    {
        Task<int> CreateUserAsync(Users User);
        Task<bool> EmailExistsAsync(string Email);
        Task<bool> UsernameExistsAsync(string Username);
        Task<Users?> FindByUsernameAsync(string Username);
        Task<Users?> FindByEmailAsync(string Email);
        Task<bool> IsEmailVerified(string Email); 
        
        Task SaveResetTokenAsync(string email, string token, DateTime expiry); 
        Task ClearResetTokenAsync(string user_id);
    }
}
