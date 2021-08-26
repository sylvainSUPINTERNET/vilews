using System.Collections.Generic;
using System.Linq;

namespace HubConnections 
{
    public class Connections 
    {
        public Dictionary<string, string> currentConnected { get; }

        public void AddConnection( string connectionId, string nickname) {
            this.currentConnected.Add(connectionId, nickname); // avoid collision
        }

        public void RemoveConnection(string connectionId) {
            this.currentConnected.Remove(connectionId);
        }

        // Looking for connectionId ( not himself )
        public string FindSomeoneWhoIsNot( string connectionId ) {
            Dictionary<string, string> dict = this.currentConnected.Where( el => el.Key != connectionId).ToDictionary(el => el.Key, el => el.Value);
            if ( dict.Count == 0 ) {
                return "";
            } else {
                return dict.First().Key;
            }
        }   
    }
}