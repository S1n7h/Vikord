namespace backend.Models;
public class User
{
    public int Id {get; set;}

    public List<Message> AllMessages { get; set; } = new();
}