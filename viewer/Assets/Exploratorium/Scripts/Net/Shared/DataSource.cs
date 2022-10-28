/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using System.Collections.Generic;
using Exploratorium.Net.Shared.Actions;
using UnityEngine;

namespace Exploratorium.Net.Shared
{
    public class DataSource : MonoBehaviour
    {
        [Tooltip("All pawn description data should be slotted in here")]
        [SerializeField]
        private PawnDescription[] pawnDescriptions;

        [Tooltip("All pawn action description data should be slotted in here")]
        [SerializeField]
        private PawnActionDescription[] actionDescriptions;

        private Dictionary<PawnRole, PawnDescription> _roleDescriptionMap;
        private Dictionary<ActionType, PawnActionDescription> _actionDescriptionMap;

        /// <summary>
        /// static accessor for all <see cref="DataSource"/>.
        /// </summary>
        public static DataSource Instance { get; private set; }

        /// <summary>
        /// Contents of the CharacterData list, indexed by CharacterType for convenience.
        /// </summary>
        public Dictionary<PawnRole, PawnDescription> RoleDescriptionByType
        {
            get
            {
                if (_roleDescriptionMap != null) 
                    return _roleDescriptionMap;
                
                _roleDescriptionMap = new Dictionary<PawnRole, PawnDescription>();
                foreach (PawnDescription data in pawnDescriptions)
                {
                    if( _roleDescriptionMap.ContainsKey(data.role))
                        throw new System.Exception($"Duplicate character definition detected: {data.role}");
                    
                    _roleDescriptionMap[data.role] = data;
                }
                return _roleDescriptionMap;
            }
        }

        /// <summary>
        /// Contents of the ActionData list, indexed by ActionType for convenience.
        /// </summary>
        public Dictionary<ActionType, PawnActionDescription> ActionDescriptionByType
        {
            get
            {
                if (_actionDescriptionMap != null) 
                    return _actionDescriptionMap;
                
                _actionDescriptionMap = new Dictionary<ActionType, PawnActionDescription>();
                foreach (PawnActionDescription data in actionDescriptions)
                {
                    if (_actionDescriptionMap.ContainsKey(data.ActionType))
                        throw new System.Exception($"Duplicate action definition detected: {data.ActionType}");
                    
                    _actionDescriptionMap[data.ActionType] = data;
                }
                return _actionDescriptionMap;
            }
        }

        private void Awake()
        {
            if (Instance != null)
                throw new System.Exception($"Multiple {nameof(DataSource)}s defined!");

            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
}
