using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Customers;

namespace SmartBiz.Application.Interfaces;

public interface ICustomerService
{
    Task<PagedResult<CustomerListItemDto>> GetCustomersAsync(
        Guid businessId,
        int page,
        int pageSize,
        string? search,
        bool includeInactive,
        CancellationToken ct = default);

    Task<CustomerDto> GetCustomerAsync(
        Guid businessId,
        Guid customerId,
        CancellationToken ct = default);

    Task<CustomerDto> CreateCustomerAsync(
        Guid businessId,
        CreateCustomerRequest request,
        CancellationToken ct = default);

    Task<CustomerDto> UpdateCustomerAsync(
        Guid businessId,
        Guid customerId,
        UpdateCustomerRequest request,
        CancellationToken ct = default);

    Task DeleteCustomerAsync(
        Guid businessId,
        Guid customerId,
        CancellationToken ct = default);
}