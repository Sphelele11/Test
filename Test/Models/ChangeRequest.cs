namespace Test.Models
{
    public enum ChangeType { Standard, Normal, Emergency }
    public enum ChangeStatus { Draft, Submitted, UnderReview, Approved, Scheduled, InProgress, Completed, Failed, Cancelled }
    public enum ChangeRisk { Low, Medium, High, Critical }
    public class ChangeRequest
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ChangeType Type { get; set; } = ChangeType.Normal;
        public ChangeStatus Status { get; set; } = ChangeStatus.Draft;
        public ChangeRisk Risk { get; set; } = ChangeRisk.Medium;
        public Priority Priority { get; set; } = Priority.Medium;
        public string? RequestedBy { get; set; }
        public string? AssignedTo { get; set; }
        public string? ApprovedBy { get; set; }
        public string Justification { get; set; } = string.Empty;
        public string ImplementationPlan { get; set; } = string.Empty;
        public string BackoutPlan { get; set; } = string.Empty;
        public string? TestPlan { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
