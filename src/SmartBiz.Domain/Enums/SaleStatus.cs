namespace SmartBiz.Domain.Enums;

public enum SaleStatus
{
    Draft = 1,      // being built, can be edited freely
    Held = 2,       // parked (POS hold bill), not finalized
    Completed = 3,  // finalized, stock reduced, invoice issued
    Cancelled = 4,  // voided after completion — stock restored
    Refunded = 5    // money returned to customer
}