using System.ComponentModel.DataAnnotations;

namespace StudentRosterDbApi.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Course { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Range(1, 4, ErrorMessage = "YearLevel must be between 1 and 4.")]
        public int YearLevel { get; set; }

        // One-to-Many Foreign Key
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        // Many-to-Many Navigation Property
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
