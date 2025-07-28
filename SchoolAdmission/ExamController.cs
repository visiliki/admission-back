using Microsoft.AspNetCore.Mvc;
using SchoolAdmission.Models;
using SchoolAdmission.Data;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class ExamController : ControllerBase
{
    private readonly SchoolAdmissionDbContext db;

    public ExamController(SchoolAdmissionDbContext context)
    {
        db = context;
    }
 

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitExamResult([FromBody] ExamResultDTO dto)
    {
        var student = await db.Students.FirstOrDefaultAsync(s => s.NationalId == dto.NationalId);
        if (student == null)
            return NotFound(new { message = "Student not found" });

        var existingExam = await db.Exams.FirstOrDefaultAsync(e => e.StudentId == student.Id);
        if (existingExam != null)
            return BadRequest(new { message = "Student has already submitted the exam." });

        var exam = new Exam
        {
            StudentId = student.Id,
            MathScore = dto.MathScore,
            EnglishScore = dto.EnglishScore,
            ArabicScore = dto.ArabicScore,
            SoftwareScore = dto.SoftwareScore
        };

        db.Exams.Add(exam);
        await db.SaveChangesAsync();

        return Ok(new { message = "Exam submitted successfully" });
    }
 [HttpPost("upload-questions")]
    public async Task<IActionResult> UploadQuestions()
    {
        var file = Request.Form.Files.FirstOrDefault();
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        var questions = new List<ExamQuestion>();

        using (var stream = file.OpenReadStream())
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            var result = reader.AsDataSet();
            var table = result.Tables[0];

            for (int i = 1; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var q = new ExamQuestion
                {
                    Subject = row[0].ToString(),
                    Text = row[1].ToString(),
                    OptionA = row[2].ToString(),
                    OptionB = row[3].ToString(),
                    OptionC = row[4].ToString(),
                    OptionD = row[5].ToString(),
                    CorrectAnswer = row[6].ToString()
                };
                questions.Add(q);
            }
        }

        db.ExamQuestions.AddRange(questions);
        await db.SaveChangesAsync();
        return Ok(new { message = "Questions uploaded successfully" });
    }

   
    [HttpGet("get-questions")]
    public IActionResult GetQuestions([FromQuery] string subject, [FromQuery] int count)
    {
        var questions = db.ExamQuestions
                          .Where(q => q.Subject == subject)
                          .ToList();

        var rng = new Random();
        var selected = questions.OrderBy(q => rng.Next()).Take(count).ToList();

        foreach (var q in selected)
            q.CorrectAnswer = null;

        return Ok(selected);
    }

   
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitExamResult([FromBody] ExamResultDTO dto)
    {
        var student = await db.Students.FirstOrDefaultAsync(s => s.NationalId == dto.NationalId);
        if (student == null)
            return NotFound(new { message = "Student not found" });

        var existingExam = await db.Exams.FirstOrDefaultAsync(e => e.StudentId == student.Id);
        if (existingExam != null)
            return BadRequest(new { message = "Student has already submitted the exam." });

        var exam = new Exam
        {
            StudentId = student.Id,
            MathScore = dto.MathScore,
            EnglishScore = dto.EnglishScore,
            ArabicScore = dto.ArabicScore,
            SoftwareScore = dto.SoftwareScore
        };

        db.Exams.Add(exam);
        await db.SaveChangesAsync();

        return Ok(new { message = "Exam submitted successfully" });
    }
}