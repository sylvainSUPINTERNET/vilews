using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using HubConnections;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

// https://docs.microsoft.com/fr-fr/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections
namespace SignalRChat.Hubs
{
    public class ProfileHub : Hub
    {
        private readonly ILogger _logger;

        //public Connections connectionsLake = new Connections();

        // TODO https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0
        private IConnections _connectionsLake;

        public ProfileHub(ILogger<ProfileHub> logger, IConnections connectionsLake) {
            _logger = logger;
            _connectionsLake = connectionsLake;
        }

        public readonly string LAKE_ROOM_NAME = "lack";

        /**
        @Deprecated
        */
        public async Task SendMessage(string user, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }

        public async Task FindSomeone(string PeerOfferId) {
            if ( _connectionsLake.FindSomeoneWhoIsNot(Context.ConnectionId) != "") {
                string targetConnectionId = _connectionsLake.FindSomeoneWhoIsNot(Context.ConnectionId);
                await Clients.Client(targetConnectionId).SendAsync(PeerOfferId);
                this._logger.LogInformation($" User : {Context.ConnectionId} send peer id to {targetConnectionId} ");
            } else {
                this._logger.LogInformation("Not enough user, nobody is connected, only you there !");
            }
    
        }

        // Call when user click on "start" and make him join the lake to date another ppl connected
        public async Task JoinLake(string peerId) {
            
            _connectionsLake.AddConnection(Context.ConnectionId, peerId);
            await Groups.AddToGroupAsync(Context.ConnectionId, LAKE_ROOM_NAME);
            
            this._logger.LogInformation($" User : {Context.ConnectionId} join the lake ");
            
            // If user clicked on start and so present in JoinLake, means hes ready to talk else we don't care, even hes connected to the hub
            this._logger.LogInformation($"Current Total connected and ready to talk : {_connectionsLake.GetCurrentConnectedCount()}");

            // Find someone peerId then return to the Context.ConnectionId
            string targetPeerId = _connectionsLake.FindSomeoneWhoIsNot(Context.ConnectionId);
            if (  targetPeerId != "") {
                this._logger.LogInformation($"Peer id target found : {targetPeerId}" );
                // TODO send peerIdTarget this information to the Context.ConnectionId 
                // TODO send peerId current connection to the ConnectionId ( must be implemented in findSomeoneWhoIsnot);
            } else {
                this._logger.LogInformation("Not enough users connected, you are alone !");
            }

        }

        public async Task LeaveLake() {
            _connectionsLake.RemoveConnection(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, LAKE_ROOM_NAME);

            this._logger.LogInformation($" User : {Context.ConnectionId} left the lake ");
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            _connectionsLake.RemoveConnection(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, this.LAKE_ROOM_NAME);
            
            this._logger.LogInformation($"New User disconnected : {Context.ConnectionId}");

            base.OnDisconnectedAsync(exception);
        }

        public override Task OnConnectedAsync()
        {
            this._logger.LogInformation($"New User connected : {Context.ConnectionId}");
    
            return base.OnConnectedAsync();
        }

    }
}