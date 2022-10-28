/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */

using Exploratorium.Net.Shared;

namespace Exploratorium.Net.Client
{
    /// <summary>
    /// Game Logic that runs when sitting at the MainMenu. This is likely to be "nothing", as no game has been started. But it is
    /// nonetheless important to have a game state, as the <see cref="NetworkStateBehaviour"/> system requires that all scenes have states. 
    /// </summary>
    public class OfflineStateClient : NetworkStateBehaviour
    {
        public override NetworkState ActiveState => NetworkState.Offline;

        public override void OnNetworkSpawn()
        {
            //note: this code won't ever run, because there is no network connection at the main menu screen.
            //fortunately we know you are a client, because all players are clients when sitting at the main menu screen. 
        }
    }
}