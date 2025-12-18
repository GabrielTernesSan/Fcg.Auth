using Fcg.Auth.Domain.Queries.Responses;

namespace Fcg.Auth.Domain.Queries
{
    public interface IAuthQuery
    {
        Task<ObterEmailByUserResponse?> GetEmailByUserIdAsync(Guid Id);
    }
}
