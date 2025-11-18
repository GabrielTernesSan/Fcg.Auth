using Fcg.Auth.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace Fcg.Auth.Application.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string Hash(string senha) =>
            _hasher.HashPassword(new object(), senha);

        public bool Verify(string senha, string hash) =>
            _hasher.VerifyHashedPassword(new object(), hash, senha) == PasswordVerificationResult.Success;
    }
}
