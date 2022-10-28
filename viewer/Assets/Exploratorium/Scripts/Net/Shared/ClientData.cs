/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
namespace Exploratorium.Net.Shared
{
    /// <summary>
    /// Represents a single player on the game server
    /// </summary>
    public readonly struct ClientData
    {
        public readonly string ClientName; //name of the player
        public readonly ulong ClientID; //the identifying id of the client

        public ClientData(string clientName, ulong clientId)
        {
            ClientName = clientName;
            ClientID = clientId;
        }
    }
}