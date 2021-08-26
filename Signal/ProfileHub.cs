using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using HubConnections;
using Microsoft.Extensions.Logging;

// https://docs.microsoft.com/fr-fr/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections
namespace SignalRChat.Hubs
{
    public class ProfileHub : Hub
    {
        private readonly ILogger _logger;

        public Connections connectionsLake;

        public ProfileHub(ILogger<ProfileHub> logger) {
            _logger = logger;
            connectionsLake =  new Connections();
        }

        public readonly string LAKE_ROOM_NAME = "lack";
        public async Task SendMessage(string user, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }

        public async Task FindSomeone(string PeerOfferId) {
            if ( this.connectionsLake.FindSomeoneWhoIsNot(Context.ConnectionId) != "") {
                string targetConnectionId = connectionsLake.FindSomeoneWhoIsNot(Context.ConnectionId);
                await Clients.Client(targetConnectionId).SendAsync(PeerOfferId);
                this._logger.LogInformation($" User : {Context.ConnectionId} send peer id to {targetConnectionId} ");
            } else {
                this._logger.LogInformation("Not enough user, nobody is connected, only you there !");
            }
    
        }

        // Call when user click on "start" and make him join the lake to date another ppl connected
        public async Task JoinLake(string nickname) {
            this.connectionsLake.AddConnection(Context.ConnectionId, nickname);
            await Groups.AddToGroupAsync(Context.ConnectionId, LAKE_ROOM_NAME);
            
            this._logger.LogInformation($" User : {Context.ConnectionId} join the lake ");
        }

        public async Task LeaveLake() {
            this.connectionsLake.RemoveConnection(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, LAKE_ROOM_NAME);

            this._logger.LogInformation($" User : {Context.ConnectionId} left the lake ");
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            this.connectionsLake.RemoveConnection(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, this.LAKE_ROOM_NAME);
            
            this._logger.LogInformation($"New User disconnected : {Context.ConnectionId}");

            Task task = base.OnDisconnectedAsync(exception);
        }

        public override Task OnConnectedAsync()
        {
            this._logger.LogInformation($"New User connected : {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

    }
}