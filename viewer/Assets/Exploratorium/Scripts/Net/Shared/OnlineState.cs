/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Exploratorium.Net.Client;
using Exploratorium.Net.Server;
using Unity.Netcode;
using UnityEngine;

namespace Exploratorium.Net.Shared
{
    /// <summary>
    /// Represents the network related state and flow of the distributed application while connected to the network.
    /// </summary>
    /// 
    public class OnlineState : NetworkBehaviour
    {
        [SerializeField] private GameObject pawnPresenterPrefab;
        [SerializeField] private Transform pawnPresenterParent;
        private OnlineStateServer _server;
        private OnlineStateClient _client;

        private readonly List<NetworkPawnPresenter> _presenters = new List<NetworkPawnPresenter>();

        private List<NetworkPawn> _pawns = new List<NetworkPawn>();
        public IReadOnlyList<NetworkPawn> Pawns => _pawns;
        public OnlineStateServer Server => _server;
        public OnlineStateClient Client => _client;

        private void Awake()
        {
            Debug.Assert(pawnPresenterPrefab != null, $"{nameof(pawnPresenterPrefab)} property not assigned");
            Debug.Assert(pawnPresenterParent != null, $"{nameof(pawnPresenterParent)} property not assigned");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log($"{nameof(OnlineState)} : Ready");

            _server = GetComponent<OnlineStateServer>();
            _client = GetComponent<OnlineStateClient>();

            // handle existing pawns
            foreach (var no in NetworkManager.SpawnManager.SpawnedObjectsList)
            {
                if (no.TryGetComponent<NetworkPawn>(out var pawn))
                    OnPawnSpawned(pawn);
            }

            NetworkPawn.Spawned += OnPawnSpawned;
            NetworkPawn.DeSpawning += OnPawnDeSpawning;
        }

        public override void OnNetworkDespawn()
        {
            NetworkPawn.Spawned -= OnPawnSpawned;
            NetworkPawn.DeSpawning -= OnPawnDeSpawning;
            foreach (NetworkPawnPresenter presenter in _presenters)
                if (presenter != null)
                    Destroy(presenter);
            _presenters.Clear();
        }

        private void OnPawnSpawned(NetworkPawn pawn)
        {
            var obj = Instantiate(pawnPresenterPrefab, pawnPresenterParent);
            var presenter = obj.GetComponent<NetworkPawnPresenter>();
            presenter.SetPawn(pawn);
            _pawns.Add(pawn);
            Debug.Log($"{nameof(OnlineState)} : pawn {pawn.NetworkObjectId} registered");
            _presenters.Add(presenter);
            Debug.Log($"{nameof(OnlineState)} : presenter created for pawn {pawn.NetworkObjectId}");
        }

        private void OnPawnDeSpawning(NetworkPawn obj)
        {
            Destroy(_presenters.First(it => it.Pawn.NetworkObjectId == obj.NetworkObjectId).gameObject);
        }
    }
}