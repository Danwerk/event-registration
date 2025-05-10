using Domain.Base;

namespace App.Domain;

public class EventParticipant : DomainEntityId
{
    public Guid EventId { get; set; }
    public Event? Event { get; set; }

    public Guid ParticipantId { get; set; }
    public Participant? Participant { get; set; }

}