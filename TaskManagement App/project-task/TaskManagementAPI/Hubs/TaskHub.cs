using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;



namespace TaskManagementAPI.Hubs
{
    [Authorize]
    public class TaskHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }
            await base.OnConnectedAsync();
        }

        public async Task SendTaskUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveTaskUpdate", message);
        }

        public async Task NotifyTaskCreated(object taskDto, string userId)
        {
            await Clients.Group($"user-{userId}").SendAsync("TaskCreated", taskDto);
        }

        public async Task NotifyTaskUpdated(object taskDto, string userId)
        {
            await Clients.Group($"user-{userId}").SendAsync("TaskUpdated", taskDto);
        }
    }
}


