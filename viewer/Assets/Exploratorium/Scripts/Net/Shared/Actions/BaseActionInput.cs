/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System;
using UnityEngine;

namespace Exploratorium.Net.Shared.Actions
{
    public abstract class BaseActionInput : MonoBehaviour
    {
        protected NetworkPawn PawnOwner;
        protected ActionType ActionType;
        protected Action<ActionRequestData> SendInput;
        private System.Action _onFinished;

        public void Initiate(NetworkPawn pawnOwner, ActionType actionType, Action<ActionRequestData> onSendInput, System.Action onFinished)
        {
            PawnOwner = pawnOwner;
            ActionType = actionType;
            SendInput = onSendInput;
            _onFinished = onFinished;
        }

        public void OnDestroy()
        {
            _onFinished();
        }

        public virtual void OnReleaseKey() {}
    }
}
