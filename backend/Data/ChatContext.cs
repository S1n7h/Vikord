using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class ChatContext(DbContextOptions<ChatContext> options) : DbContext(options)
{
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => u.UserId);
    }
}
