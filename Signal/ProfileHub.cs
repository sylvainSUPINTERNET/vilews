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
        
        public async Task FindSomeone(string PeerOfferId) {
            if ( _connectionsLake.FindSomeoneWhoIsNot(Context.ConnectionId) != "") {
                // Send to the target the peerId from the sender
                string targetConnectionId = _connectionsLake.FindSomeoneWhoIsNot(Context.ConnectionId);
                await Clients.Client(targetConnectionId).SendAsync("newPeerOffer",PeerOfferId);
                this._logger.LogInformation($" User : {Context.ConnectionId} send peer id to {targetConnectionId} ");

                // Send back to the sender the peerId found
                await Clients.Client(Context.ConnectionId).SendAsync("newPeerOfferSend", _connectionsLake.GetPeerIdForConnectionId(Context.ConnectionId));

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