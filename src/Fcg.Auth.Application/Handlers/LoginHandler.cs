using Fcg.Auth.Application.Requests;
using Fcg.Auth.Common;
using Fcg.Auth.Domain.Repositories;
using Fcg.Auth.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Auth.Application.Handlers
{
    public class LoginHandler : IRequestHandler<LoginRequest, Response<string>>
    {
        private readonly IAuthUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(IAuthUserRepository userRepository, IPasswordHasherService passwordHasherService, ILogger<LoginHandler> logger, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<string>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var response = new Response<string>();

            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null || !_passwordHasherService.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning($"Tentativa de login inválida para o e-mail: {request.Email}");
                response.AddError($"Tentativa de login inválida para o e-mail: {request.Email}");

                return response;
            }

            response.Result = _jwtTokenService.GenerateToken(user);

            _logger.LogInformation("Usuário {Email} logado com sucesso.", user.Email);

            return response;
        }
    }
}
