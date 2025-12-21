namespace PktApp.Domain.Enums;

public enum TransactionStatus
{
    Planned,
    InProgress,
    ProductionCompleted,
    Washing,
    WashingCompleted,
    Completed,
    Cancelled
}
