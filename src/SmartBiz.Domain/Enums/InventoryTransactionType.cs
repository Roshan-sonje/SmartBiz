namespace SmartBiz.Domain.Enums;

public enum InventoryTransactionType
{
    Opening = 1,        // initial stock on product creation
    Purchase = 2,       // stock added from a purchase
    Sale = 3,           // stock removed by a sale
    Adjustment = 4,     // manual correction
    Return = 5,         // customer returned goods
    Damage = 6,         // damaged/lost stock
    Transfer = 7        // moved between locations (future)
}