using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Customers;

namespace SmartBiz.Desktop.Services;

public class CustomerService : ICustomerService
{
    private readonly IApiClient _api;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IApiClient api, ILogger<CustomerService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<PagedResult<CustomerListItem>> GetCustomersAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        bool includeInactive = false,
        CancellationToken ct = default)
    {
        var query = $"?page={page}&pageSize={pageSize}&includeInactive={includeInactive.ToString().ToLower()}";
        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        var result = await _api.GetAsync<PagedResult<CustomerListItem>>(
            $"/api/{AppConfig.ApiVersion}/customers{query}", ct);

        return result ?? new PagedResult<CustomerListItem>();
    }

    public async Task<Customer> GetCustomerAsync(Guid id, CancellationToken ct = default)
    {
        var result = await _api.GetAsync<Customer>(
            $"/api/{AppConfig.ApiVersion}/customers/{id}", ct);

        return result ?? throw new InvalidOperationException("Customer not found.");
    }

    public async Task<Customer> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken ct = default)
    {
        var result = await _api.PostAsync<CreateCustomerRequest, Customer>(
            $"/api/{AppConfig.ApiVersion}/customers", request, ct);

        _logger.LogInformation("Customer created: {Name}", request.Name);
        return result ?? throw new InvalidOperationException("Failed to create customer.");
    }

    public async Task<Customer> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request, CancellationToken ct = default)
    {
        var result = await _api.PutAsync<UpdateCustomerRequest, Customer>(
            $"/api/{AppConfig.ApiVersion}/customers/{id}", request, ct);

        _logger.LogInformation("Customer updated: {Id}", id);
        return result ?? throw new InvalidOperationException("Failed to update customer.");
    }

    public async Task DeleteCustomerAsync(Guid id, CancellationToken ct = default)
    {
        await _api.DeleteAsync($"/api/{AppConfig.ApiVersion}/customers/{id}", ct);
        _logger.LogInformation("Customer deleted: {Id}", id);
    }
}