using System.ComponentModel;
using Microsoft.Extensions.AI;
using SupportTriage.Web.Models;

namespace SupportTriage.Web.Services;

public static class CustomerLookupService
{
    public static readonly AIFunction Tool = AIFunctionFactory.Create(GetCustomerByEmail);

    private static readonly Dictionary<string, Customer> Customers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["sara@example.com"] = new Customer
        {
            FullName = "Sara Ahmed",
            Email = "sara@example.com",
            MembershipTier = "Premium"
        },
        ["john.doe@example.com"] = new Customer
        {
            FullName = "John Doe",
            Email = "john.doe@example.com",
            MembershipTier = "Standard"
        },
        ["priya.singh@example.com"] = new Customer
        {
            FullName = "Priya Singh",
            Email = "priya.singh@example.com",
            MembershipTier = "Standard"
        }
    };

    [Description("Looks up a customer by their email address and returns their name and membership tier. Returns a not-found message if the email is unknown.")]
    public static string GetCustomerByEmail(
        [Description("The customer's email address, e.g. sara@example.com.")] string email)
    {
        if (Customers.TryGetValue(email, out var customer))
        {
            return $"Customer: name={customer.FullName}, email={customer.Email}, membershipTier={customer.MembershipTier}";
        }

        return $"No customer found with email '{email}'. Do not guess or invent customer details.";
    }
}
