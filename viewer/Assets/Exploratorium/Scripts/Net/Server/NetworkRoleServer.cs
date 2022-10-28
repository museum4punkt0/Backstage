/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System;
using Exploratorium.Net.Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Exploratorium.Net.Server
{
    [Serializable]
    public class RolePrefab
    {
        public PawnRole role;
        public GameObject prefab;
    }

    public class NetworkRoleServer : NetworkBehaviour
    {
        [SerializeField] private OnlineStateServer onlineStateServer;
        [SerializeField] private RolePrefab[] rolePrefabs;

        private void Awake()
        {
            Debug.Assert(onlineStateServer != null, $"{nameof(onlineStateServer)} property not assigned");
            Debug.Assert(rolePrefabs != null, $"{nameof(rolePrefabs)} property not assigned");
            Debug.Assert(rolePrefabs.Length == 0, $"{nameof(rolePrefabs)} property is empty");

        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer)
            {
                enabled = false;
                return;
            }
            
        }

        private void OnClientDisconnectCallback(ulong obj)
        {
            throw new NotImplementedException();
        }

        private void OnClientConnected(ulong obj)
        {
            throw new NotImplementedException();
        }
    }
}