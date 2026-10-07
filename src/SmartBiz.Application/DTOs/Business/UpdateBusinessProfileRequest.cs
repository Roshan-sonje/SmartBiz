namespace SmartBiz.Application.DTOs.Business;

public class UpdateBusinessProfileRequest
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
    public string? Country { get; set; }
    public string? LogoUrl { get; set; }
}