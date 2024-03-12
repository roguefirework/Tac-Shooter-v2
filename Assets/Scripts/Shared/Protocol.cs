namespace Shared
{
    public enum ServerToClientProtocol : ushort
    {
        JoinPlayer, // Join
        JoinTeam,
        
        Max,
    }
    
    public enum ClientToServerProtocol : ushort
    {
        SwitchTeam = ServerToClientProtocol.Max + 1,
        JoinGame // String, 
    }
}