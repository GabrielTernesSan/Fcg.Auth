using Fcg.Auth.Domain.Queries;
using Fcg.Auth.Domain.Queries.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Auth.Infra.Queries
{
    public class AuthQuery : IAuthQuery
    {
        private readonly FcgAuthDbContext _context;

        public AuthQuery(FcgAuthDbContext context)
        {
            _context = context;
        }

        public async Task<ObterEmailByUserResponse?> GetEmailByUserIdAsync(Guid id)
        {
            return await (from u in _context.Users
                          where u.Id == id
                          select new ObterEmailByUserResponse()
                          {
                              Id = u.Id,
                              Email = u.Email
                          }).FirstOrDefaultAsync();
        }
    }
}
