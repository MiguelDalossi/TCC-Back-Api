using System.Security.Claims;
using System.Threading.Tasks;
using ConsultorioMedico.Api.Dtos;
using ConsultorioMedico.Api.Models;
using ConsultorioMedico.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConsultorioMedico.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userMgr;
        private readonly SignInManager<AppUser> _signInMgr;
        private readonly TokenService _tokenSvc;

        public AuthController(UserManager<AppUser> userMgr, SignInManager<AppUser> signInMgr, TokenService tokenSvc)
        {
            _userMgr = userMgr;
            _signInMgr = signInMgr;
            _tokenSvc = tokenSvc;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResultDto>> Login(LoginDto dto)
        {
            var user = await _userMgr.FindByEmailAsync(dto.Email);
            if (user is null) return Unauthorized("Usuário ou senha inválidos.");

            var check = await _signInMgr.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!check.Succeeded) return Unauthorized("Usuário ou senha inválidos.");

            var roles = await _userMgr.GetRolesAsync(user);
            var (token, expires) = _tokenSvc.Generate(user, roles);


            var primaryRole = roles.Count > 0 ? roles[0] : "";

            return new AuthResultDto
            {
                Token = token,
                ExpiresAt = expires,
                FullName = user.FullName ?? user.UserName ?? "",
                Email = user.Email ?? "",
                Role = primaryRole
            };
        }


        [HttpPost("register")]
        [AllowAnonymous] // em produção, restrinja com [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto,
            [FromServices] UserManager<AppUser> userMgr,
            [FromServices] RoleManager<IdentityRole<Guid>> roleMgr)
        {
            var role = string.IsNullOrWhiteSpace(dto.Role) ? "Recepcao" : dto.Role;

            if (!await roleMgr.RoleExistsAsync(role))
                await roleMgr.CreateAsync(new IdentityRole<Guid>(role));

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
                FullName = dto.FullName
            };

            var result = await userMgr.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            await userMgr.AddToRoleAsync(user, role);
            return StatusCode(201);
        }




        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> Me()
        {
            return new
            {
                Name = User.Identity?.Name,
                Roles = User.Claims
                             .Where(c => c.Type == ClaimTypes.Role)
                             .Select(c => c.Value)
                             .ToList()
            };
        }
    }
}