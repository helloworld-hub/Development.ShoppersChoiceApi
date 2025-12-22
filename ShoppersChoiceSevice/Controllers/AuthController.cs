using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppersChoice.API.Models;
using ShoppersChoice.Entities.DTOs;
using ShoppersChoice.Utilities.Authentication_Authorization;
using ShoppersChoiceSevice.Authentication_Authorization;
using ShoppersChoiceSevice.MySQLDbContext;

namespace ShoppersChoiceSevice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MySQLDBContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(MySQLDBContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // POST: auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto dto)
        {
            try
            {
                //Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                {
                    return BadRequest(new { message = "Email already registered" });
                }

                // Hash password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // Create new user
                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create cart for user
                var cart = new Cart
                {
                    UserId = user.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                // Generate JWT token
                string token = _tokenService.GenerateToken(user);

                // Return response
                var response = new AuthResponseDto
                {
                    User = new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Token = token
                    },
                    Token = token,
                    Message = "Registration successful"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        //POST: auth/login
       [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
        {
            try
            {
                // Find user by email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Verify password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

                if (!isPasswordValid)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Update last login
                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();

                // Generate JWT token
                string token = _tokenService.GenerateToken(user);

                // Return response
                var response = new AuthResponseDto
                {
                    User = new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Token = token
                    },
                    Token = token,
                    Message = "Login successful"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        // GET: api/auth/me
        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                // Get user ID from JWT token
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}
