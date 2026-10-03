namespace SmartBiz.Domain.Enums;

public enum PermissionType
{
    // Customers
    CustomerView = 1001,
    CustomerCreate = 1002,
    CustomerUpdate = 1003,
    CustomerDelete = 1004,

    // Suppliers
    SupplierView = 2001,
    SupplierCreate = 2002,
    SupplierUpdate = 2003,
    SupplierDelete = 2004,

    // Products
    ProductView = 3001,
    ProductCreate = 3002,
    ProductUpdate = 3003,
    ProductDelete = 3004,

    // Sales
    SaleView = 4001,
    SaleCreate = 4002,
    SaleUpdate = 4003,
    SaleDelete = 4004,
    SaleCancel = 4005,

    // Purchases
    PurchaseView = 5001,
    PurchaseCreate = 5002,
    PurchaseUpdate = 5003,
    PurchaseDelete = 5004,

    // Payments
    PaymentView = 6001,
    PaymentCreate = 6002,
    PaymentUpdate = 6003,
    PaymentDelete = 6004,

    // Expenses
    ExpenseView = 7001,
    ExpenseCreate = 7002,
    ExpenseUpdate = 7003,
    ExpenseDelete = 7004,

    // Reports
    ReportView = 8001,
    ReportExport = 8002,

    // Users & Roles (admin only)
    UserView = 9001,
    UserCreate = 9002,
    UserUpdate = 9003,
    UserDelete = 9004,
    RoleView = 9101,
    RoleCreate = 9102,
    RoleUpdate = 9103,
    RoleDelete = 9104,

    // Settings
    SettingsView = 9501,
    SettingsUpdate = 9502
}