namespace Fcg.Auth.Domain.Queries.Responses
{
    public class ObterEmailByUserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
    }
}
