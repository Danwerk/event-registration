using App.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class AppDbContext : IdentityDbContext
{
    public DbSet<Event> Events { get; set; } = default!;
    public DbSet<Participant> Participants { get; set; } = default!;
    public DbSet<PrivatePerson> PrivatePersons { get; set; } = default!;
    public DbSet<LegalPerson> LegalPersons { get; set; } = default!;
    public DbSet<PaymentMethod> PaymentMethods { get; set; } = default!;
    public DbSet<EventParticipant> EventParticipants { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // disable cascade delete
        foreach (var foreignKey in builder.Model
                     .GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
       
    }
}