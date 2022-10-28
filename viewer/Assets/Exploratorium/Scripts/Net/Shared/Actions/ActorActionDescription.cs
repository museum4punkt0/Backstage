/*
 This code is based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop
 LICENSE: https://unity3d.com/legal/licenses/Unity_Companion_License
 */
using UnityEngine;

namespace Exploratorium.Net.Shared.Actions
{
    /// <summary>
    /// Data description of a single Action, including the information to visualize it (animations etc), and the information
    /// to play it back on the server.
    /// </summary>
    [CreateAssetMenu(menuName = "GameData/Pawn Action Description", order = 1)]
    public class PawnActionDescription : ScriptableObject
    {
        [SerializeField] public string DisplayedName;
        [Multiline] public string Description;
        [SerializeField] public ActionType ActionType;

        
    }
}