using Microsoft.AspNetCore.SignalR;

namespace OnlineChatMvc.Hubs
{
    public class ChatHub : Hub
    {
       public ChatHub() 
        {

        }

        public async Task Send(string message)
        {
            await Clients.All.SendAsync("Receive", message);
        }

    }
}
