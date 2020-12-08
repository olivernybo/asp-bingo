using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace asp_bingo.Web.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
			string user = Context.GetHttpContext().Session.GetString("name");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}