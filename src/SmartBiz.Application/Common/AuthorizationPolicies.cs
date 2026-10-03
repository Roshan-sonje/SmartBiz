namespace SmartBiz.Application.Common;

public static class AuthorizationPolicies
{
    public const string AuthenticatedUser = "AuthenticatedUser";
    public const string AdminOnly = "AdminOnly";
    public const string CanManageCustomers = "CanManageCustomers";
    public const string CanManageProducts = "CanManageProducts";
    public const string CanCreateSales = "CanCreateSales";
    public const string CanViewReports = "CanViewReports";
}