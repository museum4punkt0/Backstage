/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Exploratorium.Net.Shared
{
    public class NetworkName32 : NetworkBehaviour
    {
        [SerializeField]
        NetworkVariable<FixedString32Bytes> m_Name = new NetworkVariable<FixedString32Bytes>("Unnamed");

        public NetworkVariable<FixedString32Bytes> Name => m_Name;
    }
}