namespace JJ_API.Models.DAO
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int AvatarId { get; set; }
        public int Role { get; set; }
    }
}
