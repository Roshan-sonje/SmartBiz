namespace SmartBiz.Domain.Enums;

public enum PurchaseStatus
{
    Draft = 1,
    Ordered = 2,     // PO sent to supplier, not received yet
    Received = 3,    // goods received, stock increased
    Cancelled = 4,
    Returned = 5
}