namespace Job_Recruitment.Models;

public class Rate
{
    public string BaseCurrency { get; set; }
    public string Currency { get; set; }
    public double? SaleRateNB { get; set; }
    public double? PurchaseRateNB { get; set; }
    public double? SaleRate { get; set; }
    public double? PurchaseRate { get; set; }
}