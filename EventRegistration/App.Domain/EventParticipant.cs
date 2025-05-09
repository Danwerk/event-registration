using Domain.Base;

namespace App.Domain;

public class EventParticipant : DomainEntityId
{
    public int EventId { get; set; }
    public Event? Event { get; set; }

    public int ParticipantId { get; set; }
    public Participant? Participant { get; set; }

}