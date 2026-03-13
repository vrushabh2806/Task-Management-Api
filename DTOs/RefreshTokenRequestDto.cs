using System.ComponentModel.DataAnnotations;

namespace TokenManagement.DTOs
{
    public class RefreshTokenRequestDto
    {
      [Required(ErrorMessage="Refresh token is required.")]
        public string? RefreshToken { get; set; }
    }
}
