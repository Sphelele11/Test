namespace Test.Models
{
    public enum ProblemStatus { Open, UnderInvestigation, KnownError, Resolved, Closed }
    public class Problem
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Priority Priority { get; set; } = Priority.Medium;
        public ProblemStatus Status { get; set; } = ProblemStatus.Open;
        public Category Category { get; set; } = Category.Other;
        public string? AssignedTo { get; set; }
        public string? RootCause { get; set; }
        public string? Workaround { get; set; }
        public string? PermanentFix { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
        public ICollection<Incident> RelatedIncidents { get; set; } = new List<Incident>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
