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
