using App.Domain;
using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IEventRepository : IBaseRepository<Event>, IEventRepositoryCustom<Event>
{
    
}

public interface IEventRepositoryCustom<TEntity>
{

    //add here shared methods between repo and service
    Task<IEnumerable<TEntity>> GetFutureEventsAsync();
}