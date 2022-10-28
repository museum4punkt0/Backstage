/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */

using Exploratorium.Net.Shared;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Exploratorium.Net.Server
{
    [RequireComponent(typeof(NetworkPawn))]
    public class NetworkPawnServer : NetworkBehaviour
    {
        [SerializeField] private NetworkPawn pawn;
        //[SerializeField] private OnlineState onlineState;
        
        private void Awake()
        {
            Debug.Assert(pawn != null, $"{nameof(pawn)} property is unassigned", this);
            pawn.RoleRequestedServer += OnRoleRequested;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }

            //pawn.TransitionReportedServer += OnTransitionReported;

            if (pawn == null)
                pawn = GetComponent<NetworkPawn>();

            if (pawn.n_role.Value == PawnRole.None)
            {
                Debug.LogWarning($"{nameof(NetworkPawnServer)}#{NetworkObjectId} : Pawn {pawn.name}#{pawn.NetworkObjectId} spawned without a role");
            }

        }

        public void SetName(FixedString32Bytes value) => pawn.n_name.Value = value;

        public void SetRole(PawnRole value) => pawn.n_role.Value = value;

        private FixedString32Bytes Name => pawn.n_name.Value;
        private PawnRole Role => pawn.n_role.Value;

        private void OnRoleRequested(ulong clientId, PawnRole role)
        {
            if (!IsServer)
            {
                enabled = false;
                Debug.LogWarning(
                    $"{nameof(NetworkPawnServer)}#{NetworkObjectId} : {nameof(OnRoleRequested)} ignored, {NetworkManager.LocalClientId} is not a server");
                return;
            }

            Debug.Log($"{nameof(NetworkPawnServer)}#{NetworkObjectId} : {clientId} has requested role {role}");

            // conform the states of all other pawns
            foreach (NetworkClient client in NetworkManager.ConnectedClientsList)
            {
                NetworkPawnServer pawnServer = client.PlayerObject.GetComponent<NetworkPawnServer>();
                if (pawnServer.OwnerClientId == clientId)
                    continue;
                
                // eject any previously held controller role on other pawns
                if (pawnServer.Role == PawnRole.Controller)
                    pawnServer.SetRole(PawnRole.Solo);
                
                // reapply observer roles to reset them
                if (pawnServer.Role == PawnRole.Observer)
                    pawnServer.SetRole(PawnRole.Observer);
            }
            
            /*
             
            if (role == PawnRole.Controller)
            {
                // eject previously held controller role
                foreach (var client in NetworkManager.ConnectedClientsList)
                {
                    NetworkPawnServer clientPawnServer = client.PlayerObject.GetComponent<NetworkPawnServer>();
                    if (clientPawnServer.Role == PawnRole.Controller && clientPawnServer.OwnerClientId != clientId)
                        clientPawnServer.SetRole(PawnRole.Solo);
                }
            }
            
            */

            // finally apply the role for this client
            SetRole(role);
        }
    }
}