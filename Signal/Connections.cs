using System;
using System.Collections.Generic;
using System.Linq;

namespace HubConnections 
{
    public class Connections : IConnections
    {
        public IConnectionsTrackingSingleton ic;

        public Connections(IConnectionsTrackingSingleton connectionsTrackingSingleton) {
            ic = connectionsTrackingSingleton;
        }

        public int GetCurrentConnectedCount () {
            return ic.GetConnections().Count;
        }
        public void AddConnection( string connectionId, string nickname) {
            ic.GetConnections().Add(connectionId, nickname); // avoid collision
        }

        public void RemoveConnection(string connectionId) {
            ic.GetConnections().Remove(connectionId);
        }

        // Looking for connectionId ( not himself )
        // Key ( Context.ConnectionId)
        // Value (peerId)
        public string FindSomeoneWhoIsNot( string connectionId ) {
            Dictionary<string, string> dict = ic.GetConnections().Where( el => el.Key != connectionId).ToDictionary(el => el.Key, el => el.Value);
            if ( dict.Count == 0 ) {
                return "";
            } else {
                return dict.ElementAt(new Random().Next(0, dict.Count - 1)).Key;
            }
        }
        
        public string GetPeerIdForConnectionId ( string connectionId ) {
            Dictionary<string, string> dict = ic.GetConnections().Where( el => el.Key == connectionId).ToDictionary(el => el.Key, el => el.Value);
            return dict.First().Value;
        }
    }
}