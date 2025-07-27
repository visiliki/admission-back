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

}
