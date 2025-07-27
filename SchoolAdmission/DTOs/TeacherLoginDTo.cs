
using System.ComponentModel.DataAnnotations;

namespace SchoolAdmission.DTOs;

public class TeacherLoginDTO {

    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}