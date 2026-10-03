using SmartBiz.Domain.Common;
using SmartBiz.Domain.Enums;
using System.Data;

namespace SmartBiz.Domain.Entities;

public class Business : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Gstin { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "India";
    public string? LogoUrl { get; set; }
    public string Currency { get; set; } = "INR";
    public BusinessStatus Status { get; set; } = BusinessStatus.Trial;
    public DateTime? TrialEndsAt { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}