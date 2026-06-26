using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanySystemAPI.Data;
using CompanySystemAPI.Models;
using CompanySystemAPI.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // ===================== REGISTER =====================
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var exists = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (exists != null)
            return BadRequest("Email already exists");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Employee"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Role
        });
    }

    // ===================== LOGIN =====================
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null)
            return Unauthorized("Invalid username or password");

        bool validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

        if (!validPassword)
            return Unauthorized("Invalid username or password");

        // ===================== EMPLOYEE SYNC =====================
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Name == user.Username);

        if (employee == null)
        {
            employee = new Employee
            {
                Name = user.Username,
                EmployeeNumber = "EMP-" + Guid.NewGuid().ToString("N")[..6]
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        // ===================== JWT =====================
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username ?? ""),
            new Claim(ClaimTypes.Role, user.Role ?? "")
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = jwt,
            username = user.Username,
            role = user.Role,
            employeeNumber = employee.EmployeeNumber   // 🔥 مهم جداً
        });
    }
}