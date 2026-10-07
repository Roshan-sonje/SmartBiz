using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.DTOs.Business;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Application.Services;

public class BusinessService : IBusinessService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<BusinessService> _logger;

    public BusinessService(IApplicationDbContext db, ILogger<BusinessService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<BusinessProfileDto> GetProfileAsync(Guid businessId, CancellationToken ct = default)
    {
        var business = await _db.Businesses
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == businessId, ct);

        if (business is null)
            throw new AuthException("Business not found.", 404);

        return MapToDto(business);
    }

    public async Task<BusinessProfileDto> UpdateProfileAsync(
        Guid businessId,
        UpdateBusinessProfileRequest request,
        CancellationToken ct = default)
    {
        var business = await _db.Businesses
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id == businessId, ct);

        if (business is null)
            throw new AuthException("Business not found.", 404);

        // Update fields
        business.Name = request.Name.Trim();
        business.LegalName = NullIfEmpty(request.LegalName);
        business.Gstin = NullIfEmpty(request.Gstin)?.ToUpperInvariant();
        business.Phone = NullIfEmpty(request.Phone);
        business.Email = NullIfEmpty(request.Email)?.ToLowerInvariant();
        business.Address = NullIfEmpty(request.Address);
        business.City = NullIfEmpty(request.City);
        business.State = NullIfEmpty(request.State);
        business.PostalCode = NullIfEmpty(request.PostalCode);
        business.Country = NullIfEmpty(request.Country);
        business.LogoUrl = NullIfEmpty(request.LogoUrl);

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Business profile updated: {BusinessId} ({Name})", businessId, business.Name);

        return MapToDto(business);
    }

    public async Task<BusinessSettingsDto> GetSettingsAsync(Guid businessId, CancellationToken ct = default)
    {
        var settings = await _db.BusinessSettings
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.BusinessId == businessId, ct);

        if (settings is null)
        {
            // Auto-create default settings if missing
            settings = new BusinessSettings { BusinessId = businessId };
            _db.BusinessSettings.Add(settings);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Auto-created default settings for business {BusinessId}", businessId);
        }

        return MapToDto(settings);
    }

    public async Task<BusinessSettingsDto> UpdateSettingsAsync(
        Guid businessId,
        UpdateBusinessSettingsRequest request,
        CancellationToken ct = default)
    {
        var settings = await _db.BusinessSettings
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.BusinessId == businessId, ct);

        if (settings is null)
        {
            settings = new BusinessSettings { BusinessId = businessId };
            _db.BusinessSettings.Add(settings);
        }

        settings.InvoicePrefix = request.InvoicePrefix.Trim();
        settings.InvoiceNumberFormat = request.InvoiceNumberFormat.Trim();
        settings.IncludeFinancialYearInInvoice = request.IncludeFinancialYearInInvoice;
        settings.IsGstEnabled = request.IsGstEnabled;
        settings.Currency = request.Currency.Trim();
        settings.CurrencySymbol = request.CurrencySymbol.Trim();
        settings.TermsAndConditions = NullIfEmpty(request.TermsAndConditions);
        settings.InvoiceFooterNote = NullIfEmpty(request.InvoiceFooterNote);
        settings.ShowLogoOnInvoice = request.ShowLogoOnInvoice;
        settings.ShowTaxBreakupOnInvoice = request.ShowTaxBreakupOnInvoice;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Business settings updated: {BusinessId}", businessId);

        return MapToDto(settings);
    }

    // -------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static BusinessProfileDto MapToDto(Business b) => new()
    {
        Id = b.Id,
        Name = b.Name,
        LegalName = b.LegalName,
        Gstin = b.Gstin,
        Phone = b.Phone,
        Email = b.Email,
        Address = b.Address,
        City = b.City,
        State = b.State,
        PostalCode = b.PostalCode,
        Country = b.Country,
        LogoUrl = b.LogoUrl,
        Currency = b.Currency,
        Status = b.Status.ToString(),
        TrialEndsAt = b.TrialEndsAt,
        CreatedAt = b.CreatedAt,
        UpdatedAt = b.UpdatedAt
    };

    private static BusinessSettingsDto MapToDto(BusinessSettings s) => new()
    {
        Id = s.Id,
        InvoicePrefix = s.InvoicePrefix,
        InvoiceNextNumber = s.InvoiceNextNumber,
        InvoiceNumberFormat = s.InvoiceNumberFormat,
        IncludeFinancialYearInInvoice = s.IncludeFinancialYearInInvoice,
        IsGstEnabled = s.IsGstEnabled,
        DefaultTaxId = s.DefaultTaxId,
        Currency = s.Currency,
        CurrencySymbol = s.CurrencySymbol,
        TermsAndConditions = s.TermsAndConditions,
        InvoiceFooterNote = s.InvoiceFooterNote,
        ShowLogoOnInvoice = s.ShowLogoOnInvoice,
        ShowTaxBreakupOnInvoice = s.ShowTaxBreakupOnInvoice,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}