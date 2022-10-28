/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */

using Exploratorium.Frontend;
using Exploratorium.Net.Shared;
using Unity.Netcode;
using UnityEngine;

namespace Exploratorium.Net.Client
{
    /// <summary>
    /// Client specialization of <see cref="OnlineState"/>. Contains implementations for all <see cref="OnlineState"/>'s server-to-client RPCs.
    /// </summary>
    [RequireComponent(typeof(OnlineState))]
    public class OnlineStateClient : NetworkStateBehaviour
    {
        [SerializeField] private Flow flow;
        [SerializeField] private NetworkPawn pawn;
        
        public override NetworkState ActiveState => NetworkState.Online;

        private OnlineState _onlineState;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsClient)
            {
                enabled = false;
                return;
            }

            _onlineState = GetComponent<OnlineState>();
        }
        
        
        private void OnClientConnected(ulong clientId)
        {
            // server and local client only

            Debug.Log($"{nameof(OnlineStateClient)} : client={clientId} connected");
        }
    }
}