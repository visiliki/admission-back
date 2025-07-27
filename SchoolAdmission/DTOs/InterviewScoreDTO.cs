using System.ComponentModel.DataAnnotations;

namespace SchoolAdmission.DTOs
{
    public class InterviewScoreDTO
    {
        [Required]
        public string NationalId { get; set; }
        [Required]
        public double Score { get; set; } // out of 40
    }
} 