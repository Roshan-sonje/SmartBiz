namespace SmartBiz.Domain.Enums;

public enum PaymentMethod
{
    Cash = 1,
    Card = 2,
    Upi = 3,
    BankTransfer = 4,
    Cheque = 5,
    Credit = 6,     // on account / udhaar
    Other = 99
}