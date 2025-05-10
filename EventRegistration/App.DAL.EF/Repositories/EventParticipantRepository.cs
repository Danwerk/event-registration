using App.Contracts.DAL;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class EventParticipantRepository : EFBaseRepository<EventParticipant, AppDbContext>, IEventParticipantRepository
{
    public EventParticipantRepository(AppDbContext dataContext) : base(dataContext)
    {
    }

    public override async Task<IEnumerable<EventParticipant>> AllAsync()
    {
        return await RepositoryDbSet
            .Include(e=>e.Event)
            .Include(e=>e.Participant).OrderBy(e => e.EventId).ToListAsync();
    }
    
    public override async Task<EventParticipant?> FindAsync(Guid id)
    {
        return await RepositoryDbSet
            .Include(e => e.Event)
            .Include(e => e.Participant)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}