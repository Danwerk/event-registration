using App.Contracts.DAL;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class EventRepository : EFBaseRepository<Event, AppDbContext>, IEventRepository
{
    public EventRepository(AppDbContext dataContext) : base(dataContext)
    {
    }

    public override async Task<IEnumerable<Event>> AllAsync()
    {
        return await RepositoryDbSet
            .Include(e=>e.EventParticipants)!
            .ThenInclude(e => e.Participant)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Event>> GetFutureEventsAsync()
    {
        return await RepositoryDbSet
            .Where(e => e.DateTime > DateTime.UtcNow)
            .ToListAsync();
    }
}