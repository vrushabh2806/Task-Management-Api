namespace TaskManagement.DTOs
{
    public class LoginResponseDto
    {
        public string? Token { get; set; }
        public UserResponseDto? User { get; set; }

        public DateTime ExpiredAt { get; set; } 
    }
}