using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Customers;

namespace SmartBiz.Desktop.Services;

public interface ICustomerService
{
    Task<PagedResult<CustomerListItem>> GetCustomersAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        bool includeInactive = false,
        CancellationToken ct = default);

    Task<Customer> GetCustomerAsync(Guid id, CancellationToken ct = default);

    Task<Customer> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken ct = default);

    Task<Customer> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request, CancellationToken ct = default);

    Task DeleteCustomerAsync(Guid id, CancellationToken ct = default);
}