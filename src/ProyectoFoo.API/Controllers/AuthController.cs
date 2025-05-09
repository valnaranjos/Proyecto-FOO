﻿using Microsoft.AspNetCore.Mvc;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Services;
using ProyectoFoo.API.Models.Authentication;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using ProyectoFoo.Application.Features.Login;

namespace ProyectoFoo.API.Controllers
{
    /// <summary>
    /// Autentica a un usuario y devuelve un token JWT si las credenciales son válidas.
    /// </summary>
    /// <param name="model">Objeto JSON que contiene el correo electrónico y la contraseña del usuario.</param>
    /// <returns>
    /// Si las credenciales son correctas, devuelve un código de estado 200 OK con el token JWT.
    /// Si las credenciales son incorrectas, devuelve un código de estado 401 Unauthorized.
    /// </returns>
    /// <response code="200">Credenciales válidas. Devuelve el token JWT.</response>
    /// <response code="401">Credenciales inválidas.</response>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IMediator _mediator;

        public AuthController(IUserRepository usuarioRepository, ITokenService tokenService, IMediator mediator)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Autentica a un usuario y devuelve un token JWT si las credenciales son válidas.
        /// </summary>
        /// <param name="model">Objeto JSON que contiene el correo electrónico y la contraseña del usuario.</param>
        /// <returns>
        /// Si las credenciales son correctas, devuelve un código de estado 200 OK con el token JWT.
        /// Si las credenciales son incorrectas, devuelve un código de estado 401 Unauthorized.
        /// </returns>
        /// <response code="200">Credenciales válidas. Devuelve el token JWT.</response>
        /// <response code="401">Credenciales inválidas.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(model.Email);

            if (usuario == null || !usuario.VerifyPassword(model.Password))
            {
                return Unauthorized("Credenciales inválidas.");
            }

            // Generar el token utilizando el TokenService
            var token = _tokenService.GenerateToken(usuario);

            // Actualizar la fecha del último acceso
            usuario.ActualizeLastAcces();
            await _usuarioRepository.UpdateAsync(usuario);

            return Ok(new { Token = token });
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);
            if (result == null || !result.Success)
            {
                return BadRequest("No se pudo restablecer la contraseña. Verifique el token y el email.");
            }

            return Ok(result.Message);
        }

        [HttpPost("verify-password-reset-code")]
        public async Task<IActionResult> VerifyPasswordResetCode([FromBody] VerifyPasswordResetCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _mediator.Send(command);
            if (!response.Success)
            {
                return BadRequest("No se pudo restablecer la contraseña. Verifique el token y el email.");
            }

            return Ok("Contraseña restablecida exitosamente.");
        }
    }    
}