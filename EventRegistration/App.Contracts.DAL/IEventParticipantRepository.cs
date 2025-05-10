using App.Domain;
using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IEventParticipantRepository : IBaseRepository<EventParticipant>, IEventParticipantRepositoryCustom<EventParticipant>
{
    
}

public interface IEventParticipantRepositoryCustom<TEntity>
{
    //add here shared methods between repo and service
    public Task<IEnumerable<TEntity>> AllAsync(Guid eventId);
}