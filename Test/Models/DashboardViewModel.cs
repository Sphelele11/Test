namespace Test.Models
{
    public class DashboardViewModel
    {
        public int OpenIncidents { get; set; }
        public int InProgressIncidents { get; set; }
        public int CriticalIncidents { get; set; }
        public int OpenProblems { get; set; }
        public int PendingChanges { get; set; }
        public int OpenServiceRequests { get; set; }
        public int TotalAssets { get; set; }
        public int KnowledgeArticles { get; set; }
        public int SLABreached { get; set; }
        public List<Incident> RecentIncidents { get; set; } = new();
        public List<ChangeRequest> UpcomingChanges { get; set; } = new();
        public Dictionary<string, int> IncidentsByCategory { get; set; } = new();
        public Dictionary<string, int> IncidentsByPriority { get; set; } = new();
        public List<(string Month, int Count)> IncidentTrend { get; set; } = new();
    }
}
