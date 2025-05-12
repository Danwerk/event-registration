using App.Contracts.DAL;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class PaymentMethodRepository : EFBaseRepository<PaymentMethod, AppDbContext>, IPaymentMethodRepository
{
    public PaymentMethodRepository(AppDbContext dataContext) : base(dataContext)
    {
    }
    
}