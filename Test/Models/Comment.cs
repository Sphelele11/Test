namespace Test.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public bool IsInternal { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? IncidentId { get; set; }
        public int? ProblemId { get; set; }
        public int? ChangeRequestId { get; set; }
        public int? ServiceRequestId { get; set; }
    }
}
