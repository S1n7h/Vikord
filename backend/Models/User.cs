namespace backend.Models;
public class User
{
    /* public int Id {get; set;} */
    public string UserId { get; set; } = null!;
    public string GlobalName { get; set; } = null!;
    public List<Message> AllMessages { get; set; } = new();
}