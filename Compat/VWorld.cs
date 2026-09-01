using Unity.Entities;

namespace BloodyMerchant.Compat;

/// <summary>
/// Drop-in replacement for Bloodstone.API.VWorld.
///
/// Bloodstone is abandoned. Its network serialization hooks call
/// Stunlock.Network.NetBufferOut.Write and NetBufferIn.ReadUInt32, which no longer
/// exist in V Rising 1.1.x. The result is a MissingMethodException on every single
/// server network event - chat commands stop working, achievement rewards cannot be
/// claimed, items cannot be dropped or moved, and clients crash to desktop on connect.
///
/// BloodyMerchant only ever used Bloodstone to get the server World, so we fetch it
/// straight from Unity ECS instead and drop the dependency entirely.
/// </summary>
public static class VWorld
{
    private static World _server;

    public static World Server
    {
        get
        {
            if (_server != null && _server.IsCreated) return _server;
            _server = GetWorld("Server");
            return _server;
        }
    }

    public static World Game => Server;
    public static bool IsServer => true;
    public static bool IsClient => false;

    private static World GetWorld(string name)
    {
        foreach (var world in World.s_AllWorlds)
        {
            if (world.Name == name) return world;
        }
        return null;
    }
}
