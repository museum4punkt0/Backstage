/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System;
using Unity.Netcode;
using UnityEngine;

namespace Exploratorium.Net.Shared.Actions
{
    /// <summary>
    /// List of all Actions supported in the game.
    /// </summary>
    public enum ActionType
    {
        None
    }


    /// <summary>
    /// List of all Types of Actions. There is a many-to-one mapping of Actions to ActionLogics.
    /// </summary>
    public enum ActionLogic
    {
        None
    }

    /// <summary>
    /// Comprehensive class that contains information needed to play back any action on the server. This is what gets sent client->server when
    /// the Action gets played, and also what gets sent server->client to broadcast the action event. Note that the OUTCOMES of the action effect
    /// don't ride along with this object when it is broadcast to clients; that information is sync'd separately, usually by NetworkVariables.
    /// </summary>
    public struct ActionRequestData : INetworkSerializable
    {
        public ActionType ActionTypeEnum;
        public Vector3 Position;         
        public Vector3 Direction;        
        public ulong[] TargetIds;        
        public float Amount;             
        public bool ShouldQueue;         

        [Flags]
        private enum PackFlags
        {
            None = 0,
            HasPosition = 1,
            HasDirection = 1 << 1,
            HasTargetIds = 1 << 2,
            HasAmount = 1 << 3,
            ShouldQueue = 1 << 4,
        }

        /// <summary>
        /// Returns true if the ActionRequestDatas are "functionally equivalent" (not including their Queueing or Closing properties).
        /// </summary>
        public bool Compare(ref ActionRequestData rhs)
        {
            bool scalarParamsEqual = (ActionTypeEnum, Position, Direction, Amount) == (rhs.ActionTypeEnum, rhs.Position, rhs.Direction, rhs.Amount);
            if (!scalarParamsEqual) { return false; }

            if (TargetIds == rhs.TargetIds) { return true; } //covers case of both being null.
            if (TargetIds == null || rhs.TargetIds == null || TargetIds.Length != rhs.TargetIds.Length) { return false; }
            for (int i = 0; i < TargetIds.Length; i++)
            {
                if (TargetIds[i] != rhs.TargetIds[i]) { return false; }
            }

            return true;
        }


        private PackFlags GetPackFlags()
        {
            PackFlags flags = PackFlags.None;
            if (Position != Vector3.zero) { flags |= PackFlags.HasPosition; }
            if (Direction != Vector3.zero) { flags |= PackFlags.HasDirection; }
            if (TargetIds != null) { flags |= PackFlags.HasTargetIds; }
            if (Amount != 0) { flags |= PackFlags.HasAmount; }
            if (ShouldQueue) { flags |= PackFlags.ShouldQueue; }


            return flags;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            PackFlags flags = PackFlags.None;
            if (!serializer.IsReader) 
                flags = GetPackFlags();

            serializer.SerializeValue(ref ActionTypeEnum);
            serializer.SerializeValue(ref flags);

            if (serializer.IsReader) 
                ShouldQueue = (flags & PackFlags.ShouldQueue) != 0;
            if ((flags & PackFlags.HasPosition) != 0) 
                serializer.SerializeValue(ref Position);
            if ((flags & PackFlags.HasDirection) != 0) 
                serializer.SerializeValue(ref Direction);
            if ((flags & PackFlags.HasTargetIds) != 0) 
                serializer.SerializeValue(ref TargetIds);
            if ((flags & PackFlags.HasAmount) != 0) 
                serializer.SerializeValue(ref Amount);
            
        }
    }
}
