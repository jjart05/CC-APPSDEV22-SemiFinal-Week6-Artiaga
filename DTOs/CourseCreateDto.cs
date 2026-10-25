using System.ComponentModel.DataAnnotations;

namespace StudentRosterDbApi.DTOs
{
    public class CourseCreateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;
    }
}
