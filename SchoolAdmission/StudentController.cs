using Microsoft.AspNetCore.Mvc;
using SchoolAdmission.DTOs;
using SchoolAdmission.Models;
using SchoolAdmission.Data;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly SchoolAdmissionDbContext db;

    public StudentController(SchoolAdmissionDbContext context)
    {
        db = context;
    }

   
}
[HttpGet("check-national-id/{nationalId}")]
public async Task<ActionResult<Student>> GetStudentByNationalId(string nationalId)
{
    var student = await db.Students.FirstOrDefaultAsync(s => s.NationalId == nationalId);

    if (student == null)
        return NotFound(new { message = "Student not found" });

    return Ok(student);
}
[HttpPut("complete-info")]
public async Task<IActionResult> UpdateStudentInfo(StudentCompleteInfoDTO dto)
{
    var student = await db.Students.FirstOrDefaultAsync(s => s.NationalId == dto.NationalId);

    if (student == null)
        return NotFound(new { message = "Student not found" });

    student.DateOfBirth = dto.DateOfBirth;
    student.ParentOccupation = dto.ParentOccupation;
    student.Address = dto.Address;
    student.City = dto.City;
    student.District = dto.District;
    student.StreetName = dto.StreetName;
    student.BuildingNo = dto.BuildingNo;
    student.PhoneNumber = dto.PhoneNumber;
    student.Email = dto.Email;
    await db.SaveChangesAsync();
    return Ok(new { message = "Student info updated successfully" });
}
[HttpPost("submit-exam")]
public async Task<IActionResult> SubmitExamResult(ExamResultDTO dto)
{
    var student = await db.Students.FirstOrDefaultAsync(s => s.NationalId == dto.NationalId);
    if (student == null)
        return NotFound(new { message = "Student not found" });

    student.ExamMathScore = dto.MathScore;
    student.ExamEnglishScore = dto.EnglishScore;
    student.ExamArabicScore = dto.ArabicScore;
    student.ExamSoftwareScore = dto.SoftwareScore;
    await db.SaveChangesAsync();
    return Ok(new { message = "Exam results submitted successfully" });
}

