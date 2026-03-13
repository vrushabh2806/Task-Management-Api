using Microsoft.Identity.Client.Extensibility;

namespace TaskManagement.Models
{
    public class RefreshToken
    {
        public int Id{get; set;}
        public string? Token{get; set;}
        public DateTime ExpiresAt{get; set;}
        public DateTime? CreatedAt{get; set;}= DateTime.UtcNow;
        public DateTime? RevokedAt{get; set;}
        public string? RevokedByIp{get; set;}
        public string CreatedByIp { get; set; } = string.Empty; 
        public string?  ReplacedByToken{get; set;}
        public int UserId{get; set;}
        public User? User{get; set;} 
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}