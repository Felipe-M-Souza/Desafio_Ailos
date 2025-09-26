using System;
using System.Threading.Tasks;
using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Constants;
using ContaCorrente.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContaCorrente.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Realiza login na conta corrente
        /// </summary>
        /// <param name="request">Credenciais de login</param>
        /// <returns>Token de autenticação</returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var command = new LoginCommand(request.Numero, request.Cpf, request.Senha);
                var result = await _mediator.Send(command);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(
                    new ErrorResponse { Error = ex.Message, Code = ErrorCodes.USER_UNAUTHORIZED }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new ErrorResponse { Error = ex.Message, Code = ErrorCodes.DADOS_INVALIDOS }
                );
            }
        }
    }
}

