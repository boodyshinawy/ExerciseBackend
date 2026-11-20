namespace Project.Api;

public class SearchParams
{
    public string? FirmId { get; set; }
    public string? PersonName { get; set; }
    public int? CategoryId { get; set; }
    public int? ItemId { get; set; }
    public int? StatusId { get; set; }
    public DateTime? DelieveryDateFrom { get; set; }
    public DateTime? DelieveryDateTo { get; set; }
    public string? AmanaAcceptNumber { get; set; }
    public DateTime? AmanaAcceptDateFrom { get; set; }
    public DateTime? AmanaAcceptDateTo { get; set; }
}