namespace ESE.Payments.API.Models
{
    public enum PaymentTransactionStatus
    {
        Authorized = 1,
        Paid,
        Refused,
        Chargedback,
        Cancelled
    }
}
