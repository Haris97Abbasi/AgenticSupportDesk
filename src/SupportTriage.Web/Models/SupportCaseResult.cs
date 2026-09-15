namespace SupportTriage.Web.Models;

public sealed class SupportCaseResult
{
    public required string Summary { get; set; }
    public required string Category { get; set; }
    public required string Priority { get; set; }
    public required string RecommendedAction { get; set; }
    public bool NeedsHumanReview { get; set; }
}
