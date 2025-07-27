using Microsoft.AspNetCore.Mvc;
using SchoolAdmission.DTOs;
using SchoolAdmission.Models;
using SchoolAdmission.Data;

[ApiController]
[Route("api/[controller]")]
public class TeacherController : ControllerBase
{
    private readonly SchoolAdmissionDbContext db;

    public TeacherController(SchoolAdmissionDbContext context)
    {
        db = context;
    }

  
}
