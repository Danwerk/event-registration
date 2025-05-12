using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Seeding;

public static class AppDataInit
{

    public static void MigrateDatabase(AppDbContext context)
    {
        context.Database.Migrate();
    }

    public static void DropDatabase(AppDbContext context)
    {
        context.Database.EnsureDeleted();
    }


        public static void SeedData(AppDbContext context)
    {
        SeedPaymentMethods(context);
        context.SaveChanges();
    }
    
    public static void SeedPaymentMethods(AppDbContext context)
    {
        if (context.PaymentMethods.Any()) return;
    
        context.PaymentMethods.Add(new PaymentMethod()
            {
                Name = "Krediitkaart"
            }
        );
        context.PaymentMethods.Add(new PaymentMethod()
            {
                Name = "Arve"
            }
        );
    }
}