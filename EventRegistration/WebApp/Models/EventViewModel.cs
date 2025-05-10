namespace WebApp.Models;

public class EventViewModel
{
    public IEnumerable<App.Domain.Event> FutureEvents { get; set; } = new List<App.Domain.Event>();
    public IEnumerable<App.Domain.Event> PastEvents { get; set; } = new List<App.Domain.Event>();
}