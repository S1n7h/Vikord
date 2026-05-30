using backend.Models;

public class Message
{
    public int Id { get; set; }

    public required string Content { get; set; }

    public int UserId { get; set; }  
    public User User { get; set; } = null!;  // navigation
}