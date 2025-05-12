using System.ComponentModel.DataAnnotations;
using Domain.Base;

namespace App.Domain;

public class Event : DomainEntityId
{
    [MaxLength(512)]
    public string Name { get; set; } = default!;
    
    public DateTime DateTime { get; set; }
    
    public string Location { get; set; } = default!;
    
    [MaxLength(1000)]
    public string AdditionalInfo { get; set; } = default!;
    
    public ICollection<EventParticipant>? EventParticipants { get; set; }
}