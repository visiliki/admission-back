using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SchoolAdmission.DTOs;
using SchoolAdmission.Models;
using SchoolAdmission.Data;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly SchoolAdmissionDbContext db;

    public AdminController(SchoolAdmissionDbContext context)
    {
        db = context;
    }

    
    private string GetCurrentAdminEmail()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
    }

    [HttpGet("students")]
    public IActionResult GetAllStudents()
    {
        try
        {
            var students = db.Students.ToList();
            if (students.Count == 0)
                return NotFound("No students found in the system.");
            return Ok(students);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving students: {ex.Message}");
        }
    }

    [HttpGet("students/filter")]
    public IActionResult FilterStudents([FromQuery] string? name, [FromQuery] string? nationalId)
    {
        try
        {
            var query = db.Students.AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(s => s.FullName.Contains(name));
            if (!string.IsNullOrEmpty(nationalId))
                query = query.Where(s => s.NationalId == nationalId);
            var result = query.ToList();
            if (result.Count == 0)
                return NotFound("No students match the provided filter criteria.");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while filtering students: {ex.Message}");
        }
    }

    // Admin: Get my interview score for a student
    [Authorize]
    [HttpGet("student/{studentId}/my-interview-score")]
    public IActionResult GetMyInterviewScore(int studentId)
    {
        try
        {
            var userEmail = GetCurrentAdminEmail();
            var admin = db.Admins.FirstOrDefault(a => a.Email == userEmail);
            if (admin == null)
                return Unauthorized("Admin not found or not authorized. Please log in again.");
            if (admin.Role.ToLower() != "admin")
                return Forbid("Only admins can view their own interview score.");
            var student = db.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
                return NotFound($"Student with ID {studentId} not found.");
            var score = db.InterviewScores.FirstOrDefault(s => s.StudentId == studentId && s.AdminId == admin.Id);
            return Ok(new { Score = score?.Score, Editable = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving your interview score: {ex.Message}");
        }
    }

    // Requires admin role
    [Authorize]
    [HttpPost("student/{studentId}/my-interview-score")]
    public IActionResult SetMyInterviewScore(int studentId, [FromBody] double scoreValue)
    {
        try
        {
            var userEmail = GetCurrentAdminEmail();
            var admin = db.Admins.FirstOrDefault(a => a.Email == userEmail);
            if (admin == null)
                return Unauthorized("Admin not found or not authorized. Please log in again.");
            if (admin.Role.ToLower() != "admin")
                return Forbid("Only admins can set or edit their own interview score.");
            if (scoreValue < 0 || scoreValue > 40)
                return BadRequest("Interview score must be between 0 and 40.");
            var student = db.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
                return NotFound($"Student with ID {studentId} not found.");
            var interviewScore = db.InterviewScores.FirstOrDefault(s => s.StudentId == studentId && s.AdminId == admin.Id);
            if (interviewScore == null)
            {
                interviewScore = new InterviewScore { StudentId = studentId, AdminId = admin.Id, Score = scoreValue };
                db.InterviewScores.Add(interviewScore);
            }
            else
            {
                interviewScore.Score = scoreValue;
            }
            db.SaveChanges();
            var allScores = db.InterviewScores.Where(s => s.StudentId == studentId).Sum(s => s.Score);
            var percentage = (allScores / 120.0) * 100;
            return Ok(new { Success = true, Message = "Interview score submitted successfully.", TotalPercentage = percentage });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while submitting your interview score: {ex.Message}");
        }
    }


    [Authorize]
    [HttpGet("student/{studentId}/all-interview-scores")]
    public IActionResult GetAllInterviewScores(int studentId)
    {
        try
        {
            var userEmail = GetCurrentAdminEmail();
            var admin = db.Admins.FirstOrDefault(a => a.Email == userEmail);
            if (admin == null)
                return Unauthorized("Admin not found or not authorized. Please log in again.");
            if (admin.Role.ToLower() != "superadmin")
                return Forbid("Only superadmins can view all interview scores.");
            var student = db.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
                return NotFound($"Student with ID {studentId} not found.");
            var scores = db.InterviewScores
                .Where(s => s.StudentId == studentId)
                .Select(s => new { s.Admin.FullName, s.Score })
                .ToList();
            if (scores.Count == 0)
                return NotFound("No interview scores found for this student.");
            return Ok(scores);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving interview scores: {ex.Message}");
        }
    }

    
    [Authorize]
    [HttpGet("student/{studentId}/total-percentage")]
    public IActionResult GetTotalPercentage(int studentId)
    {
        try
        {
            var userEmail = GetCurrentAdminEmail();
            var admin = db.Admins.FirstOrDefault(a => a.Email == userEmail);
            if (admin == null)
                return Unauthorized("Admin not found or not authorized. Please log in again.");
            if (admin.Role.ToLower() != "superadmin")
                return Forbid("Only superadmins can view the total percentage.");
            var student = db.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
                return NotFound($"Student with ID {studentId} not found.");
            var allScores = db.InterviewScores.Where(s => s.StudentId == studentId).Sum(s => s.Score);
            var percentage = (allScores / 120.0) * 100;
            return Ok(new { TotalPercentage = percentage });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while calculating the total percentage: {ex.Message}");
        }
    }
}
