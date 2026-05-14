namespace Test.Models
{
    public enum ServiceRequestStatus { Submitted, Approved, InFulfillment, Completed, Rejected, Cancelled }
    public enum ServiceRequestCategory { NewEquipment, Software, Access, Information, Other }

    public class ServiceRequest
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Submitted;
        public ServiceRequestCategory Category { get; set; } = ServiceRequestCategory.Other;
        public Priority Priority { get; set; } = Priority.Low;
        public string? RequestedBy { get; set; }
        public string? AssignedTo { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Department { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime SLADueDate { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
