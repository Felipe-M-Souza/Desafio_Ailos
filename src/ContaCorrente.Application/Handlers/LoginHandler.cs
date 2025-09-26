using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Constants;
using ContaCorrente.Application.DTOs;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Handlers
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IConfiguration _configuration;

        public LoginHandler(IContaCorrenteRepository contaRepository, IConfiguration configuration)
        {
            _contaRepository = contaRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Buscar conta por número ou CPF
            Conta conta;
            
            if (request.Numero.HasValue)
            {
                conta = await _contaRepository.ObterPorNumeroAsync(request.Numero.Value);
            }
            else if (!string.IsNullOrEmpty(request.Cpf))
            {
                conta = await _contaRepository.ObterPorCpfAsync(request.Cpf);
            }
            else
            {
                throw new UnauthorizedAccessException(ErrorMessages.USER_UNAUTHORIZED);
            }
            
            if (conta == null)
            {
                throw new UnauthorizedAccessException(ErrorMessages.USER_UNAUTHORIZED);
            }

            // Verificar senha
            var senhaValida = BCrypt.Net.BCrypt.Verify(request.Senha, conta.Senha);
            if (!senhaValida)
            {
                throw new UnauthorizedAccessException(ErrorMessages.USER_UNAUTHORIZED);
            }

            // Gerar token JWT (permitir login mesmo com conta inativa)
            var token = GerarToken(conta);

            return new LoginResponse(token, conta.IdContaCorrente);
        }

        private string GerarToken(Domain.Entities.Conta conta)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "super_secret_key_here_change";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ContaCorrente";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "ContaCorrente";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, conta.IdContaCorrente),
                new Claim(ClaimTypes.Name, conta.Nome),
                new Claim("numero", conta.Numero.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
