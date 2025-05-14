// EventParticipantCreateLegalViewModel.cs
namespace WebApp.Models
{
    public class EventParticipantCreateLegalViewModel
    {
        public Guid EventId { get; set; }
        public string CompanyName { get; set; } = default!;
        public string RegistryCode { get; set; } = default!;
        public int? NumberOfAttendees { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}