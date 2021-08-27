using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HubConnections 
{
    public class ConnectionsTracking : IConnectionsTrackingSingleton
    {
        public Dictionary<string, string> currentConnected { get; set;} = new Dictionary<string, string>();

        public ConnectionsTracking() { }

        public Dictionary<string, string> GetConnections()
        {
            return this.currentConnected;
        }
    
    }
}