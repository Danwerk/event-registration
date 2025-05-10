namespace WebApp.Models;

public class EventParticipantViewModel
{
    public App.Domain.Event Event { get; set; } = default!;
    public List<ParticipantDisplayViewModel> Participants { get; set; } = new();
}