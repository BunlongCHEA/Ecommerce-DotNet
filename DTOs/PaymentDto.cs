public class PaymentDto
{
    public int Id { get; set; }

    public string? PaymentMethod { get; set; }

    public string? BankName { get; set; }

    public string? AccountOrCardNumber { get; set; }

    public string? CardHolderName { get; set; }

    public DateTimeOffset? CardExpireDate { get; set; }

    public int UserId { get; set; }
}