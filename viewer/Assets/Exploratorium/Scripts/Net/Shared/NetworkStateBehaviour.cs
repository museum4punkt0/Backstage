/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using Unity.Netcode;
using UnityEngine;

namespace Exploratorium.Net.Shared
{
    public enum NetworkState
    {
        Offline,
        Lobby,
        Online
    }
    
    /// <summary>
    /// A special kind of <see cref="NetworkBehaviour"/> that represents a discrete game state. The special feature it offers is
    /// that it provides some guarantees that only one such GameState will be running at a time.
    /// </summary>
    public abstract class NetworkStateBehaviour : NetworkBehaviour
    {
        /// <summary>
        /// Does this <see cref="NetworkStateBehaviour"/> persist across multiple scenes?
        /// </summary>
        public virtual bool Persists => false;

        /// <summary>
        /// What <see cref="NetworkStateBehaviour"/> this represents. Server and client specializations of a state should always return the same enum.
        /// </summary>
        public abstract NetworkState ActiveState { get; }

        /// <summary>
        /// This is the single active <see cref="NetworkStateBehaviour"/> object. There can be only one.
        /// </summary>
        private static GameObject s_activeStateGO;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            if (s_activeStateGO != null)
            {
                if (s_activeStateGO == gameObject)
                {
                    //nothing to do here, if we're already the active state object.
                    return;
                }

                //on the host, this might return either the client or server version, but it doesn't matter which;
                //we are only curious about its type, and its persist state.
                var previousState = s_activeStateGO.GetComponent<NetworkStateBehaviour>();

                if (previousState.Persists && previousState.ActiveState == ActiveState)
                {
                    //we need to make way for the DontDestroyOnLoad state that already exists.
                    Destroy(gameObject);
                    return;
                }

                //otherwise, the old state is going away. Either it wasn't a Persisting state, or it was,
                //but we're a different kind of state. In either case, we're going to be replacing it.
                Destroy(s_activeStateGO);
            }

            s_activeStateGO = gameObject;
            if (Persists)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (!Persists)
            {
                s_activeStateGO = null;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            if (!isActiveAndEnabled)
                return;

            if (NetworkManager.Singleton != null)
                NetworkManager.Singleton.Shutdown();
        }

    }
}



