/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Exploratorium.Frontend;
using Exploratorium.Net.Shared.Actions;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Netcode;
using UnityAtoms.BaseAtoms;
using UnityAtoms.Extensions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;

namespace Exploratorium.Net.Shared
{
    [Serializable]
    public struct PawnDescription
    {
        public bool isNpc;
        public PawnRole role;
        public FixedString32Bytes displayedName;
    }

    [Serializable]
    public enum PawnState
    {
        Active,
        Disabled,
    }

    [Serializable]
    public enum PawnRole
    {
        None,
        Observer,
        Solo,
        Controller
    }

    /// <summary>
    /// Contains all NetworkVariables and RPCs of a pawn. This component is present on both client and server objects.
    /// </summary>
    public class NetworkPawn : NetworkBehaviour
    {
        [SerializeField] private GameObjectEvent spawnedEvent;
        [SerializeField] private GameObjectEvent deSpawningEvent;

        [Header(Constants.ReadVariables)] [SerializeField]
        private IntVariable autostartRole;

        [Header(Constants.WriteVariables)] [SerializeField]
        private PawnRoleVariable roleVariable;

        [Header("Read/Write Variables")] [SerializeField]
        private FloatVariable audioVolume;

        [Header(Constants.ObservedEvents)] [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private VoidEvent reqSoloEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private VoidEvent reqControlEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private VoidEvent reqVObsEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private VoidEvent reqHObsEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private FlowStateEvent flowReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private FloatEvent videoTimeReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private BoolEvent videoPlayReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private FloatEvent slideshowTimeReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private Vector3Event slideshowPanReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private Vector3Event slideshowScaleReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private BoolEvent slideshowFullscreenReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private BoolEvent slideshowInfoReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private IntEvent modelIndexReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private Vector3Event modelPosReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private QuaternionEvent modelRotReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private BoolEvent modelFullscreenReportEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private BoolEvent modelInfoReportEvent;

        [Header(Constants.InvokedEvents)] [SerializeField]
        private FlowStateEvent flowSyncEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private VoidEvent controllerHeartbeat;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private FloatEvent videoTimeSyncEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private BoolEvent videoPlaySyncEvent;


        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private Vector3Event slideshowPanSyncEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private Vector3Event slideshowScaleSyncEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private FloatEvent slideshowTimeSyncEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private BoolEvent slideshowFullscreenSyncEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private BoolEvent slideshowInfoSyncEvent;


        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private BoolEvent modelFullscreenSyncEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private BoolEvent modelInfoSyncEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private Vector3Event modelPosSyncEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private QuaternionEvent modelRotSyncEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private IntEvent modelIndexSyncEvent;

        [SerializeField] private bool debug;

        public NetworkVariable<PawnDescription> n_description { get; } = new NetworkVariable<PawnDescription>();
        public NetworkVariable<PawnRole> n_role { get; } = new NetworkVariable<PawnRole>();
        public NetworkVariable<FixedString32Bytes> n_name { get; } = new NetworkVariable<FixedString32Bytes>();
        public NetworkVariable<Flow.FlowState> n_flow { get; } = new NetworkVariable<Flow.FlowState>();
        public NetworkVariable<float> n_slideshowTime { get; } = new NetworkVariable<float>();
        public NetworkVariable<float> n_videoTime { get; } = new NetworkVariable<float>();
        public NetworkVariable<int> n_ModelIndex { get; } = new NetworkVariable<int>();
        public NetworkVariable<bool> n_videoPlay { get; } = new NetworkVariable<bool>();
        public NetworkVariable<float> n_audioVolume { get; } = new NetworkVariable<float>();
        public NetworkVariable<FixedString32Bytes> n_locale { get; } = new NetworkVariable<FixedString32Bytes>();

        private Action _unsubscribeOnDespawn;
        private OnlineState onlineState;
        private double _nextHeartbeat;

        public PawnDescription Description => n_description.Value;

        /*public event Action<FixedString32Bytes> NameChanged;
        public event Action<PawnRole> RoleChanged;*/
        public static event Action<NetworkPawn> Spawned;
        public static event Action<NetworkPawn> DeSpawning;

        private void Awake()
        {
            Debug.Assert(!debug, "!debug");
            Debug.Assert(spawnedEvent != null, "spawnedEvent != null", this);
            Debug.Assert(deSpawningEvent != null, "deSpawningEvent != null", this);
            Debug.Assert(roleVariable != null, "roleVariable != null", this);
            Debug.Assert(audioVolume != null, "audioVolume != null", this);
            Debug.Assert(reqSoloEvent != null, "reqSoloEvent != null", this);
            Debug.Assert(reqControlEvent != null, "reqControlEvent != null", this);
            Debug.Assert(reqVObsEvent != null, "reqVObsEvent != null", this);
            Debug.Assert(reqHObsEvent != null, "reqHObsEvent != null", this);
            Debug.Assert(flowReportEvent != null, "flowReportEvent != null", this);
            Debug.Assert(videoTimeReportEvent != null, "videoTimeReportEvent != null", this);
            Debug.Assert(videoPlayReportEvent != null, "videoPlayReportEvent != null", this);
            Debug.Assert(slideshowTimeReportEvent != null, "slideshowTimeReportEvent != null", this);
            Debug.Assert(slideshowPanReportEvent != null, "slideshowPanReportEvent != null", this);
            Debug.Assert(slideshowScaleReportEvent != null, "slideshowScaleReportEvent != null", this);
            Debug.Assert(slideshowFullscreenReportEvent != null, "slideshowFullscreenReportEvent != null", this);
            Debug.Assert(slideshowInfoReportEvent != null, "slideshowInfoReportEvent != null", this);
            Debug.Assert(modelIndexReportEvent != null, "modelIndexReportEvent != null", this);
            Debug.Assert(modelPosReportEvent != null, "modelPosReportEvent != null", this);
            Debug.Assert(modelRotReportEvent != null, "modelRotReportEvent != null", this);
            Debug.Assert(modelFullscreenReportEvent != null, "modelFullscreenReportEvent != null", this);
            Debug.Assert(modelInfoReportEvent != null, "modelInfoReportEvent != null", this);
            Debug.Assert(flowSyncEvent != null, "flowSyncEvent != null", this);
            Debug.Assert(controllerHeartbeat != null, "controllerHeartbeat != null", this);
            Debug.Assert(videoTimeSyncEvent != null, "videoTimeSyncEvent != null", this);
            Debug.Assert(videoPlaySyncEvent != null, "videoPlaySyncEvent != null", this);
            Debug.Assert(slideshowPanSyncEvent != null, "slideshowPanSyncEvent != null", this);
            Debug.Assert(slideshowScaleSyncEvent != null, "slideshowScaleSyncEvent != null", this);
            Debug.Assert(slideshowTimeSyncEvent != null, "slideshowTimeSyncEvent != null", this);
            Debug.Assert(slideshowFullscreenSyncEvent != null, "slideshowFullscreenSyncEvent != null", this);
            Debug.Assert(slideshowInfoSyncEvent != null, "slideshowInfoSyncEvent != null", this);
            Debug.Assert(modelFullscreenSyncEvent != null, "modelFullscreenSyncEvent != null", this);
            Debug.Assert(modelInfoSyncEvent != null, "modelInfoSyncEvent != null", this);
            Debug.Assert(modelPosSyncEvent != null, "modelPosSyncEvent != null", this);
            Debug.Assert(modelRotSyncEvent != null, "modelRotSyncEvent != null", this);
            Debug.Assert(modelIndexSyncEvent != null, "modelIndexSyncEvent != null", this);
        }

        private void Update()
        {
            if (IsServer)
            {
                if (n_role.Value == PawnRole.Controller && _nextHeartbeat < NetworkManager.ServerTime.TimeAsFloat)
                {
                    _nextHeartbeat = NetworkManager.ServerTime.TimeAsFloat + 5f;
                    SendHeartbeatToObservers();
                }
            }
        }

        private void SendHeartbeatToObservers() => InvokeOnObservers(pawn =>
        {
            if (debug)
                Debug.Log(
                    $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(SendHeartbeatToObservers)}()",
                    this);
            pawn.ControllerHeartbeat_SyncClientRpc();
        });

        [ClientRpc]
        private void ControllerHeartbeat_SyncClientRpc()
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer)
            {
                if (debug)
                    Debug.Log(
                        $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(ControllerHeartbeat_SyncClientRpc)}()",
                        this);
                if (controllerHeartbeat != null)
                    controllerHeartbeat.Raise();
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : Ready");

            name = IsLocalPlayer
                ? $"{nameof(NetworkPawn)}#{NetworkObjectId} (LOCAL PLAYER)"
                : $"{nameof(NetworkPawn)}#{NetworkObjectId}";

            if (onlineState == null)
                onlineState = FindObjectOfType<OnlineState>();
            Debug.Assert(onlineState != null,
                $"{nameof(NetworkPawn)}/{NetworkObjectId} : Could not find instance of {nameof(onlineState)}, is one added to the scene?");


            //n_description.OnValueChanged += (value, newValue) => DescriptionChanged?.Invoke(newValue);
            //n_role.OnValueChanged += (value, newValue) => RoleChanged?.Invoke(newValue);
            //n_name.OnValueChanged += (value, newValue) => NameChanged?.Invoke(newValue);
            Spawned?.Invoke(this);
            if (spawnedEvent)
                spawnedEvent.Raise(gameObject);

            if (IsLocalPlayer)
            {
                void OnControlEvent() => RequestRoleServerRpc(NetworkManager.LocalClientId, PawnRole.Controller);
                void OnVObsEvent() => RequestRoleServerRpc(NetworkManager.LocalClientId, PawnRole.Observer);
                void OnHObsEvent() => RequestRoleServerRpc(NetworkManager.LocalClientId, PawnRole.Observer);
                void OnSoloEvent() => RequestRoleServerRpc(NetworkManager.LocalClientId, PawnRole.Solo);

                void Subscribe()
                {
                    reqSoloEvent.OnEventNoValue += OnSoloEvent;
                    reqControlEvent.OnEventNoValue += OnControlEvent;
                    reqVObsEvent.OnEventNoValue += OnVObsEvent;
                    reqHObsEvent.OnEventNoValue += OnHObsEvent;

                    n_flow.OnValueChanged += OnFlowSync;
                    n_role.OnValueChanged += OnRoleSync;
                    n_slideshowTime.OnValueChanged += OnSlideshowTimeSync;
                    n_videoTime.OnValueChanged += OnVideoTimeSync;
                    n_videoPlay.OnValueChanged += OnVideoPlaySync;
                    n_ModelIndex.OnValueChanged += OnModelIndexSync;
                    n_locale.OnValueChanged += OnLocaleSync;
                    n_audioVolume.OnValueChanged += OnAudioVolumeSync;

                    if (audioVolume.Changed)
                        audioVolume.Changed.Register(OnAudioVolumeReport);

                    if (flowReportEvent != null)
                        flowReportEvent.Register(OnFlowReport);

                    if (videoTimeReportEvent != null)
                        videoTimeReportEvent.Register(OnVideoTimeReport);
                    if (videoPlayReportEvent != null)
                        videoPlayReportEvent.Register(OnVideoPlayReport);

                    if (modelIndexReportEvent != null)
                        modelIndexReportEvent.Register(OnModelIndexReport);
                    if (modelPosReportEvent != null)
                        modelPosReportEvent.Register(OnModelPosReport);
                    if (modelRotReportEvent != null)
                        modelRotReportEvent.Register(OnModelRotReport);
                    if (modelFullscreenReportEvent != null)
                        modelFullscreenReportEvent.Register(OnModelFullscreenReport);
                    if (modelInfoReportEvent != null)
                        modelInfoReportEvent.Register(OnModelInfoReport);

                    if (slideshowTimeReportEvent != null)
                        slideshowTimeReportEvent.Register(OnSlideshowTimeReport);
                    if (slideshowPanReportEvent != null)
                        slideshowPanReportEvent.Register(OnSlideshowPanReport);
                    if (slideshowScaleReportEvent != null)
                        slideshowScaleReportEvent.Register(OnSlideshowScaleReport);
                    if (slideshowFullscreenReportEvent != null)
                        slideshowFullscreenReportEvent.Register(OnSlideshowFullscreenReport);
                    if (slideshowInfoReportEvent != null)
                        slideshowInfoReportEvent.Register(OnSlideshowInfoReport);

                    LocalizationSettings.SelectedLocaleChanged += OnLocaleChangedReport;
                }

                void Unsubscribe()
                {
                    reqSoloEvent.OnEventNoValue -= OnSoloEvent;
                    reqControlEvent.OnEventNoValue -= OnControlEvent;
                    reqVObsEvent.OnEventNoValue -= OnVObsEvent;
                    reqHObsEvent.OnEventNoValue -= OnHObsEvent;

                    n_flow.OnValueChanged -= OnFlowSync;
                    n_role.OnValueChanged -= OnRoleSync;
                    n_slideshowTime.OnValueChanged -= OnSlideshowTimeSync;
                    n_videoTime.OnValueChanged -= OnVideoTimeSync;
                    n_videoPlay.OnValueChanged -= OnVideoPlaySync;
                    n_ModelIndex.OnValueChanged -= OnModelIndexSync;
                    n_locale.OnValueChanged -= OnLocaleSync;
                    n_audioVolume.OnValueChanged -= OnAudioVolumeSync;

                    if (audioVolume.Changed)
                        audioVolume.Changed.Unregister(OnAudioVolumeReport);

                    if (flowReportEvent != null)
                        flowReportEvent.Unregister(OnFlowReport);

                    if (videoTimeReportEvent != null)
                        videoTimeReportEvent.Unregister(OnVideoTimeReport);
                    if (videoPlayReportEvent != null)
                        videoPlayReportEvent.Unregister(OnVideoPlayReport);

                    if (modelIndexReportEvent != null)
                        modelIndexReportEvent.Unregister(OnModelIndexReport);
                    if (modelPosReportEvent != null)
                        modelPosReportEvent.Unregister(OnModelPosReport);
                    if (modelRotReportEvent != null)
                        modelRotReportEvent.Unregister(OnModelRotReport);
                    if (modelFullscreenReportEvent != null)
                        modelFullscreenReportEvent.Unregister(OnModelFullscreenReport);
                    if (modelInfoReportEvent != null)
                        modelInfoReportEvent.Unregister(OnModelInfoReport);

                    if (slideshowTimeReportEvent != null)
                        slideshowTimeReportEvent.Unregister(OnSlideshowTimeReport);
                    if (slideshowPanReportEvent != null)
                        slideshowPanReportEvent.Unregister(OnSlideshowPanReport);
                    if (slideshowScaleReportEvent != null)
                        slideshowScaleReportEvent.Unregister(OnSlideshowScaleReport);
                    if (slideshowFullscreenReportEvent != null)
                        slideshowFullscreenReportEvent.Unregister(OnSlideshowFullscreenReport);
                    if (slideshowInfoReportEvent != null)
                        slideshowInfoReportEvent.Unregister(OnSlideshowInfoReport);

                    LocalizationSettings.SelectedLocaleChanged -= OnLocaleChangedReport;
                }

                Subscribe();
                _unsubscribeOnDespawn = Unsubscribe;

                if (IsClient)
                {
                    switch (autostartRole.Value)
                    {
                        case (int)PawnRole.Observer:
                            Debug.LogWarning(
                                $"{nameof(NetworkPawn)}/{NetworkObjectId}/Client : Requesting autostart role {PawnRole.Observer}",
                                this);
                            RequestRoleServerRpc(NetworkObject.OwnerClientId, PawnRole.Observer);
                            break;
                        case (int)PawnRole.Controller:
                            Debug.LogWarning(
                                $"{nameof(NetworkPawn)}/{NetworkObjectId}/Client : Requesting autostart role {PawnRole.Controller}",
                                this);
                            RequestRoleServerRpc(NetworkObject.OwnerClientId, PawnRole.Controller);
                            break;
                        case (int)PawnRole.None:
                        case (int)PawnRole.Solo:
                            Debug.LogWarning(
                                $"{nameof(NetworkPawn)}/{NetworkObjectId}/Client : Requesting autostart role {PawnRole.Solo}",
                                this);
                            RequestRoleServerRpc(NetworkObject.OwnerClientId, PawnRole.Solo);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                Debug.Assert(flowReportEvent != null, "No flow event assigned, flow state will bot be synchronized",
                    this);
            }
        }

        // ACTION SYSTEM

        /// <summary>
        /// This event is raised on the server when an action request arrives
        /// </summary>
        public event Action<ActionRequestData> DoActionEventServer;

        /// <summary>
        /// This event is raised on the client when an action is being played back.
        /// </summary>
        public event Action<ActionRequestData> DoActionEventClient;

        /// <summary>
        /// This event is raised on the client when the active action FXs need to be cancelled (e.g. when the character has been stunned)
        /// </summary>
        public event Action CancelAllActionsEventClient;

        /// <summary>
        /// This event is raised on the client when active action FXs of a certain type need to be cancelled (e.g. when the Stealth action ends)
        /// </summary>
        public event Action<ActionType> CancelActionsByTypeEventClient;

        /// <summary>
        /// Raised on the server when a role is requested
        /// </summary>
        public event Action<ulong, PawnRole> RoleRequestedServer;

        /// <summary>
        /// Server to Client RPC that broadcasts this action play to all clients.
        /// </summary>
        /// <param name="data"> Data about which action to play and its associated details. </param>
        [ClientRpc]
        public void DoActionClientRPC(ActionRequestData data) => DoActionEventClient?.Invoke(data);

        [ClientRpc]
        public void CancelAllActionsClientRpc() => CancelAllActionsEventClient?.Invoke();

        [ClientRpc]
        public void CancelActionsByTypeClientRpc(ActionType action) => CancelActionsByTypeEventClient?.Invoke(action);

        /// <summary>
        /// Client->Server RPC that sends a request to play an action.
        /// </summary>
        /// <param name="data">Data about which action to play and its associated details. </param>
        [ServerRpc]
        public void DoActionServerRPC(ActionRequestData data) => DoActionEventServer?.Invoke(data);


        // ROLE SYNC

        [ServerRpc]
        public void RequestRoleServerRpc(ulong clientId, PawnRole role)
        {
            Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId}/Server : Requested {role}");
            RoleRequestedServer?.Invoke(clientId, role);
        }

        private void OnRoleSync(PawnRole previousvalue, PawnRole newvalue)
        {
            if (roleVariable != null)
                roleVariable.Value = newvalue;

            flowSyncEvent.Raise(new Flow.FlowState
            {
                Trigger = Flow.Trigger.Reset,
                Destination = Flow.State.Idle,
                Source = Flow.State.Initial,
            });
        }

        // REPORT

        private void OnLocaleChangedReport(Locale locale)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                Locale_ReportServerRpc(OwnerClientId, locale.Identifier.Code);
        }

        private void OnModelIndexReport(int ndx)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                ModelIndex_ReportServerRpc(ndx);
        }

        private void OnVideoPlayReport(bool shouldPlay)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                VideoPlay_ReportServerRpc(shouldPlay);
        }

        private void OnVideoTimeReport(float value)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                VideoTime_ReportServerRpc(value);
        }

        private void OnSlideshowTimeReport(float time)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                SlideshowTime_ReportServerRpc(time);
        }

        public void OnFlowReport(Flow.FlowState state)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                FlowReport_ServerRpc(OwnerClientId, state);
        }

        public void OnAudioVolumeReport(float value)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                AudioVolumeReport_ServerRpc(OwnerClientId, value);
        }


        public void OnSlideshowScaleReport(Vector3 scale)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                SlideshowScale_ReportServerRpc(scale);
        }

        public void OnSlideshowPanReport(Vector3 pos)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                SlideshowPan_ReportServerRpc(pos);
        }

        public void OnModelPosReport(Vector3 pos)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                ModelPos_ReportServerRpc(pos);
        }

        public void OnModelRotReport(Quaternion rot)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                ModelRot_ReportServerRpc(rot);
        }

        public void OnSlideshowFullscreenReport(bool isOn)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                SlideshowFullscreen_ReportServerRpc(isOn);
        }

        public void OnModelFullscreenReport(bool isOn)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                ModelFullscreen_ReportServerRpc(isOn);
        }

        public void OnModelInfoReport(bool isOn)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                ModelInfo_ReportServerRpc(isOn);
        }

        public void OnSlideshowInfoReport(bool isOn)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Controller)
                SlideshowInfo_ReportServerRpc(isOn);
        }

        // RPC

        [ServerRpc]
        private void SlideshowScale_ReportServerRpc(Vector3 scale)
        {
            if (debug)
                Debug.Log(
                    $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(SlideshowScale_ReportServerRpc)}({scale:F2})",
                    this);
            InvokeOnObservers(p => p.SlideshowScale_ReportClientRpc(scale));
        }

        [ServerRpc]
        private void SlideshowPan_ReportServerRpc(Vector3 pos)
        {
            if (debug)
                Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(SlideshowPan_ReportServerRpc)}({pos:F2})",
                    this);
            InvokeOnObservers(p => p.SlideshowPan_SyncClientRpc(pos));
        }

        [ServerRpc]
        private void ModelPos_ReportServerRpc(Vector3 pos)
        {
            if (debug)
                Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(ModelPos_ReportServerRpc)}({pos:F2})",
                    this);
            InvokeOnObservers(p => p.ModelPos_SyncClientRpc(pos));
        }

        [ServerRpc]
        private void ModelRot_ReportServerRpc(Quaternion rot)
        {
            if (debug)
                Debug.Log(
                    $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(ModelRot_ReportServerRpc)}({rot.eulerAngles:F2})",
                    this);
            InvokeOnObservers(p => p.ModelRot_SyncClientRpc(rot));
        }

        [ServerRpc]
        private void ModelFullscreen_ReportServerRpc(bool isOn)
        {
            if (debug)
                Debug.Log(
                    $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(ModelFullscreen_ReportServerRpc)}({isOn})",
                    this);
            InvokeOnObservers(p => p.ModelFullscreen_SyncClientRpc(isOn));
        }

        [ServerRpc]
        private void SlideshowFullscreen_ReportServerRpc(bool isOn)
        {
            if (debug)
                Debug.Log(
                    $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(SlideshowFullscreen_ReportServerRpc)}({isOn})",
                    this);
            InvokeOnObservers(p => p.SlideshowFullscreen_SyncClientRpc(isOn));
        }

        [ServerRpc]
        private void ModelInfo_ReportServerRpc(bool isOn)
        {
            if (debug)
                Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(ModelInfo_ReportServerRpc)}({isOn})",
                    this);
            InvokeOnObservers(p => p.ModelInfo_SyncClientRpc(isOn));
        }

        [ServerRpc]
        private void SlideshowInfo_ReportServerRpc(bool isOn)
        {
            if (debug)
                Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(SlideshowInfo_ReportServerRpc)}({isOn})",
                    this);
            InvokeOnObservers(p => p.SlideshowInfo_SyncClientRpc(isOn));
        }

        [ServerRpc]
        private void Locale_ReportServerRpc(ulong clientId, FixedString32Bytes code)
        {
            if (debug)
                Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(Locale_ReportServerRpc)}({code})", this);
            InvokeOnObservers(p => p.n_locale.Value = code);
        }


        [ServerRpc]
        private void AudioVolumeReport_ServerRpc(ulong clientId, float value)
        {
            if (debug)
                Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(AudioVolumeReport_ServerRpc)}({value})",
                    this);
            InvokeOnObservers(p => p.n_audioVolume.Value = value);
        }

        [ServerRpc]
        private void FlowReport_ServerRpc(ulong clientId, Flow.FlowState state)
        {
            if (debug)
                Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(FlowReport_ServerRpc)}({state})",
                    this);
            InvokeOnObservers(p => p.n_flow.Value = state);
        }

        [ServerRpc]
        private void SlideshowTime_ReportServerRpc(float time)
        {
            if (debug)
                Debug.Log(
                    $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(SlideshowTime_ReportServerRpc)}({time:F2})",
                    this);
            InvokeOnObservers(p => p.n_slideshowTime.Value = time);
        }

        [ServerRpc]
        private void VideoPlay_ReportServerRpc(bool shouldPlay)
        {
            if (debug)
                Debug.Log(
                    $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(VideoPlay_ReportServerRpc)}({shouldPlay})",
                    this);
            InvokeOnObservers(p => p.n_videoPlay.Value = shouldPlay);
        }

        [ServerRpc]
        private void VideoTime_ReportServerRpc(float time)
        {
            if (debug)
                Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(VideoTime_ReportServerRpc)}({time:F2})",
                    this);
            InvokeOnObservers(p => p.n_videoTime.Value = time);
        }

        [ServerRpc]
        private void ModelIndex_ReportServerRpc(int ndx)
        {
            if (debug)
                Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(ModelIndex_ReportServerRpc)}({ndx})",
                    this);
            InvokeOnObservers(p => p.n_ModelIndex.Value = ndx);
        }


        // CLIENT RPC


        [ClientRpc]
        private void SlideshowPan_SyncClientRpc(Vector3 pos)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer && slideshowPanSyncEvent != null)
            {
                if (debug)
                    Debug.Log(
                        $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(SlideshowPan_SyncClientRpc)}({pos:F2})",
                        this);
                slideshowPanSyncEvent.Raise(pos);
            }
        }

        [ClientRpc]
        private void SlideshowScale_ReportClientRpc(Vector3 scale)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer && slideshowScaleSyncEvent != null)
            {
                if (debug)
                    Debug.Log(
                        $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(SlideshowScale_ReportClientRpc)}({scale:F2})",
                        this);
                slideshowScaleSyncEvent.Raise(scale);
            }
        }

        [ClientRpc]
        private void SlideshowFullscreen_SyncClientRpc(bool isOn)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer && slideshowFullscreenSyncEvent != null)
            {
                if (debug)
                    Debug.Log(
                        $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(SlideshowFullscreen_SyncClientRpc)}({isOn})",
                        this);
                slideshowFullscreenSyncEvent.Raise(isOn);
            }
        }

        [ClientRpc]
        private void SlideshowInfo_SyncClientRpc(bool isOn)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer && slideshowInfoSyncEvent != null)
            {
                if (debug)
                    Debug.Log(
                        $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(SlideshowInfo_SyncClientRpc)}({isOn})",
                        this);
                slideshowInfoSyncEvent.Raise(isOn);
            }
        }


        [ClientRpc]
        private void ModelPos_SyncClientRpc(Vector3 pos)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer && modelPosSyncEvent != null)
            {
                if (debug)
                    Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(ModelPos_SyncClientRpc)}({pos:F2})",
                        this);
                modelPosSyncEvent.Raise(pos);
            }
        }

        [ClientRpc]
        private void ModelRot_SyncClientRpc(Quaternion rot)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer && modelRotSyncEvent != null)
            {
                if (debug)
                    Debug.Log(
                        $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(ModelRot_SyncClientRpc)}({rot.eulerAngles:F2})",
                        this);
                modelRotSyncEvent.Raise(rot);
            }
        }


        [ClientRpc]
        private void ModelFullscreen_SyncClientRpc(bool isOn)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer && modelFullscreenSyncEvent != null)
            {
                if (debug)
                    Debug.Log(
                        $"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(ModelFullscreen_SyncClientRpc)}({isOn})",
                        this);
                modelFullscreenSyncEvent.Raise(isOn);
            }
        }

        [ClientRpc]
        private void ModelInfo_SyncClientRpc(bool isOn)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer && modelInfoSyncEvent != null)
            {
                if (debug)
                    Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(ModelInfo_SyncClientRpc)}({isOn})",
                        this);
                modelInfoSyncEvent.Raise(isOn);
            }
        }

        // SYNC VAR UPDATE

        private void OnLocaleSync(FixedString32Bytes previousValue, FixedString32Bytes newValue)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer)
            {
                var syncLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(it =>
                    it.Identifier.Code == newValue);
                if (syncLocale != default)
                {
                    Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(OnLocaleSync)}({newValue})", this);
                    LocalizationSettings.SelectedLocaleChanged -= OnLocaleChangedReport;
                    LocalizationSettings.SelectedLocale = syncLocale;
                    LocalizationSettings.SelectedLocaleChanged += OnLocaleChangedReport;
                }
                else
                {
                    Debug.LogWarning(
                        $"{nameof(NetworkPawn)}/{NetworkObjectId} : Locale sync failed; No locale found matching code {newValue}");
                }
            }
        }

        private void OnSlideshowTimeSync(float previousValue, float newValue)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer)
            {
                if (debug)
                    Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(OnSlideshowTimeSync)}({newValue:F2})",
                        this);
                slideshowTimeSyncEvent.Raise(newValue);
            }
        }

        private void OnModelIndexSync(int previousValue, int newValue)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer)
            {
                if (debug)
                    Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(OnModelIndexSync)}({newValue})",
                        this);
                modelIndexSyncEvent.Raise(newValue);
            }
        }

        private void OnVideoTimeSync(float previousValue, float newValue)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer)
            {
                if (debug)
                    Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(OnVideoTimeSync)}({newValue:F2})",
                        this);
                videoTimeSyncEvent.Raise(newValue);
            }
        }

        private void OnAudioVolumeSync(float previousValue, float newValue)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer)
            {
                if (debug)
                    Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(OnVideoPlaySync)}({newValue})", this);
                audioVolume.Value = newValue;
            }
        }

        private void OnVideoPlaySync(bool previousValue, bool newValue)
        {
            if (IsLocalPlayer && n_role.Value == PawnRole.Observer)
            {
                if (debug)
                    Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(OnVideoPlaySync)}({newValue})", this);
                videoPlaySyncEvent.Raise(newValue);
            }
        }

        private void OnFlowSync(Flow.FlowState _, Flow.FlowState next)
        {
            if (debug)
                Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : {nameof(OnFlowSync)}({next.Destination})", this);

            if (!IsLocalPlayer)
                return;

            // only the Observer role will be responding to FlowSync events
            switch (n_role.Value)
            {
                case PawnRole.Observer:
                    flowSyncEvent.Raise(next);
                    break;
                case PawnRole.Controller:
                case PawnRole.Solo:
                    // ignore
                    if (debug)
                        Debug.Log(
                            $"{nameof(NetworkPawn)}/{NetworkObjectId} : {n_role.Value} with ID {NetworkObjectId} has received a flow sync event but will ignore it");
                    break;
                case PawnRole.None:
                default:
                    Debug.LogError(
                        $"{nameof(NetworkPawn)}/{NetworkObjectId} : {n_role.Value} with ID {NetworkObjectId} has received a flow sync event; This is not allowed");
                    throw new ArgumentOutOfRangeException();
            }
        }

        // SYNC-UTILITY

        private void InvokeOnObservers(Action<NetworkPawn> action)
        {
            if (n_role.Value == PawnRole.Controller)
            {
                IEnumerable<NetworkPawn> observers = onlineState.Pawns
                    .Where(it => it.n_role.Value == PawnRole.Observer);
                foreach (NetworkPawn otherPawn in observers)
                    action(otherPawn);
            }

            /*
            switch (pawn.n_role.Value)
            {
                case PawnRole.None:
                    Debug.LogWarning(
                        $"Client with ID {clientId} has an undefined role; Its transition reports will be ignored");
                    break;
                case PawnRole.Observer:
                    // can't request transition
                    Debug.LogWarning(
                        $"Observer with ID {clientId} has reported a transition; It should not do that; The report will be ignored");
                    break;
                case PawnRole.Solo:
                    // pass-thru
                    Debug.LogWarning(
                        $"Solo with ID {clientId} has reported a transition; It should not do that; The report will be ignored");
                    break;
                case PawnRole.Controller:
                    // pass-thru and broadcast to all observers
                    Debug.Log(
                        $"Controller with ID {clientId} has reported a transition; It will be passed on to all observers");
                    foreach (NetworkPawn otherPawn in onlineState.Pawns.Where(it =>
                                 it.n_role.Value == PawnRole.Observer))
                        action(otherPawn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            */
        }

        // SPAWN

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (debug) Debug.Log($"{nameof(NetworkPawn)}/{NetworkObjectId} : Despawn");

            DeSpawning?.Invoke(this);
            if (deSpawningEvent)
                deSpawningEvent.Raise(gameObject);

            _unsubscribeOnDespawn?.Invoke();
        }
    }
}