namespace Test.Models
{
    public enum ArticleStatus { Draft, Review, Published, Archived }
    public class KnowledgeArticle
    {
        public int Id { get; set; }
        public string ArticleNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
        public Category Category { get; set; } = Category.Other;
        public string? Tags { get; set; }
        public string? Author { get; set; }
        public string? ReviewedBy { get; set; }
        public int Views { get; set; } = 0;
        public int HelpfulVotes { get; set; } = 0;
        public int UnhelpfulVotes { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAt { get; set; }
    }
}
