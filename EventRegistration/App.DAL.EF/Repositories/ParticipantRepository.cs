using App.Contracts.DAL;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class ParticipantRepository : EFBaseRepository<Participant, AppDbContext>, IParticipantRepository
{
    public ParticipantRepository(AppDbContext dataContext) : base(dataContext)
    {
    }
    
}