/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System.Collections.Generic;
using Exploratorium.Net.Shared;
using UnityEngine;

namespace Exploratorium.Net.Server
{
    public class ClientConfigs
    {
        public readonly Dictionary<ulong, PawnDescription> Configs = new Dictionary<ulong, PawnDescription>();
    }
}