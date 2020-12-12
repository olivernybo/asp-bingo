using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace asp_bingo.Web.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
			HttpContext context = Context.GetHttpContext();
			string user = context.Session.GetString("name");
			string color = context.Session.GetString("color");
            await Clients.All.SendAsync("ReceiveMessage", user, color, message);
        }
    }
}