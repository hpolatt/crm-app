using PktApp.Domain.Enums;

namespace PktApp.Core.DTOs.PktTransactions;

public class TransactionStatusUpdateDto
{
    public TransactionStatus NewStatus { get; set; }
    public string? Note { get; set; }
}
