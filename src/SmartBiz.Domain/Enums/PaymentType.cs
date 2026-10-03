namespace SmartBiz.Domain.Enums;

public enum PaymentType
{
    CustomerReceipt = 1,   // money received from a customer
    SupplierPayment = 2,   // money paid to a supplier
    CustomerRefund = 3,    // money returned to a customer
    SupplierRefund = 4     // money returned by a supplier
}