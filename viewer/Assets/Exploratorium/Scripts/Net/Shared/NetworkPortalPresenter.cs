/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System.Linq;
using Directus.Connect.v9.Unity.Runtime;
using Exploratorium.Net.Client;
using Exploratorium.Net.Server;
using Sirenix.OdinInspector;
using TMPro;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Exploratorium.Net.Shared
{
    /*
     *
    "com.unity.netcode.gameobjects": "1.0.0",
    "com.unity.multiplayer.tools": "1.0.0",
    "com.unity.transport": "1.0.0",
     */
    public class NetworkPortalPresenter : MonoBehaviour
    {
        [SerializeField] private NetworkPortal portal;
        [SerializeField] private NetworkPortalServer portalServer;
        [SerializeField] private NetworkPortalClient portalClient;
        [SerializeField] private CanvasGroup whileReady;
        [SerializeField] private GameObject whileOnline;
        [SerializeField] private GameObject whileOffline;
        
        [SerializeField] private TMP_Text status;

        [BoxGroup(Constants.ReadVariables)] [SerializeField]
        private BoolVariable[] isReadyToStart;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private AtomEventBase readStartServerEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private AtomEventBase readStartClientEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private AtomEventBase readStartHostEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private AtomEventBase readStopEvent;

        [BoxGroup("Host Configuration")] [SerializeField]
        private IntReference readHostPort;

        [BoxGroup("Host Configuration")] [SerializeField]
        private StringReference readHostAddress;

        [BoxGroup("Client Configuration")] [SerializeField]
        private StringReference clientName;

        private ConnectStatus _clientStatus;

        [ShowInInspector]
        [Sirenix.OdinInspector.ReadOnly]
        public bool IsReadyToStart => isReadyToStart.All(it => it == null || it.Value);

        public void StartHost()
        {
            if (!IsReadyToStart)
            {
                Debug.LogError($"{nameof(NetworkPortalPresenter)} : Not ready to start networking", this);
                return;
            }

            SetPlayerName();
            Debug.Log($"{nameof(NetworkPortalPresenter)} : Starting host", this);
            portal.StartHost(readHostAddress.Value, (ushort)readHostPort.Value);
        }

        public void StartClient()
        {
            if (!IsReadyToStart)
            {
                Debug.LogError($"{nameof(NetworkPortalPresenter)} : Not ready to start networking", this);
                return;
            }

            SetPlayerName();
            Debug.Log($"{nameof(NetworkPortalPresenter)} : Connecting to {readHostAddress.Value}:{(ushort)readHostPort.Value}", this);
            NetworkPortalClient.StartClient(portal, readHostAddress.Value, (ushort)readHostPort.Value);

        }

        public void StartServer()
        {
            if (!IsReadyToStart)
            {
                Debug.LogError($"{nameof(NetworkPortalPresenter)} : Not ready to start networking", this);
                return;
            }

            Debug.Log($"{nameof(NetworkPortalPresenter)} : Starting server", this);
            SetPlayerName();
            NetworkPortalServer.StartServer(portal, readHostAddress.Value, (ushort)readHostPort.Value);
        }

        private void SetPlayerName()
        {
            if (clientName.IsUnassigned)
                portal.PlayerName = null;
            else
                portal.PlayerName = clientName.Value;
        }

        public void Disconnect() => portal.RequestDisconnect();

        private void Awake()
        {
            Debug.Assert(portal != null, $"{nameof(portal)} property not assigned");
            Debug.Assert(readStartServerEvent != null, $"{nameof(readStartServerEvent)} property not assigned");
            Debug.Assert(readStartClientEvent != null, $"{nameof(readStartClientEvent)} property not assigned");
            Debug.Assert(readStartHostEvent != null, $"{nameof(readStartHostEvent)} property not assigned");
            Debug.Assert(readStopEvent != null, $"{nameof(readStopEvent)} property not assigned");
        }

        private void OnEnable()
        {
            if (portal && portal.Manager)
            {
                portal.Manager.OnClientConnectedCallback += OnClientConnected;
                portal.Manager.OnClientDisconnectCallback += OnClientDisconnected;
                portal.Manager.OnServerStarted += OnServerStarted;
                portalClient.OnClientConnectFinished += OnClientConnectFinished;
                portal.DisconnectReasonReceived += OnDisconnectReason;
                portalClient.NetworkTimedOut += OnClientTimeout;
            }

            if (readStartServerEvent)
                readStartServerEvent.Register(StartServer);
            if (readStartClientEvent)
                readStartClientEvent.Register(StartClient);
            if (readStartHostEvent)
                readStartHostEvent.Register(StartHost);
            if (readStopEvent)
                readStopEvent.Register(Disconnect);

            UpdateStatus();
        }
        
        private void OnDisable()
        {
            if (portal && portal.Manager)
            {
                portal.Manager.OnClientConnectedCallback -= OnClientConnected;
                portal.Manager.OnClientDisconnectCallback -= OnClientDisconnected;
                portal.Manager.OnServerStarted -= OnServerStarted;
                portalClient.OnClientConnectFinished -= OnClientConnectFinished;
                portal.DisconnectReasonReceived -= OnDisconnectReason;
            }

            if (readStartServerEvent)
                readStartServerEvent.Unregister(StartServer);
            if (readStartClientEvent)
                readStartClientEvent.Unregister(StartClient);
            if (readStartHostEvent)
                readStartHostEvent.Unregister(StartHost);
            if (readStopEvent)
                readStopEvent.Unregister(Disconnect);

            UpdateStatus();
        }

        private void OnDisconnectReason(ConnectStatus value)
        {
            Debug.LogWarning($"{nameof(NetworkPortalPresenter)} : Disconnect detected; Reason was {value}");
            _clientStatus = value;
            UpdateStatus();
        }
        
        private void OnClientTimeout()
        {
            Debug.LogWarning($"{nameof(NetworkPortalPresenter)} : Timeout detected");
            _clientStatus = ConnectStatus.Timeout;
            UpdateStatus();
        }

        private void OnClientConnectFinished(ConnectStatus value)
        {
            _clientStatus = value;
            UpdateStatus();
        }

        private void OnServerStarted()
        {
            _clientStatus = ConnectStatus.Undefined;
            UpdateStatus();
        }

        private void OnClientDisconnected(ulong value)
        {
            UpdateStatus();
        }

        private void OnClientConnected(ulong value)
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            Debug.Assert(status != null, "status != null");

            if (status == null)
                return;

            // in case the portal is destroyed
            if (portal == null || portal.Manager == null)
                return;

            if (portal.Manager.IsHost)
            {
                status.text = $"Host, {portal.Manager.ConnectedClients.Count} clients including self";
            }
            else if (portal.Manager.IsClient && !portal.Manager.IsServer)
            {
                status.text = $"Client, connect {_clientStatus} to {portal.Manager.ConnectedHostname}";
            }
            else if (!portal.Manager.IsClient && portal.Manager.IsServer)
            {
                status.text = $"Server, {portal.Manager.ConnectedClients.Count} clients";
            }
            else if (!portal.Manager.IsClient && !portal.Manager.IsServer)
            {
                status.text = _clientStatus != default
                    ? $"Offline ({_clientStatus})"
                    : "Offline";
            }
        }

        private void Update()
        {
            if (whileReady != null)
                whileReady.interactable = IsReadyToStart;
            
            if (portal.Manager.IsServer || portal.Manager.IsClient)
            {
                whileOnline.SetActive(true);
                whileOffline.SetActive(false);
            }
            else
            {
                whileOnline.SetActive(false);
                whileOffline.SetActive(true);
            }
        }
    }
}