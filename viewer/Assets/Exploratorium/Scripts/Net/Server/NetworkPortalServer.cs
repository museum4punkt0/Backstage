/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Exploratorium.Net.Client;
using Exploratorium.Net.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Exploratorium.Net.Server
{
    /// <summary>
    /// Server logic plugin for the <see cref="NetworkPortal"/>. Contains implementations for all <see cref="NetworkPortal"/>'s client-to-server RPCs.
    /// </summary>
    public class NetworkPortalServer : MonoBehaviour
    {
        private NetworkPortal _portal;

        /// <summary>
        /// Maps a given client guid to the data for a given client player.
        /// </summary>
        private Dictionary<string, ClientData> _clientDataMap;

        /// <summary>
        /// Map to allow us to cheaply map from guid to player data.
        /// </summary>
        private Dictionary<ulong, string> _clientIDToGuid;

        // used in ApprovalCheck. This is intended as a bit of light protection against DOS attacks that rely on sending silly big buffers of garbage.
        private const int MaxConnectPayload = 1024;

        /// <summary>
        /// Keeps a list of what clients are in what scenes.
        /// </summary>
        private readonly Dictionary<ulong, int> _clientSceneMap = new Dictionary<ulong, int>();
        
        [BoxGroup(Constants.InvokedEvents)]
        [SerializeField] private VoidEvent serverStartedEvent;

        [BoxGroup(Constants.InvokedEvents)]
        [SerializeField] private VoidEvent serverStoppedEvent;
        
        /// <summary>
        /// The active server scene index.
        /// </summary>
        public int ServerScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        private void Start()
        {
            _portal = GetComponent<NetworkPortal>();
            _portal.NetworkReady += OnNetworkReady;

            // we add ApprovalCheck callback BEFORE NetworkStart to avoid spurious Netcode warning:
            // "No ConnectionApproval callback defined. Connection approval will timeout"
            _portal.Manager.ConnectionApprovalCallback += ApprovalCheck;
            _portal.Manager.OnServerStarted += ServerStartedHandler;
            _clientDataMap = new Dictionary<string, ClientData>();
            _clientIDToGuid = new Dictionary<ulong, string>();
        }

        private void OnDestroy()
        {
            if (_portal != null)
            {
                _portal.NetworkReady -= OnNetworkReady;

                if (_portal.Manager != null)
                {
                    _portal.Manager.ConnectionApprovalCallback -= ApprovalCheck;
                    _portal.Manager.OnServerStarted -= ServerStartedHandler;
                }
            }
        }

        private void OnNetworkReady()
        {
            if (!_portal.Manager.IsServer)
            {
                enabled = false;
                Debug.Log($"{nameof(NetworkPortalServer)} : Disabled (not a server)");
                return;
            }
            
            Debug.Log($"{nameof(NetworkPortalServer)} : Ready");



            //O__O if adding any event registrations here, please add an unregistration in OnClientDisconnect.
            _portal.UserDisconnectRequested += OnUserDisconnectRequest;
            _portal.Manager.OnClientDisconnectCallback += OnClientDisconnect;
            _portal.ClientSceneChanged += OnClientSceneChanged;

            //The server always advances to Lobby immediately on start. Different games
            //may do this differently.
            _portal.Manager.SceneManager.LoadScene(_portal.OnlineScene, LoadSceneMode.Single);

            if (_portal.Manager.IsHost)
            {
                _clientSceneMap[_portal.Manager.LocalClientId] = ServerScene;
            }
        }

        /// <summary>
        /// Handles the case where <see cref="NetworkManager"/> has told us a client has disconnected. This includes ourselves, if we're the host,
        /// and the server is stopped."
        /// </summary>
        private void OnClientDisconnect(ulong clientId)
        {
            Debug.Log($"{nameof(OnClientDisconnect)}");

            _clientSceneMap.Remove(clientId);
            if (_clientIDToGuid.TryGetValue(clientId, out var guid))
            {
                _clientIDToGuid.Remove(clientId);

                if (_clientDataMap[guid].ClientID == clientId)
                {
                    //be careful to only remove the ClientData if it is associated with THIS clientId; in a case where a new connection
                    //for the same GUID kicks the old connection, this could get complicated. In a game that fully supported the reconnect flow,
                    //we would NOT remove ClientData here, but instead time it out after a certain period, since the whole point of it is
                    //to remember client information on a per-guid basis after the connection has been lost.
                    _clientDataMap.Remove(guid);
                }
            }

            if (clientId == _portal.Manager.LocalClientId)
            {
                //the ServerGameNetPortal may be initialized again, which will cause its NetworkStart to be called again.
                //Consequently we need to unregister anything we registered, when the NetworkManager is shutting down.
                _portal.UserDisconnectRequested -= OnUserDisconnectRequest;
                _portal.Manager.OnClientDisconnectCallback -= OnClientDisconnect;
                _portal.ClientSceneChanged -= OnClientSceneChanged;
            }
        }

        private void OnClientSceneChanged(ulong clientId, int sceneIndex)
        {
            _clientSceneMap[clientId] = sceneIndex;
            Debug.Log($"{nameof(NetworkPortalServer)} : Client {clientId} changed to scene {sceneIndex}");
        }

        /// <summary>
        /// Handles the flow when a user has requested a disconnect via UI (which can be invoked on the Host, and thus must be
        /// handled in server code). 
        /// </summary>
        private void OnUserDisconnectRequest()
        {
            if (_portal.Manager.IsHost)
            {
                Debug.Log($"{nameof(NetworkPortalServer)} : Stopping host in response to disconnect request");
                _portal.Manager.Shutdown();
            }
            else if (_portal.Manager.IsServer)
            {
                Debug.Log($"{nameof(NetworkPortalServer)} : Stopping server in response to disconnect request");
                _portal.Manager.Shutdown();
            }

            Clear();
            if (serverStoppedEvent != null)
                serverStoppedEvent.Raise();
            
            // fall back to offline scene
            SceneManager.LoadScene(_portal.OfflineScene, LoadSceneMode.Single);
        }

        private void Clear()
        {
            //resets all our runtime state.
            _clientDataMap.Clear();
            _clientIDToGuid.Clear();
            _clientSceneMap.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"> guid of the client whose data is requested</param>
        /// <returns>Player data struct matching the given ID</returns>
        public bool AreAllClientsInServerScene()
        {
            foreach (var kvp in _clientSceneMap)
            {
                if (kvp.Value != ServerScene)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if the given client is currently in the server scene.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public bool IsClientInServerScene(ulong clientId)
        {
            return _clientSceneMap.TryGetValue(clientId, out int clientScene) && clientScene == ServerScene;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="clientId"> guid of the client whose data is requested</param>
        /// <returns>Player data struct matching the given ID</returns>
        public ClientData GetPlayerData(ulong clientId)
        {
            //First see if we have a guid matching the clientID given.

            if (_clientIDToGuid.TryGetValue(clientId, out string clientguid))
            {
                if (_clientDataMap.TryGetValue(clientguid, out ClientData data))
                {
                    return data;
                }
                else
                {
                    Debug.Log($"{nameof(NetworkPortalServer)} : No PlayerData of matching guid found");
                }
            }
            else
            {
                Debug.Log($"{nameof(NetworkPortalServer)} : No client guid found mapped to the given client ID");
            }

            return default;
        }

        /// <summary>
        /// Convenience method to get player name from player data
        /// Returns name in data or default name using playerNum
        /// </summary>
        public string GetPlayerName(ulong clientId, int playerNum)
        {
            ClientData playerData = GetPlayerData(clientId);
            return playerData.ClientName ?? $"Pawn{playerNum}/{clientId}";
        }


        /// <summary>
        /// This logic plugs into the <see cref="NetworkManager.ConnectionApprovalCallback"/> exposed by <see cref="NetworkManager"/>, and is run every time a client connects to us.
        /// See <see cref="NetworkPortalClient.StartClient"/> for the complementary logic that runs when the client starts its connection.
        /// </summary>
        private void ApprovalCheck(
            byte[] connectionData,
            ulong clientId,
            NetworkManager.ConnectionApprovedDelegate callback
        )
        {
            if (connectionData.Length > MaxConnectPayload)
            {
                callback(false, 0, false, null, null);
                return;
            }

            string payload = System.Text.Encoding.UTF8.GetString(connectionData);
            var connectionPayload =
                JsonUtility.FromJson<ConnectionPayload>(
                    payload
                ); // https://docs.unity3d.com/2020.2/Documentation/Manual/JSONSerialization.html
            int clientScene = connectionPayload.clientScene;
            
            Debug.Log($"{nameof(NetworkPortalServer)} : Host ApprovalCheck: connecting client GUID: " + connectionPayload.clientGUID);

            //TODO: GOMPS-78. We are saving the GUID, but we have more to do to fully support a reconnect flow (where you get your same character back after disconnect/reconnect).

            ConnectStatus gameReturnStatus = ConnectStatus.Success;

            //Test for Duplicate Login. 
            if (_clientDataMap.ContainsKey(connectionPayload.clientGUID))
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log(
                        $"{nameof(NetworkPortalServer)} : Client GUID {connectionPayload.clientGUID} already exists. Because this is a debug build, we will still accept the connection"
                    );
                    while (_clientDataMap.ContainsKey(connectionPayload.clientGUID))
                    {
                        connectionPayload.clientGUID += "_Secondary";
                    }
                }
                else
                {
                    ulong oldClientId = _clientDataMap[connectionPayload.clientGUID].ClientID;
                    StartCoroutine(WaitToDisconnectClient(oldClientId, ConnectStatus.LoggedInAgain));
                }
            }

            //Test for over-capacity Login.
            if (_clientDataMap.Count >= _portal.MaxPlayers)
            {
                gameReturnStatus = ConnectStatus.ServerFull;
            }

            //Populate our dictionaries with the playerData
            if (gameReturnStatus == ConnectStatus.Success)
            {
                _clientSceneMap[clientId] = clientScene;
                _clientIDToGuid[clientId] = connectionPayload.clientGUID;
                _clientDataMap[connectionPayload.clientGUID] = new ClientData(connectionPayload.pawnName, clientId);
            }

            callback(false, 0, true, null, null);

            //TODO: This must be done after the callback for now. In the future we expect Netcode to allow us to return more information as part of
            //the approval callback, so that we can provide more context on a reject. In the meantime we must provide the extra information ourselves,
            //and then manually close down the connection.
            _portal.ServerToClientConnectResult(clientId, gameReturnStatus);
            if (gameReturnStatus != ConnectStatus.Success)
            {
                StartCoroutine(WaitToDisconnectClient(clientId, gameReturnStatus));
            }
        }

        private IEnumerator WaitToDisconnectClient(ulong clientId, ConnectStatus reason)
        {
            _portal.ServerToClientSetDisconnectReason(clientId, reason);

            // TODO-FIXME: Once this is solved: Issue 796 Unity-Technologies/com.unity.multiplayer.mlapi#796
            // this wait is a workaround to give the client time to receive the above RPC before closing the connection
            yield return new WaitForSeconds(0);

            EjectClient(clientId);
        }

        /// <summary>
        /// This method will summarily remove a player connection, as well as its controlled object.
        /// </summary>
        /// <param name="clientId">the ID of the client to boot.</param>
        public void EjectClient(ulong clientId)
        {
            var netObj = _portal.Manager.SpawnManager.GetPlayerNetworkObject(clientId);
            if (netObj)
            {
                // TODO-FIXME: Netcode Issue #795. Should not need to explicitly despawn player objects.
                netObj.Despawn(true);
            }

            _portal.Manager.DisconnectClient(clientId);
        }

        /// <summary>
        /// Called after the server is created-  This is primarily meant for the host server to clean up or handle/set state as its starting up
        /// </summary>
        private void ServerStartedHandler()
        {
            _clientDataMap.Add("host_guid", new ClientData(_portal.PlayerName, _portal.Manager.LocalClientId));
            _clientIDToGuid.Add(_portal.Manager.LocalClientId, "host_guid");
            Debug.Log($"{nameof(NetworkPortalServer)} : Server started");
            serverStartedEvent.Raise();
        }


        /// <summary>
        /// Wraps the invocation of <see cref="NetworkManager.StartServer"/>.
        /// </summary>
        /// <remarks>
        /// This method must be static because, when it is invoked, the server still doesn't know it's a server yet, and in particular, NetworkPortal hasn't
        /// yet initialized its client and server logic objects (which it does in NetworkStart, based on the role that the current player is performing).
        /// </remarks>
        /// <param name="portal"> </param>
        /// <param name="ipaddress">the IP address of the host to connect to. (currently IPV4 only)</param>
        /// <param name="port">The port of the host to connect to. </param>
        public static void StartServer(NetworkPortal portal, string ipaddress, ushort port)
        {
            if (NetworkPortal.StaticManager.NetworkConfig.NetworkTransport is UnityTransport utp)
            {
                utp.ConnectionData.Address = ipaddress;
                utp.ConnectionData.Port = port;
                utp.ConnectionData.ServerListenAddress = ipaddress;

            }
            else
            {
                Debug.LogError($"{nameof(NetworkPortalServer)} : Unsupported transport");
                return;
            }

            NetworkPortal.StaticManager.StartServer();
            portal.RegisterServerMessageHandlers();
        }
    }
}