using App.Contracts.DAL;
using App.DAL.EF.Repositories;
using Base.DAL.EF;

namespace App.DAL.EF;

public class AppUOW : EFBaseUOW<AppDbContext>, IAppUOW
{
     public AppUOW(AppDbContext dataContext) : base(dataContext)
     {
     }
     public IEventRepository? _eventRepository;
     public IEventParticipantRepository? _eventParticipantRepository;
     public IParticipantRepository? _participantRepository;
     public IPaymentMethodRepository? _paymentMethodRepository;

     public IEventRepository EventRepository => _eventRepository ??= new EventRepository(UowDbContext);
     public IEventParticipantRepository EventParticipantRepository => _eventParticipantRepository ??= new EventParticipantRepository(UowDbContext);
     public IParticipantRepository ParticipantRepository => _participantRepository ??= new ParticipantRepository(UowDbContext);
     public IPaymentMethodRepository PaymentMethodRepository => _paymentMethodRepository ??= new PaymentMethodRepository(UowDbContext);

}