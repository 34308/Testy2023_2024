namespace JJ_API.Models.DTO
{
    public class CommentDto
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }
        public int UserId { get; set; }
        public int TouristSpotId { get; set; }
    }
}
