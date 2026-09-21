namespace Application.DTOs.Wallet;

public class LedgerTransactionDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? AuctionId { get; set; }
}
