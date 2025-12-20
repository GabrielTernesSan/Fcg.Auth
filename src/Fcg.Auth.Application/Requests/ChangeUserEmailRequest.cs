using Fcg.Auth.Application.Responses;
using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;

namespace Fcg.Auth.Application.Requests
{
    public class ChangeUserEmailRequest : IRequest<ExternalResponse>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string Email { get; set; }
    }

    public class ChangeUserEmailRequestValidator : AbstractValidator<ChangeUserEmailRequest>
    {
        public ChangeUserEmailRequestValidator()
        {
            RuleFor(p => p.Email).EmailAddress().NotNull().WithMessage("Email inválido!");
        }
    }
}
