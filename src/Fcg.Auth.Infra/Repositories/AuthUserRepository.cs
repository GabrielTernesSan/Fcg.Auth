using Fcg.Auth.Domain;
using Fcg.Auth.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Auth.Infra.Repositories
{
    public class AuthUserRepository : IAuthUserRepository
    {
        private readonly FcgAuthDbContext _context;

        public AuthUserRepository(FcgAuthDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateUserAsync(User user)
        {
            var entity = new Tables.User
            {
                Id = user.Id,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var entity = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            return entity is null
                ? null
                : new User(entity.Id, entity.Email, entity.PasswordHash, entity.Role);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            var entity = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            return entity is null
                ? null
                : new User(entity.Id, entity.Email, entity.PasswordHash, entity.Role);
        }

        public async Task UpdateUserRoleAsync(User user)
        {
            var entity = await _context.Users.FindAsync(user.Id)
                ?? throw new InvalidOperationException(
                    $"Usuário com Id {user.Id} não encontrado para atualizar a politica de permissões.");

            entity.Role = user.Role;

            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
