namespace Fcg.Auth.Domain.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
