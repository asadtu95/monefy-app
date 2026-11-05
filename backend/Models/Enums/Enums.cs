namespace Monefy.Api.Models.Enums;

public enum TransactionType
{
    Expense = 0,
    Income = 1
}

public enum BudgetPeriod
{
    Weekly = 0,
    Monthly = 1,
    Yearly = 2
}

public enum GoalStatus
{
    Active = 0,
    Achieved = 1,
    Cancelled = 2
}

public enum PaymentMethod
{
    Cash = 0,
    Card = 1,
    BankTransfer = 2,
    Other = 3
}
