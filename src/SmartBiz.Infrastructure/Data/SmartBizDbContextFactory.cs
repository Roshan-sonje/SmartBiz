using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Infrastructure.Data;

/// <summary>
/// Used by EF Core CLI tools (dotnet ef migrations add / database update)
/// to instantiate the DbContext at design time without running the API.
/// </summary>
public class SmartBizDbContextFactory : IDesignTimeDbContextFactory<SmartBizDbContext>
{
    // ⚠️ MUST match the <UserSecretsId> in SmartBiz.Api.csproj
    private const string ApiUserSecretsId = "73278c0d-c2ae-4fb6-a614-8f43736aa635";

    public SmartBizDbContext CreateDbContext(string[] args)
    {
        // Build a config that looks in the Api project's settings files
        var apiProjectPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "SmartBiz.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetFullPath(apiProjectPath))
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets(ApiUserSecretsId)         // ← reads User Secrets
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString) ||
            connectionString.Contains("CHANGE_ME"))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is missing or still has the placeholder password. " +
                "Configure it via: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\" " +
                "from the SmartBiz.Api folder.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<SmartBizDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        var designTimeTenantProvider = new DesignTimeTenantProvider();

        return new SmartBizDbContext(optionsBuilder.Options, designTimeTenantProvider);
    }
}

internal class DesignTimeTenantProvider : ITenantProvider
{
    public Guid? BusinessId => null;
    public Guid? UserId => null;
    public bool HasTenant => false;
}