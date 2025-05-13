using App.DAL.EF;
using App.DAL.EF.Repositories;
using App.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.WebApp.UnitTests;

public class EventParticipantRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly EventParticipantRepository _repository;

    public EventParticipantRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new AppDbContext(options);

        _repository = new EventParticipantRepository(_context);
    }

    [Fact]
    public async Task Add_ShouldAddEventParticipant()
    {
        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Name = "Test Event",
            DateTime = DateTime.Now.AddDays(5),
            Location = "Test Location",
            AdditionalInfo = "Test Event Info"
        };

        var participant = new PrivatePerson
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Participant",
            PersonalCode = "12345678901",
            PaymentMethodId = Guid.NewGuid(),
            AdditionalInfo = "Participant info"
        };

        _context.Events.Add(eventEntity);
        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        var eventParticipant = new EventParticipant
        {
            Id = Guid.NewGuid(),
            EventId = eventEntity.Id,
            ParticipantId = participant.Id
        };

        _repository.Add(eventParticipant);
        await _context.SaveChangesAsync();

        var saved = await _repository.FindAsync(eventParticipant.Id);

        Assert.NotNull(saved);
        Assert.Equal(eventEntity.Id, saved!.EventId);
        Assert.Equal(participant.Id, saved.ParticipantId);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnEventParticipant()
    {
        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Name = "Find Test Event",
            DateTime = DateTime.Now.AddDays(3),
            Location = "Find Location",
            AdditionalInfo = "Find Info"
        };

        var participant = new PrivatePerson
        {
            Id = Guid.NewGuid(),
            FirstName = "Find",
            LastName = "Person",
            PersonalCode = "12345678901",
            PaymentMethodId = Guid.NewGuid(),
            AdditionalInfo = "Find participant"
        };

        _context.Events.Add(eventEntity);
        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        var eventParticipant = new EventParticipant
        {
            Id = Guid.NewGuid(),
            EventId = eventEntity.Id,
            ParticipantId = participant.Id
        };

        _context.EventParticipants.Add(eventParticipant);
        await _context.SaveChangesAsync();

        var found = await _repository.FindAsync(eventParticipant.Id);

        Assert.NotNull(found);
        Assert.Equal(eventParticipant.Id, found!.Id);
    }

    [Fact]
    public async Task Remove_ShouldDeleteEventParticipant()
    {
        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Name = "Delete Event",
            DateTime = DateTime.Now.AddDays(7),
            Location = "Delete Location",
            AdditionalInfo = "Delete Info"
        };

        var participant = new PrivatePerson
        {
            Id = Guid.NewGuid(),
            FirstName = "Delete",
            LastName = "Person",
            PersonalCode = "12345678901",
            PaymentMethodId = Guid.NewGuid(),
            AdditionalInfo = "Delete participant"
        };

        _context.Events.Add(eventEntity);
        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        var eventParticipant = new EventParticipant
        {
            Id = Guid.NewGuid(),
            EventId = eventEntity.Id,
            ParticipantId = participant.Id
        };

        _context.EventParticipants.Add(eventParticipant);
        await _context.SaveChangesAsync();

        _repository.Remove(eventParticipant);
        await _context.SaveChangesAsync();

        var deleted = await _repository.FindAsync(eventParticipant.Id);

        Assert.Null(deleted);
    }

    [Fact]
    public async Task AllAsync_ShouldReturnAllEventParticipants_ForEvent()
    {
        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Name = "All Test Event",
            DateTime = DateTime.Now.AddDays(10),
            Location = "All Location",
            AdditionalInfo = "All Event Info"
        };

        var participant1 = new PrivatePerson
        {
            Id = Guid.NewGuid(),
            FirstName = "First",
            LastName = "Participant",
            PersonalCode = "11111111111",
            PaymentMethodId = Guid.NewGuid(),
            AdditionalInfo = "Participant one"
        };

        var participant2 = new PrivatePerson
        {
            Id = Guid.NewGuid(),
            FirstName = "Second",
            LastName = "Participant",
            PersonalCode = "22222222222",
            PaymentMethodId = Guid.NewGuid(),
            AdditionalInfo = "Participant two"
        };

        _context.Events.Add(eventEntity);
        _context.Participants.AddRange(participant1, participant2);
        await _context.SaveChangesAsync();

        _context.EventParticipants.AddRange(
            new EventParticipant { Id = Guid.NewGuid(), EventId = eventEntity.Id, ParticipantId = participant1.Id },
            new EventParticipant { Id = Guid.NewGuid(), EventId = eventEntity.Id, ParticipantId = participant2.Id }
        );
        await _context.SaveChangesAsync();

        var allParticipants = await _repository.AllAsync(eventEntity.Id);

        Assert.Equal(2, allParticipants.Count());
    }
}
