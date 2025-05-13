using App.DAL.EF;
using App.DAL.EF.Repositories;
using App.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.WebApp.UnitTests;

public class PaymentMethodRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly PaymentMethodRepository _repository;

    public PaymentMethodRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new AppDbContext(options);

        _repository = new PaymentMethodRepository(_context);
    }

    [Fact]
    public async Task Add_ShouldAddPaymentMethod()
    {
        var paymentMethod = new PaymentMethod
        {
            Id = Guid.NewGuid(),
            Name = "Bank Transfer"
        };

        _repository.Add(paymentMethod);
        await _context.SaveChangesAsync();

        var saved = await _repository.FindAsync(paymentMethod.Id);

        Assert.NotNull(saved);
        Assert.Equal(paymentMethod.Name, saved!.Name);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnPaymentMethod()
    {
        var paymentMethod = new PaymentMethod
        {
            Id = Guid.NewGuid(),
            Name = "Cash"
        };

        _context.PaymentMethods.Add(paymentMethod);
        await _context.SaveChangesAsync();

        var found = await _repository.FindAsync(paymentMethod.Id);

        Assert.NotNull(found);
        Assert.Equal(paymentMethod.Name, found!.Name);
    }

    [Fact]
    public async Task Remove_ShouldDeletePaymentMethod()
    {
        var paymentMethod = new PaymentMethod
        {
            Id = Guid.NewGuid(),
            Name = "Card"
        };

        _context.PaymentMethods.Add(paymentMethod);
        await _context.SaveChangesAsync();

        _repository.Remove(paymentMethod);
        await _context.SaveChangesAsync();

        var deleted = await _repository.FindAsync(paymentMethod.Id);

        Assert.Null(deleted);
    }

    [Fact]
    public async Task AllAsync_ShouldReturnAllPaymentMethods()
    {
        _context.PaymentMethods.AddRange(
            new PaymentMethod { Id = Guid.NewGuid(), Name = "Transfer" },
            new PaymentMethod { Id = Guid.NewGuid(), Name = "Cash" }
        );
        await _context.SaveChangesAsync();

        var allMethods = await _repository.AllAsync();

        Assert.Equal(2, allMethods.Count());
    }
}
