namespace SmartBiz.Application.DTOs.Sales;

public class SalesSummaryDto
{
    public int TodayCount { get; set; }
    public decimal TodayTotal { get; set; }
    public int MonthCount { get; set; }
    public decimal MonthTotal { get; set; }
    public decimal UnpaidTotal { get; set; }
}