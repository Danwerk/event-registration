using App.DAL.EF;
using App.DAL.EF.Repositories;
using App.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.WebApp.UnitTests;

public class ParticipantRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly ParticipantRepository _repository;

    public ParticipantRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new AppDbContext(options);

        _repository = new ParticipantRepository(_context);
    }

    [Fact]
    public async Task Add_ShouldAddParticipant()
    {
        var participant = new PrivatePerson
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            PersonalCode = "12345678901",
            PaymentMethodId = Guid.NewGuid(),
            AdditionalInfo = "Some info"
        };

        _repository.Add(participant);
        await _context.SaveChangesAsync();

        var saved = await _repository.FindAsync(participant.Id);

        Assert.NotNull(saved);
        Assert.Equal(participant.FirstName, ((PrivatePerson)saved!).FirstName);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnParticipant()
    {
        var participant = new LegalPerson
        {
            Id = Guid.NewGuid(),
            CompanyName = "Test Company",
            RegistryCode = "12345678",
            NumberOfAttendees = 5,
            PaymentMethodId = Guid.NewGuid(),
            AdditionalInfo = "Company info"
        };

        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        var found = await _repository.FindAsync(participant.Id);

        Assert.NotNull(found);
        Assert.Equal(participant.CompanyName, ((LegalPerson)found!).CompanyName);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnParticipant()
    {
        var participant = new PrivatePerson
        {
            Id = Guid.NewGuid(),
            FirstName = "First",
            LastName = "Last",
            PersonalCode = "12345678901",
            PaymentMethodId = Guid.NewGuid(),
            AdditionalInfo = "Info"
        };

        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        var first = await _repository.FirstOrDefaultAsync(participant.Id);

        Assert.NotNull(first);
        Assert.Equal(participant.Id, first!.Id);
    }

    [Fact]
    public async Task Remove_ShouldDeleteParticipant()
    {
        var participant = new PrivatePerson
        {
            Id = Guid.NewGuid(),
            FirstName = "Remove",
            LastName = "Me",
            PersonalCode = "12345678901",
            PaymentMethodId = Guid.NewGuid(),
            AdditionalInfo = "Info"
        };

        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        _repository.Remove(participant);
        await _context.SaveChangesAsync();

        var deleted = await _repository.FindAsync(participant.Id);

        Assert.Null(deleted);
    }
}
