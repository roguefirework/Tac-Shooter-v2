namespace Shared
{
    public enum ServerToClientProtocol : ushort
    {
        JoinPlayer, 
        PlayerLeave,
        PlayerSwitchTeam,
        PlayerSwitchState,
        Max,
    }
    
    public enum ClientToServerProtocol : ushort
    {
        SwitchTeam = ServerToClientProtocol.Max + 1,
        SwitchState,
        JoinGame // String, 
    }
}