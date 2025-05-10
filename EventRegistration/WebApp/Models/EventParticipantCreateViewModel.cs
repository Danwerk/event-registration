namespace WebApp.Models
{
    public class EventParticipantCreateViewModel
    {
        public Guid EventId { get; set; }

        public string ParticipantType { get; set; } = default!; // "private" või "legal"

        // PrivatePerson andmed
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonalCode { get; set; }

        // LegalPerson andmed
        public string? CompanyName { get; set; }
        public string? RegistryCode { get; set; }
        public int? NumberOfAttendees { get; set; } // mitu osalejat ettevõttel

        // Ühine
        public Guid PaymentMethodId { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}