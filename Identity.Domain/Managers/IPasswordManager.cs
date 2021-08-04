namespace Identity.Domain.Managers
{
    public interface IPasswordManager
    {
        string HashPassword(string password);

        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}