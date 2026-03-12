
namespace MVCWEB.Services.Abstract

{
    public interface IPasswordHasher
    {
        string HashedPassword(string password);
        bool VerifyPassword(string password,string HashedPassword);
    }
}
