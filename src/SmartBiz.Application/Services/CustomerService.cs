using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Customers;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IApplicationDbContext db, ILogger<CustomerService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<CustomerListItemDto>> GetCustomersAsync(
        Guid businessId,
        int page,
        int pageSize,
        string? search,
        bool includeInactive,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.Customers
            .IgnoreQueryFilters()
            .Where(c => c.BusinessId == businessId);

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(c =>
                c.Name.ToLower().Contains(s) ||
                (c.Phone != null && c.Phone.Contains(s)) ||
                (c.Email != null && c.Email.ToLower().Contains(s)));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CustomerListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                City = c.City,
                OpeningBalance = c.OpeningBalance,
                IsActive = c.IsActive
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedResult<CustomerListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<CustomerDto> GetCustomerAsync(
        Guid businessId,
        Guid customerId,
        CancellationToken ct = default)
    {
        var customer = await _db.Customers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == customerId && c.BusinessId == businessId, ct);

        if (customer is null)
            throw new AuthException("Customer not found.", 404);

        return MapToDto(customer);
    }

    public async Task<CustomerDto> CreateCustomerAsync(
        Guid businessId,
        CreateCustomerRequest request,
        CancellationToken ct = default)
    {
        var customer = new Customer
        {
            BusinessId = businessId,
            Name = request.Name.Trim(),
            Phone = NullIfEmpty(request.Phone),
            Email = NullIfEmpty(request.Email)?.ToLowerInvariant(),
            Address = NullIfEmpty(request.Address),
            City = NullIfEmpty(request.City),
            State = NullIfEmpty(request.State),
            PostalCode = NullIfEmpty(request.PostalCode),
            Gstin = NullIfEmpty(request.Gstin)?.ToUpperInvariant(),
            OpeningBalance = request.OpeningBalance,
            CreditLimit = request.CreditLimit,
            Notes = NullIfEmpty(request.Notes),
            IsActive = true
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Customer created: {CustomerId} ({Name}) for business {BusinessId}",
            customer.Id, customer.Name, businessId);

        return MapToDto(customer);
    }

    public async Task<CustomerDto> UpdateCustomerAsync(
        Guid businessId,
        Guid customerId,
        UpdateCustomerRequest request,
        CancellationToken ct = default)
    {
        var customer = await _db.Customers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == customerId && c.BusinessId == businessId, ct);

        if (customer is null)
            throw new AuthException("Customer not found.", 404);

        customer.Name = request.Name.Trim();
        customer.Phone = NullIfEmpty(request.Phone);
        customer.Email = NullIfEmpty(request.Email)?.ToLowerInvariant();
        customer.Address = NullIfEmpty(request.Address);
        customer.City = NullIfEmpty(request.City);
        customer.State = NullIfEmpty(request.State);
        customer.PostalCode = NullIfEmpty(request.PostalCode);
        customer.Gstin = NullIfEmpty(request.Gstin)?.ToUpperInvariant();
        customer.OpeningBalance = request.OpeningBalance;
        customer.CreditLimit = request.CreditLimit;
        customer.Notes = NullIfEmpty(request.Notes);
        customer.IsActive = request.IsActive;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Customer updated: {CustomerId}", customerId);

        return MapToDto(customer);
    }

    public async Task DeleteCustomerAsync(
        Guid businessId,
        Guid customerId,
        CancellationToken ct = default)
    {
        var customer = await _db.Customers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == customerId && c.BusinessId == businessId, ct);

        if (customer is null)
            throw new AuthException("Customer not found.", 404);

        // Soft delete
        customer.IsActive = false;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Customer soft-deleted: {CustomerId}", customerId);
    }

    // -------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static CustomerDto MapToDto(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Phone = c.Phone,
        Email = c.Email,
        Address = c.Address,
        City = c.City,
        State = c.State,
        PostalCode = c.PostalCode,
        Gstin = c.Gstin,
        OpeningBalance = c.OpeningBalance,
        CreditLimit = c.CreditLimit,
        Notes = c.Notes,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}