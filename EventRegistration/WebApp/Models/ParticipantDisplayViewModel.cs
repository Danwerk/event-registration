namespace WebApp.Models;

public class ParticipantDisplayViewModel
{
    public Guid EventParticipantId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
}