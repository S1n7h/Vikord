using Microsoft.AspNetCore.SignalR;
using backend.Dtos;
public class ChatHub : Hub
{
    public static void DummyFunction()
    {
        public async Task SendMessage(MessageDto responseDto)
        => await Clients.All.SendAsync("ReceivedMessage", responseDto);
    }
    
}