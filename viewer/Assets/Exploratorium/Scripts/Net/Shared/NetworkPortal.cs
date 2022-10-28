/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Exploratorium.Net.Shared
{
    public enum ConnectStatus
    {
        Undefined,
        Timeout,
        Success, //client successfully connected. This may also be a successful reconnect.
        ServerFull, //can't join, server is already at capacity.
        LoggedInAgain, //logged in on a separate client, causing this one to be kicked out.
        UserRequestedDisconnect, //Intentional Disconnect triggered by the user. 
        GenericDisconnect, //server disconnected, but no specific reason given.
    }

    public enum OnlineMode
    {
        IpHost = 0, // The server is hosted directly and clients can join by ip address.
        Relay = 1, // The server is hosted over a relay server and clients join by entering a room name.
    }

    [Serializable]
    public class ConnectionPayload
    {
        public string clientGUID;
        public int clientScene = -1;
        [FormerlySerializedAs("playerName")] public string pawnName;
    }

    /// <summary>
    /// The NetworkPortal is the general purpose entry-point for game network messages between the client and server. It is available
    /// as soon as the initial network connection has completed, and persists across all scenes. Its purpose is to move non-GameObject-specific
    /// methods between server and client. Generally these have to do with connection, and gameplay conditions.
    /// </summary>
    public class NetworkPortal : MonoBehaviour
    {
        /// <summary>
        /// the name of the player chosen at game start
        /// </summary>
        private string _playerName;
        [SerializeField] private LoadSceneMode sceneSyncMode;
        [SerializeField] private string offlineScene = "Offline";
        [SerializeField] private string onlineScene = "Online";
        [Min(1)]
        [SerializeField] private int maxPlayers = 1;

        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }

        public bool IsRunning => Manager.IsServer || Manager.IsClient;

        public string OnlineScene => onlineScene;

        public string OfflineScene => offlineScene;

        public int MaxPlayers => maxPlayers;
        public NetworkManager Manager => NetworkManager.Singleton;
        public static NetworkManager StaticManager => NetworkManager.Singleton;

        /// <summary>
        /// This event is fired when Netcode has reported that it has finished initialization, and is ready for
        /// business, equivalent to OnServerStarted on the server, and OnClientConnected on the client.
        /// </summary>
        public event Action NetworkReady;

        /// <summary>
        /// This event contains the game-level results of the ApprovalCheck carried out by the server, and is fired
        /// immediately after the socket connection completing. It won't fire in the event of a socket level failure.
        /// </summary>
        public event Action<ConnectStatus> Connected;

        /// <summary>
        /// This event relays a ConnectStatus sent from the server to the client. The server will invoke this to provide extra
        /// context about an upcoming network Disconnect.
        /// </summary>
        public event Action<ConnectStatus> DisconnectReasonReceived;

        /// <summary>
        /// raised when a client has changed scenes. Returns the ClientID and the new scene the client has entered, by index.
        /// </summary>
        public event Action<ulong, int> ClientSceneChanged;

        /// <summary>
        /// This fires in response to NetworkPortal.RequestDisconnect. It's a local signal (not from the network), indicating that
        /// the user has requested a disconnect. 
        /// </summary>
        public event Action UserDisconnectRequested;


        private void Awake()
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(offlineScene), $"{nameof(offlineScene)} property is unassigned");
            Debug.Assert(!string.IsNullOrWhiteSpace(onlineScene), $"{nameof(onlineScene)} property is unassigned");
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            //we synthesize a "NetworkStart" event for the NetworkManager out of existing events. At some point
            //we expect NetworkManager will expose an event like this itself.
            Manager.OnServerStarted += OnNetworkReady;
            Manager.OnClientConnectedCallback += ClientNetworkReadyWrapper;

            // we can register message handlers only after the NetworkManager has been initialized, which happens on
            // StartHost, -Server, -Client
        }

        private void OnDestroy()
        {
            if (Manager != null)
            {
                Manager.OnServerStarted -= OnNetworkReady;
                Manager.OnClientConnectedCallback -= ClientNetworkReadyWrapper;
            }

            UnregisterClientMessageHandlers();
            UnregisterServerMessageHandlers();
        }


        private void ClientNetworkReadyWrapper(ulong clientId)
        {
            if (clientId == Manager.LocalClientId)
            {
                OnNetworkReady();
            }
        }

        internal void RegisterClientMessageHandlers()
        {
            Manager.CustomMessagingManager.RegisterNamedMessageHandler(
                nameof(ServerToClientConnectResult),
                (senderClientId, reader) =>
                {
                    reader.ReadValueSafe(out ConnectStatus status);
                    Connected?.Invoke(status);
                }
            );


            Manager.CustomMessagingManager.RegisterNamedMessageHandler(
                nameof(ServerToClientSetDisconnectReason),
                (senderClientId, reader) =>
                {
                    reader.ReadValueSafe(out ConnectStatus status);
                    DisconnectReasonReceived?.Invoke(status);
                }
            );
        }

        internal void RegisterServerMessageHandlers()
        {
            Manager.CustomMessagingManager.RegisterNamedMessageHandler(
                nameof(ClientToServerSceneChanged),
                (senderClientId, reader) =>
                {
                    reader.ReadValueSafe(out int sceneIndex);
                    ClientSceneChanged?.Invoke(senderClientId, sceneIndex);
                }
            );
        }

        private void UnregisterClientMessageHandlers()
        {
            if (Manager && Manager.CustomMessagingManager != null)
                Manager.CustomMessagingManager.UnregisterNamedMessageHandler(nameof(ServerToClientConnectResult));
        }

        private void UnregisterServerMessageHandlers()
        {
            if (Manager && Manager.CustomMessagingManager != null)
                Manager.CustomMessagingManager.UnregisterNamedMessageHandler(nameof(ClientToServerSceneChanged));
        }


        /// <summary>
        /// This method runs when NetworkManager has started up (following a successful connect on the client, or directly after StartHost is invoked
        /// on the host). It serves the same role as OnNetworkSpawn, even though NetworkPortal itself isn't a NetworkBehaviour.
        /// </summary>
        private void OnNetworkReady()
        {
            Debug.Log($"{nameof(NetworkPortal)} : Ready");
            
            if (Manager.IsHost)
            {
                //special host code. This is what kicks off the flow that happens on a regular client
                //when it has finished connecting successfully. A dedicated server would remove this.
                Connected?.Invoke(ConnectStatus.Success);
            }

            NetworkReady?.Invoke();
        }

        /// <summary>
        /// Initializes host mode on this client. Call this and then other clients should connect to us!
        /// </summary>
        /// <remarks>
        /// See notes in <see cref="Net.Client.NetworkPortalClient"/> about why this must be static.
        /// </remarks>
        /// <param name="ipaddress">The IP address to connect to (currently IPV4 only).</param>
        /// <param name="port">The port to connect to. </param>
        public void StartHost(string ipaddress, ushort port)
        {
            if (Manager.NetworkConfig.NetworkTransport is UnityTransport utp)
            {
                utp.ConnectionData.Address = ipaddress;
                utp.ConnectionData.Port = port;
                utp.ConnectionData.ServerListenAddress = ipaddress;
            }
            else
            {
                Debug.LogError($"{nameof(NetworkPortal)} : Unsupported transport");
                return;
            }

            Manager.StartHost();
            RegisterClientMessageHandlers();
            RegisterServerMessageHandlers();

        }

        public void StartRelayHost(string roomName)
        {
            Manager.StartHost();
            RegisterClientMessageHandlers();
            RegisterServerMessageHandlers();
        }

        /// <summary>
        /// Responsible for the Server->Client RPC's of the connection result.
        /// </summary>
        /// <param name="netId"> id of the client to send to </param>
        /// <param name="status"> the status to pass to the client</param>
        public void ServerToClientConnectResult(ulong netId, ConnectStatus status)
        {
            using var writer = new FastBufferWriter(8, Allocator.Temp);
            writer.WriteValueSafe((int)status);
            Manager.CustomMessagingManager.SendNamedMessage(
                nameof(ServerToClientConnectResult),
                netId, writer, NetworkDelivery.Reliable
            );
        }

        /// <summary>
        /// Sends a DisconnectReason to the indicated client. This should only be done on the server, prior to disconnecting the client.
        /// </summary>
        /// <param name="netId"> id of the client to send to </param>
        /// <param name="status"> The reason for the upcoming disconnect.</param>
        public void ServerToClientSetDisconnectReason(ulong netId, ConnectStatus status)
        {
            using var writer = new FastBufferWriter(8, Allocator.Temp);
            writer.WriteValueSafe((int)status);
            Manager.CustomMessagingManager.SendNamedMessage(
                nameof(ServerToClientSetDisconnectReason),
                netId, writer, NetworkDelivery.Reliable
            );
        }

        public void ClientToServerSceneChanged(int newScene)
        {
            if (Manager.IsHost)
            {
                ClientSceneChanged?.Invoke(Manager.ServerClientId, newScene);
            }
            else if (Manager.IsConnectedClient)
            {
                using var writer = new FastBufferWriter(8, Allocator.Temp);
                writer.WriteValueSafe(newScene);
                Manager.CustomMessagingManager.SendNamedMessage(
                    nameof(ClientToServerSceneChanged),
                    Manager.ServerClientId, writer, NetworkDelivery.Reliable
                );
            }
        }

        /// <summary>
        /// This will disconnect (on the client) or shutdown the server (on the host). 
        /// </summary>
        public void RequestDisconnect()
        {
            Debug.Log($"{nameof(NetworkPortal)} : Disconnect requested");
            UserDisconnectRequested?.Invoke();
        }
    }
}