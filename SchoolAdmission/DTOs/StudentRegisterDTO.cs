using System.ComponentModel.DataAnnotations;

namespace SchoolAdmission.DTOs
{
    public class StudentRegisterDTO
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string NationalId { get; set; }
        public double MathScore { get; set; }
        public double EnglishScore { get; set; }
        public double FinalYearScore { get; set; }
        public bool AcceptanceLetterReceived { get; set; }
    }
} 