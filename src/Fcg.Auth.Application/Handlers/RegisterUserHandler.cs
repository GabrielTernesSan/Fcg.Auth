using Fcg.Auth.Application.Requests;
using Fcg.Auth.Common;
using Fcg.Auth.Domain;
using Fcg.Auth.Domain.Repositories;
using Fcg.Auth.Domain.Services;
using Fcg.Auth.Proxy.User.Client.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Auth.Application.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, Response>
    {
        private readonly IAuthUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IClientUser _clientUser;
        private readonly ILogger<RegisterUserHandler> _logger;

        public RegisterUserHandler(IAuthUserRepository userRepository, IPasswordHasherService passwordHasherService, ILogger<RegisterUserHandler> logger, IClientUser clientUser)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _logger = logger;
            _clientUser = clientUser;
        }

        public async Task<Response> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user != null)
            {
                _logger.LogWarning($"Tentativa de criar usuário com e-mail já existente: {request.Email}");
                response.AddError($"Tentativa de criar usuário com e-mail já existente: {request.Email}");

                return response;
            }

            user = new User(request.Email);
            user.SetPasswordHash(_passwordHasherService.Hash(request.Password));

            await _userRepository.CreateUserAsync(user);

            _logger.LogInformation("Usuário criado com sucesso: {Email}, ID: {Id}", user.Email, user.Id);

            var externalResponse = await _clientUser.CreateUserAsync(user.Id);

            if (externalResponse.HasErrors)
            {
                _logger.LogWarning("Erro ao criar usuário externo: {Erros}", string.Join(", ", externalResponse.Erros));
                response.AddError("Erro ao criar usuário externo.");
            }

            return response;
        }
    }
}
