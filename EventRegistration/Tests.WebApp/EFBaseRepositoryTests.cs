using System;
using System.Linq;
using System.Threading.Tasks;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Xunit;

// Dummy Entity
public class TestEntity : Domain.Contracts.Base.IDomainEntityId
{
    public Guid Id { get; set; }
}

// Dummy DbContext
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    
    public DbSet<TestEntity> TestEntities { get; set; } = default!;
}

// Tests
public class EFBaseRepositoryTests
{
    private readonly TestDbContext _dbContext;
    private readonly EFBaseRepository<TestEntity, TestDbContext> _repository;

    public EFBaseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // iga test oma DB
            .Options;

        _dbContext = new TestDbContext(options);
        _repository = new EFBaseRepository<TestEntity, TestDbContext>(_dbContext);
    }

    [Fact]
    public async Task Add_AddsEntity()
    {
        var entity = new TestEntity { Id = Guid.NewGuid() };

        _repository.Add(entity);
        await _dbContext.SaveChangesAsync();

        var found = await _repository.FindAsync(entity.Id);
        Assert.NotNull(found);
        Assert.Equal(entity.Id, found!.Id);
    }

    [Fact]
    public async Task AllAsync_ReturnsAllEntities()
    {
        _repository.Add(new TestEntity { Id = Guid.NewGuid() });
        _repository.Add(new TestEntity { Id = Guid.NewGuid() });
        await _dbContext.SaveChangesAsync();

        var all = await _repository.AllAsync();
        Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task FindAsync_ReturnsCorrectEntity()
    {
        var id = Guid.NewGuid();
        _repository.Add(new TestEntity { Id = id });
        await _dbContext.SaveChangesAsync();

        var found = await _repository.FindAsync(id);
        Assert.NotNull(found);
        Assert.Equal(id, found!.Id);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ReturnsCorrectEntity()
    {
        var id = Guid.NewGuid();
        _repository.Add(new TestEntity { Id = id });
        await _dbContext.SaveChangesAsync();

        var found = await _repository.FirstOrDefaultAsync(id);
        Assert.NotNull(found);
        Assert.Equal(id, found!.Id);
    }

    [Fact]
    public async Task RemoveAsync_RemovesEntity()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity { Id = id };
        _repository.Add(entity);
        await _dbContext.SaveChangesAsync();

        var removed = await _repository.RemoveAsync(id);
        await _dbContext.SaveChangesAsync();

        var found = await _repository.FindAsync(id);
        Assert.Null(found);
        Assert.Equal(id, removed!.Id);
    }

    [Fact]
    public async Task Update_UpdatesEntity()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity { Id = id };
        _repository.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Simuleerime muutust
        var updatedEntity = await _repository.FindAsync(id);
        updatedEntity!.Id = id; 

        _repository.Update(updatedEntity);
        await _dbContext.SaveChangesAsync();

        var found = await _repository.FindAsync(id);
        Assert.NotNull(found);
        Assert.Equal(id, found!.Id);
    }
}
