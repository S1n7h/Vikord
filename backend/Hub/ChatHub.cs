using Microsoft.AspNetCore.SignalR;
using backend.Dtos;

public class ChatHub: Hub
{
    private readonly IHubContext<Hub> _myHubContext;

    public ChatHub(IHubContext<Hub> myHubContext)
    {
        _myHubContext= myHubContext;
    }

    public async Task SendMessage(MessageDto responseDto)
    {
        await _myHubContext.Clients.All.SendAsync("Update", responseDto);
    }      
}
