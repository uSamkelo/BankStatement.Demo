using BankStatement.Demo.Models;
using BankStatement.Demo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankStatement.Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(AuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = await authService.RegisterAsync(request);
            if (user == null) return BadRequest("Username already exists");
            
            return Ok(new { user.Id, user.Username });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var response = await authService.RefreshTokenAsync(request.RefreshToken);
            if (response == null)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            return Ok(response);
        }

        //testing purposes
        [Authorize]
        [HttpGet("protect")]
        public IActionResult Protected()
        {
            var username = User.Identity.Name;
            return Ok($"Hello, {username}! This is a protected endpoint");
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest request)
        {
            var success = await authService.RevokeRefreshTokenAsync(request.RefreshToken);
            if (!success)
            {
                return BadRequest("Invalid or already revoked refresh token.");
            }

            return Ok("Refresh token revoked.");
        }
    }
}
