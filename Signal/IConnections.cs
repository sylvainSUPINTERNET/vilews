

using System.Collections.Generic;

namespace HubConnections 
{
    public interface IConnections
    {

        void AddConnection( string connectionId, string peerId);
        void RemoveConnection(string connectionId);
        string FindSomeoneWhoIsNot( string connectionId );

        string GetPeerIdForConnectionId( string connectionId ) ;

        int GetCurrentConnectedCount();



    }
}
