using Domain.Base;

namespace App.Domain;

public class Participant : DomainEntityId
{
    public Guid PaymentMethodId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }

    public string AdditionalInfo { get; set; } = default!;

    public ICollection<EventParticipant>? EventParticipants { get; set; }
}