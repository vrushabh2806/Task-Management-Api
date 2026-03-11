using System.Security.Claims;

namespace TaskManagement.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
            }

            return userId;
        }
    }
}