using GrainOperationAPI.Models.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace GrainOperationAPI.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendCreationEvent(CreationEventDTO creationEvent)
        {
            await Clients.All.SendAsync("CreationEventOccurred", creationEvent);
        }
    }
}
