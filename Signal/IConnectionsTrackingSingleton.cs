
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HubConnections 
{
    public interface IConnectionsTrackingSingleton
    {

        Dictionary<string, string> GetConnections();
 
    }
}