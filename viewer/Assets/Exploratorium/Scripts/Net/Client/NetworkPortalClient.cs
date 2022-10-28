/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System;
using Exploratorium.Net.Shared;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Exploratorium.Net.Client
{
    /// <summary>
    /// Client side logic for a <see cref="NetworkPortal"/>. Contains implementations for all of <see cref="NetworkPortal"/>'s server-to-client RPCs.
    /// </summary>
    [RequireComponent(typeof(NetworkPortal))]
    public class NetworkPortalClient : MonoBehaviour
    {
        private NetworkPortal _portal;

        /// <summary>
        /// If a disconnect occurred this will be populated with any contextual information that was available to explain why.
        /// </summary>
        public DisconnectReason DisconnectReason { get; private set; } = new DisconnectReason();

        /// <summary>
        /// Time in seconds before the client considers a lack of server response a timeout
        /// </summary>
        private const int TimeoutDuration = 5;

        public event Action<ConnectStatus> OnClientConnectFinished;

        [BoxGroup(Constants.InvokedEvents)]
        [SerializeField] private VoidEvent clientConnectedEvent;

        [BoxGroup(Constants.InvokedEvents)]
        [SerializeField] private VoidEvent clientDisconnectedEvent;

        /// <summary>
        /// This event fires when the client sent out a request to start the client, but failed to hear back after an allotted amount of
        /// time from the host.
        /// </summary>
        public event Action NetworkTimedOut;

        void Start()
        {
            _portal = GetComponent<NetworkPortal>();
            _portal.NetworkReady += OnNetworkReady;
            _portal.Connected += OnConnected;
            _portal.DisconnectReasonReceived += OnDisconnectReasonReceived;
            _portal.UserDisconnectRequested += OnUserDisconnectRequest;
            _portal.Manager.OnClientDisconnectCallback += OnDisconnectOrTimeout;
        }

        void OnDestroy()
        {
            if (_portal != null)
            {
                _portal.NetworkReady -= OnNetworkReady;
                _portal.Connected -= OnConnected;
                _portal.DisconnectReasonReceived -= OnDisconnectReasonReceived;

                if (_portal.Manager != null)
                {
                    _portal.Manager.OnClientDisconnectCallback -= OnDisconnectOrTimeout;
                }
            }
        }

        private void OnNetworkReady()
        {
            if (!_portal.Manager.IsClient)
            {
                enabled = false;
                Debug.Log($"{nameof(NetworkPortalClient)} : Disabled (not a client)");
                return;
            }

            Debug.Log($"{nameof(NetworkPortalClient)} : Ready");

            // if adding any event registrations in this block, please add unregistrations in the OnDisconnectOrTimeout method.
            if (!_portal.Manager.IsHost)
            {
                //only do this if a pure client, so as not to overlap with host behavior in NetworkPortalClient.
                _portal.UserDisconnectRequested += OnUserDisconnectRequest;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _portal.ClientToServerSceneChanged(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Invoked when the user has requested a disconnect via the UI
        /// </summary>
        private void OnUserDisconnectRequest()
        {
            if (_portal.Manager.IsClient)
            {
                Debug.Log($"{nameof(NetworkPortalClient)} : Stopping client in response to disconnect request");
                DisconnectReason.SetDisconnectReason(ConnectStatus.UserRequestedDisconnect);
                _portal.Manager.Shutdown();
            }
        }

        private void OnConnected(ConnectStatus status)
        {
            Debug.Log($"{nameof(NetworkPortalClient)} : Client connected; Got status: {status}");

            if (status != ConnectStatus.Success)
            {
                //this indicates a game level failure, rather than a network failure. See note in ServerGameNetPortal.
                DisconnectReason.SetDisconnectReason(status);
            }

            OnClientConnectFinished?.Invoke(status);
            if (clientConnectedEvent != null)
                clientConnectedEvent.Raise();
        }

        private void OnDisconnectReasonReceived(ConnectStatus status)
        {
            DisconnectReason.SetDisconnectReason(status);
            if (clientDisconnectedEvent != null)
                clientDisconnectedEvent.Raise();
        }

        private void OnDisconnectOrTimeout(ulong clientID)
        {
            Debug.Log($"{nameof(NetworkPortalClient)} : OnDisconnectOrTimeout");
            
            // we could also check whether the disconnect was us or the host, but the "interesting" question is whether
            // following the disconnect, we're no longer a Connected Client, so we just explicitly check that scenario.
            if (_portal.Manager.IsConnectedClient && !_portal.Manager.IsHost)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                _portal.UserDisconnectRequested -= OnUserDisconnectRequest;

                // On a client disconnect we want to take them back to the offline scene.
                // We have to check here in SceneManager if our active scene is the offline scene, as if it is,
                // it means we timed out rather than a raw disconnect;
                if (SceneManager.GetActiveScene().name != _portal.OfflineScene)
                {
                    Debug.Log($"{nameof(NetworkPortalClient)} : Disconnect");
                    // we're not at the main menu, so we obviously had a connection before... thus, we aren't in a timeout scenario.
                    // Just shut down networking and switch back to main menu.
                    _portal.Manager.Shutdown();
                    if (!DisconnectReason.HasTransitionReason)
                    {
                        //disconnect that happened for some other reason than user UI interaction--should display a message.
                        DisconnectReason.SetDisconnectReason(ConnectStatus.GenericDisconnect);
                    }

                    if (!SceneManager.GetSceneByName(_portal.OfflineScene).isLoaded)
                        SceneManager.LoadScene(_portal.OfflineScene, LoadSceneMode.Single);
                    if (clientDisconnectedEvent != null)
                        clientDisconnectedEvent.Raise();
                }
                else
                {
                    Debug.Log($"{nameof(NetworkPortalClient)} : Timeout");
                    NetworkTimedOut?.Invoke();
                    if (clientDisconnectedEvent != null)
                        clientDisconnectedEvent.Raise();
                }
            }
            else
            {
                Debug.LogError($"{nameof(NetworkPortalClient)} : Undefined state, forcing offline");
                _portal.Manager.Shutdown();
                SceneManager.LoadScene(_portal.OfflineScene, LoadSceneMode.Single);
            }
        }

        /// <summary>
        /// Wraps the invocation of <see cref="NetworkManager.StartClient"/>, including our GUID as the payload.
        /// </summary>
        /// <remarks>
        /// This method must be static because, when it is invoked, the client still doesn't know it's a client yet, and in particular, NetworkPortal hasn't
        /// yet initialized its client and server logic objects yet (which it does in NetworkStart, based on the role that the current player is performing).
        /// </remarks>
        /// <param name="portal"> </param>
        /// <param name="ipaddress">the IP address of the host to connect to. (currently IPV4 only)</param>
        /// <param name="port">The port of the host to connect to. </param>
        public static void StartClient(NetworkPortal portal, string ipaddress, ushort port)
        {
            
            if (portal.Manager.NetworkConfig.NetworkTransport is UnityTransport utp)
            {
                utp.ConnectionData.Address = ipaddress;
                utp.ConnectionData.Port = port;
            }
            else
            {
                Debug.LogError($"{nameof(NetworkPortalClient)} : Unsupported transport");
                return;
            }

            ConnectClient(portal);
        }

        private static void ConnectClient(NetworkPortal portal)
        {
            var clientGuid = NetworkClientPrefs.GetGuid();
            var payload = JsonUtility.ToJson(
                new ConnectionPayload
                {
                    clientGUID = clientGuid,
                    clientScene = SceneManager.GetActiveScene().buildIndex,
                    pawnName = portal.PlayerName
                }
            );

            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

            portal.Manager.NetworkConfig.ConnectionData = payloadBytes;
            portal.Manager.NetworkConfig.ClientConnectionBufferTimeout = TimeoutDuration;
            portal.Manager.StartClient();
            portal.RegisterClientMessageHandlers();
        }
    }
}