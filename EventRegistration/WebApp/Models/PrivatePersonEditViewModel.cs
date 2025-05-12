namespace WebApp.Models;

public class PrivatePersonEditViewModel
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string PersonalCode { get; set; } = default!;
    
    public Guid PaymentMethodId { get; set; }
    public string? AdditionalInfo { get; set; }
    public Guid EventId { get; set; }

}