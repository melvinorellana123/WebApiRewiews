    using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiAutores.Dtos;
using WebApiAutores.Dtos.Auth;
using WebApiAutores.Helpers;
using WebApiAutores.Services;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace WebApiAutores.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSenderService _emailSenderService;

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,
            IConfiguration configuration, IEmailSenderService mailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _emailSenderService = mailService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Login(LoginDto dto)
        {
            var result = await _signInManager
                .PasswordSignInAsync(dto.Email, dto.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                
                //generar el token de autenticacion
                //Crear claims
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),//para que sea unico
                    new Claim("UserId", user.Id)
                };
                
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));//para agregar los roles
                }
                //Generar el token
                var jwtToken = GetToken(authClaims);
                
                var loginResponseDto = new LoginResponseDto
                {
                    Email = user.Email,
                    FullName = "",
                    TokenExpiration = jwtToken.ValidTo,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken)//para que tradusca el token a string
                };
                await _emailSenderService.SendEmailAsync(user.Email, "WebApiAutores Inicio de sesión", 
                    EmailTemplates.LoginTemplate(user.Email));
                return Ok(new ResponseDto<LoginResponseDto>
                {
                    Status = true,
                    Message = "Login exitoso",
                    Data = loginResponseDto
                });
            }

            return StatusCode(StatusCodes.Status401Unauthorized, new ResponseDto<LoginResponseDto>
            {
                Status = false,
                Message = "La autenticacion ha fallado"
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseDto<object>>> RegisterUser(RegisterUserDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is not null)
            {
                return BadRequest(new ResponseDto<object>
                {
                    Status = false,
                    Message = $"El usuario con el correo {dto.Email} ya existe"
                });
            }

            var identityUser = new IdentityUser
            {
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ResponseDto<object>
                {
                    Status = false,
                    Message = "No se pudo crear el usuario",
                    Data = result.Errors
                });
            }
            
            return Ok(new ResponseDto<object>
            {
                Status = true,
                Message = "Usuario creado exitosamente"
            });
        }
        
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));// verificar la integridad del token
            
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
            );// crear el token
            
            return token;
        }
    }
}