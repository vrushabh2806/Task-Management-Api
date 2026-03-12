using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DTOs
{
    public class ChangeRoleDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string? RoleName{get;set;}
    }
}