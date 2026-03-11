using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DTOs
{
    public class CreateTaskDto
    {
        [Required(ErrorMessage ="Title is required")]
        [StringLength(200,MinimumLength =1,ErrorMessage ="Title must be between 1 and 200 characters.")]
        public string? Title { get; set; }
        [StringLength(1000, ErrorMessage="Description cannot exceed 1000 characters.")]
        public string? Description{get; set;}
        [Range(1,3,ErrorMessage="Priority must be between 1 (Low) and 3 (High).")]
        public int Priority{get; set;}=1;
        public DateTime? DueDate{get;set;}
    }
}