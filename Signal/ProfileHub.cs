using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using HubConnections;

// https://docs.microsoft.com/fr-fr/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections
namespace SignalRChat.Hubs
{
    public class ProfileHub : Hub
    {
        public Connections connectionsLake = new Connections();

        public readonly string LAKE_ROOM_NAME = "lack";
        public async Task SendMessage(string user, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }

        public async Task FindSomeone() {
            
        }

        // Call when user click on "start" and make him join the lake to date another ppl connected
        public async Task JoinLake(string nickname) {
            this.connectionsLake.AddConnection(Context.ConnectionId, nickname);
            await Groups.AddToGroupAsync(Context.ConnectionId, LAKE_ROOM_NAME);
        }

        public async Task LeaveLake() {
            this.connectionsLake.RemoveConnection(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, LAKE_ROOM_NAME);
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            this.connectionsLake.RemoveConnection(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, this.LAKE_ROOM_NAME);
            Task task = base.OnDisconnectedAsync(exception);
        }

        public override Task OnConnectedAsync()
        {
            //string name = Context.User.Identity.Name;
            //Context.User.
            //Console.WriteLine("HELLO : " + name);
            //_connections.Add(name, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

    }
}