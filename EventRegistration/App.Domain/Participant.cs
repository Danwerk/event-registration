using System.ComponentModel.DataAnnotations;
using Domain.Base;

namespace App.Domain;

public class Participant : DomainEntityId
{
    public Guid PaymentMethodId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }

    [MaxLength(5000)]
    public string AdditionalInfo { get; set; } = default!;

    public ICollection<EventParticipant>? EventParticipants { get; set; }
}