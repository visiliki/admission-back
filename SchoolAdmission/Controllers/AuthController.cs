using Microsoft.AspNetCore.Mvc;
using SchoolAdmission.DTOs;
using SchoolAdmission.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SchoolAdmissionDbContext db;
    private readonly IConfiguration _configuration;

    public AuthController(SchoolAdmissionDbContext context, IConfiguration configuration)
    {
        db = context;
        _configuration = configuration;
    }

    [HttpPost("teacher/login")]
    public IActionResult TeacherLogin([FromBody] TeacherLoginDTO teacher)
    {
        if (teacher.Email == null || teacher.Password == null)
            return BadRequest("Email and password are required");

        var dbTeacher = db.Teachers.FirstOrDefault(t => t.Email == teacher.Email);
        if (dbTeacher == null || !BCrypt.Net.BCrypt.Verify(teacher.Password, dbTeacher.PasswordHash))
            return BadRequest("Invalid email or password");

        var token = GenerateJwtToken(dbTeacher.Email, "Teacher");
        return Ok(new { token });
    }

    [HttpPost("admin/login")]
    public IActionResult AdminLogin([FromBody] AdminLoginDTO admin)
    {
        if (admin.Email == null || admin.Password == null)
            return BadRequest("Email and password are required");

        var dbAdmin = db.Admins.FirstOrDefault(a => a.Email == admin.Email);
        if (dbAdmin == null || !BCrypt.Net.BCrypt.Verify(admin.Password, dbAdmin.PasswordHash))
            return BadRequest("Invalid email or password");

        var token = GenerateJwtToken(dbAdmin.Email, "Admin");
        return Ok(new { token });
    }

    private string GenerateJwtToken(string email, string role)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtIssuer,
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
