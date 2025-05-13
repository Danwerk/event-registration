using App.DAL.EF;
using App.DAL.EF.Repositories;
using App.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.WebApp.UnitTests
{
    public class EventRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly EventRepository _repository;

        public EventRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new EventRepository(_context);
        }

        [Fact]
        public async Task Add_ShouldAddEvent()
        {
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Test Event",
                Location = "Tallinn",
                DateTime = DateTime.UtcNow.AddDays(5),
                AdditionalInfo = "Test additional info"
            };

            _repository.Add(newEvent);
            await _context.SaveChangesAsync();

            var events = await _repository.AllAsync();

            Assert.Single(events);
            Assert.Contains(events, e => e.Name == "Test Event");
        }


        [Fact]
        public async Task FindAsync_ShouldReturnCorrectEvent()
        {
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Findable Event",
                Location = "Tartu",
                DateTime = DateTime.UtcNow.AddDays(3),
                AdditionalInfo = "Test additional info"

            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            var foundEvent = await _repository.FindAsync(newEvent.Id);

            Assert.NotNull(foundEvent);
            Assert.Equal(newEvent.Name, foundEvent!.Name);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_ShouldReturnEvent()
        {
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "First Event",
                Location = "Pärnu",
                DateTime = DateTime.UtcNow.AddDays(7),
                AdditionalInfo = "Test additional info"
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            var foundEvent = await _repository.FirstOrDefaultAsync(newEvent.Id);

            Assert.NotNull(foundEvent);
            Assert.Equal(newEvent.Id, foundEvent!.Id);
        }

        [Fact]
        public async Task Update_ShouldModifyEvent()
        {
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Location = "Viljandi",
                DateTime = DateTime.UtcNow.AddDays(2),
                AdditionalInfo = "Test additional info"
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            newEvent.Name = "New Name";
            _repository.Update(newEvent);
            await _context.SaveChangesAsync();

            var updatedEvent = await _repository.FindAsync(newEvent.Id);

            Assert.NotNull(updatedEvent);
            Assert.Equal("New Name", updatedEvent!.Name);
        }

        [Fact]
        public async Task Remove_ShouldDeleteEvent()
        {
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Event to Delete",
                Location = "Haapsalu",
                DateTime = DateTime.UtcNow.AddDays(4),
                AdditionalInfo = "Test additional info"
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            _repository.Remove(newEvent);
            await _context.SaveChangesAsync();

            var events = await _repository.AllAsync();

            Assert.Empty(events);
        }

        [Fact]
        public async Task RemoveAsync_ShouldDeleteEvent()
        {
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Event to Delete Async",
                Location = "Rakvere",
                DateTime = DateTime.UtcNow.AddDays(6),
                AdditionalInfo = "Test additional info"
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            await _repository.RemoveAsync(newEvent.Id);
            await _context.SaveChangesAsync();

            var events = await _repository.AllAsync();

            Assert.Empty(events);
        }
    }
}
