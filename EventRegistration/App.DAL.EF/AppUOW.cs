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

     public IEventRepository EventRepository => _eventRepository ??= new EventRepository(UowDbContext);

}