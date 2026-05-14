using System.Net.Mail;

namespace Test.Models
{
    public enum Priority { Low = 1, Medium, High, Critical }
    public enum IncidentStatus { Open, InProgress, Resolved, Closed, Cancelled }
    public enum Category { Hardware, Software, Network, Access, Other }
    public class Incident
    {

          public int Id { get; set; }
            public string TicketNumber { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public Priority Priority { get; set; } = Priority.Medium;
            public IncidentStatus Status { get; set; } = IncidentStatus.Open;
            public Category Category { get; set; } = Category.Other;
            public string? AssignedTo { get; set; }
            public string? ReportedBy { get; set; }
            public string? AffectedSystem { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? ResolvedAt { get; set; }
            public DateTime SLADueDate { get; set; }
            public string? ResolutionNotes { get; set; }
            public int? LinkedProblemId { get; set; }
            public Problem? LinkedProblem { get; set; }
            public ICollection<Comment> Comments { get; set; } = new List<Comment>();
            public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        

    }
}
