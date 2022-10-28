/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System.Collections.Generic;
using Exploratorium.Net.Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Exploratorium.Net.Server
{
    /// <summary>
    /// Server specialization of <see cref="OnlineState"/>. Contains implementations for all <see cref="OnlineState"/>'s client-to-server RPCs.
    /// </summary>
    [RequireComponent(typeof(OnlineState))]
    public class OnlineStateServer : NetworkStateBehaviour
    {
        [SerializeField] private NetworkObject pawnPrefab;

        private NetworkPortal _netPortal;
        private NetworkPortalServer _serverNetPortal;
        private readonly List<NetworkPawn> _pawns = new List<NetworkPawn>();
        private OnlineState _onlineState;

        public override NetworkState ActiveState => NetworkState.Online;

        private void Awake()
        {
            Debug.Assert(pawnPrefab != null, $"{nameof(pawnPrefab)} property not assigned");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer)
            {
                enabled = false;
                Debug.LogWarning(
                    $"{nameof(OnlineStateServer)} : {nameof(OnNetworkSpawn)} ignored, {NetworkManager.LocalClientId} is not a server");
                return;
            }

            SceneManager.SetActiveScene(gameObject.scene);

            _onlineState = GetComponent<OnlineState>();

            _netPortal = FindObjectOfType<NetworkPortal>();
            _netPortal.ClientSceneChanged += OnClientSceneChanged;

            _serverNetPortal = _netPortal.GetComponent<NetworkPortalServer>();

            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;

            if (IsHost)
            {
                // host doesn't get an OnClientConnected() and other clients could be connects from last game,
                // so look for any existing connections to do initial setup
                var clients = NetworkManager.Singleton.ConnectedClientsList;
                foreach (var netClient in clients)
                    OnClientConnected(netClient.ClientId);
            }
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            DespawnPawn(clientId);
        }

        private void OnClientConnected(ulong clientId)
        {
            // server and local client only

            Debug.Log($"{nameof(OnlineStateServer)} : Client {clientId} connected");

            if (NetworkManager.SpawnManager.GetPlayerNetworkObject(clientId) == null)
                SpawnPawn(clientId);
        }

        private void OnClientSceneChanged(ulong clientId, int sceneIndex)
        {
            int serverScene = SceneManager.GetActiveScene().buildIndex;
            if (sceneIndex != serverScene)
                return;

            Debug.Log(
                $"{nameof(OnlineStateServer)} : Client {clientId} is now in scene {sceneIndex}, server_scene={serverScene}, all players in server scene={_serverNetPortal.AreAllClientsInServerScene()}");

            if (NetworkManager.SpawnManager.GetPlayerNetworkObject(clientId) == null)
                SpawnPawn(clientId);
        }

        public override void OnDestroy()
        {
            // unsubscribe (may have already been destroyed)

            if (_netPortal != null)
            {
                _netPortal.ClientSceneChanged -= OnClientSceneChanged;
            }

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            }

            base.OnDestroy();
        }


        private void DespawnPawn(ulong clientId)
        {
            Debug.Log($"{nameof(OnlineStateServer)} : {nameof(DespawnPawn)}");

            for (var i = _pawns.Count - 1; i >= 0; i--)
            {
                var pawn = _pawns[i];
                if (pawn.OwnerClientId == clientId)
                {
                    Debug.Log(
                        $"{nameof(OnlineStateServer)} : {nameof(DespawnPawn)} on {NetworkManager.LocalClientId}, isServer = {IsServer}");
                    Destroy(pawn.gameObject);
                }

                _pawns.RemoveAt(i);
            }
        }

        private void SpawnPawn(ulong clientId)
        {
            //Assert.IsTrue(worldSpawnParent, "worldSpawnParent transform is not set!");
            // Assert.IsTrue(spawnParent, "spawnParent transform is not set!");
            Debug.Log(
                $"{nameof(OnlineStateServer)} : {nameof(SpawnPawn)} on {NetworkManager.LocalClientId}, isServer = {IsServer}");

            // netObject spawn
            SceneManager.SetActiveScene(gameObject.scene);
            NetworkObject pawnObj = Instantiate(pawnPrefab);
            NetworkPawn pawn = pawnObj.GetComponent<NetworkPawn>();
            NetworkPawnServer pawnServer = pawn.GetComponent<NetworkPawnServer>();
            _pawns.Add(pawn);
            string playerName = _serverNetPortal.GetPlayerName(clientId, 0);
            pawnServer.SetName(playerName);
            pawnServer.SetRole(PawnRole.Solo);
            pawnObj.SpawnAsPlayerObject(clientId);
        }
    }
}