namespace JJ_API.Models.DAO
{
    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }
        public int UserId { get; set; }
        public int TouristSpotId { get; set; }
        public int? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Avatar { get; set; }
        public string Username { get; set; }
        public int CommentChildNumber { get; set; }

    }
}
