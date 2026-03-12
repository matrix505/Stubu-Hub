using MVCWEB.Services.Abstract;
using Isopoh.Cryptography.Argon2;
namespace MVCWEB.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashedPassword(string password)
        {
            return Argon2.Hash(password);
        }

        public bool VerifyPassword(string password, string HashedPassword)
        {
            var result = Argon2.Verify(HashedPassword, password);
            return result;
        }
    }
}
