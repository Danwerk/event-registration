using App.Domain;
using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IPaymentMethodRepository : IBaseRepository<PaymentMethod>, IParticipantRepositoryCustom<PaymentMethod>
{
    
}

public interface IPaymentMethodRepositoryCustom<TEntity>
{

    //add here shared methods between repo and service
}