using App.Domain;
using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IParticipantRepository : IBaseRepository<Participant>, IParticipantRepositoryCustom<Participant>
{
    
}

public interface IParticipantRepositoryCustom<TEntity>
{

    //add here shared methods between repo and service
}