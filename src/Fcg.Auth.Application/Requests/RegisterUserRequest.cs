using Fcg.Auth.Common;
using FluentValidation;
using MediatR;

namespace Fcg.Auth.Application.Requests
{
    public class RegisterUserRequest : IRequest<Response>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.Email).NotNull()
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("Formato de e-mail inválido.");

            RuleFor(x => x.Password).NotNull()
                .NotEmpty().WithMessage("A senha é obrigatória.");
        }
    }
}
