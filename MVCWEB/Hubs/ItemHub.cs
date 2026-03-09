using Microsoft.AspNetCore.SignalR;
using MVCWEB.Models;

namespace MVCWEB.Hubs
{
    public class ItemHub : Hub
    {
        public async Task UpdateItem(ItemDto item)
        {
            await Clients.All.SendAsync("ReceiveItem", item);
        }
    }
}
