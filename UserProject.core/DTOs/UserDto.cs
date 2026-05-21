namespace UserProject.Core.DTOs
{
    public class UserDto
    {
        public string Role { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public Guid UserId { get; set; }
        public string ProfileImageUrl { get; set; }
        
    }
}
