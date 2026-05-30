using backend.Data;
using backend.Models; // Make sure to include your models namespace
using Microsoft.EntityFrameworkCore;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider
            .GetRequiredService<ChatContext>();

        // 1. Run migrations
        dbContext.Database.Migrate();

        // 2. Seed a default user if the table is empty
        if (!dbContext.Users.Any())
        {
            dbContext.Users.Add(new User { }); // This will become User 1
            dbContext.Users.Add(new User { }); // This will become User 2            
            dbContext.SaveChanges();
        }
    }
}