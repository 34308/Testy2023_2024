namespace JJ_API.Models.DAO
{
    public class NotificationDao
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Checked { get; set; }
        
    }
}
