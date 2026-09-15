namespace SupportTriage.Web.Models;

public sealed class Customer
{
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string MembershipTier { get; init; }
}
