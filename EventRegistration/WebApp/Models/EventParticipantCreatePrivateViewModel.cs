// EventParticipantCreatePrivateViewModel.cs
namespace WebApp.Models
{
    public class EventParticipantCreatePrivateViewModel
    {
        public Guid EventId { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PersonalCode { get; set; } = default!;
        public Guid? PaymentMethodId { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}