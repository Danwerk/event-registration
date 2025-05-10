using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IAppUOW : IBaseUOW
{
    // List your repositories here
    IEventRepository EventRepository { get; }
    IEventParticipantRepository EventParticipantRepository { get; }
    IParticipantRepository ParticipantRepository { get; }
}