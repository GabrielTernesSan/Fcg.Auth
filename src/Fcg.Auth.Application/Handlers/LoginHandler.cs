using Fcg.Auth.Application.Requests;
using Fcg.Auth.Common;
using Fcg.Auth.Domain.Repositories;
using Fcg.Auth.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Auth.Application.Handlers
{
    public class LoginHandler : IRequestHandler<LoginRequest, Response>
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

        public async Task<Response> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null || !_passwordHasherService.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning($"Tentativa de login inválida para o e-mail: {request.Email}");
                response.AddError($"Tentativa de login inválida para o e-mail: {request.Email}");

                return response;
            }

            var token = _jwtTokenService.GenerateToken(user.Email, user.Role);

            _logger.LogInformation("Usuário {Email} logado com sucesso.", user.Email);

            // TODO: Ver como retorno esse cara
            return new LoginResponse
            {
                Success = true,
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Message = "Login realizado com sucesso!"
            };
        }
    }
}
