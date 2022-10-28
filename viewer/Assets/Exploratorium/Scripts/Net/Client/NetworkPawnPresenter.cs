using System;
using Exploratorium.Net.Shared;
using JetBrains.Annotations;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Exploratorium.Net.Client
{
    public class NetworkPawnPresenter : MonoBehaviour
    {
        [SerializeField] [CanBeNull]
        private NetworkPawn pawn;

        [SerializeField] [CanBeNull]
        private TMP_Text pawnName;

        [SerializeField] [CanBeNull]
        private TMP_Text pawnRole;

        [SerializeField] [CanBeNull]
        private TMP_Text clientId;

        [SerializeField] [CanBeNull]
        private CanvasGroup isLocalPlayer;
        
        [SerializeField] [CanBeNull]
        private CanvasGroup isServer;


        public NetworkPawn Pawn => pawn;

        private void Awake()
        {
            Debug.Assert(pawnName != null, $"{nameof(pawnName)} property is unassigned");
            Debug.Assert(pawnRole != null, $"{nameof(pawnRole)} property is unassigned");
            Debug.Assert(clientId != null, $"{nameof(clientId)} property is unassigned");
            Debug.Assert(isLocalPlayer != null, $"{nameof(isLocalPlayer)} property is unassigned");
            Debug.Assert(isServer != null, $"{nameof(isServer)} property is unassigned");
        }

        private void OnEnable()
        {
            if (pawn != null)
                SetPawn(pawn);
        }

        private void OnDisable()
        {
            if (pawn != null)
                pawn.n_name.OnValueChanged -= OnNameChanged;
        }

        private void OnNameChanged(FixedString32Bytes value, FixedString32Bytes newValue) => OnDescriptionChanged();
        private void OnRoleChanged(PawnRole value, PawnRole newValue) => OnDescriptionChanged();

        private void OnDescriptionChanged()
        {
            Debug.Assert(pawn != null, $"{nameof(pawn)} property is unassigned");

            if (pawnName != null)
                pawnName.text = pawn.n_name.Value.ToString();
            if (pawnRole != null)
                pawnRole.text = pawn.n_role.Value.ToString();
            if (clientId != null)
                clientId.text = pawn.OwnerClientId.ToString();
            if (isLocalPlayer != null)
                isLocalPlayer.alpha = pawn.OwnerClientId == NetworkManager.Singleton.LocalClientId ? 1.0f : 0;
            if (isServer != null)
                isServer.alpha = pawn.OwnerClientId == NetworkManager.Singleton.ServerClientId ? 1.0f : 0;
        }

        public void SetPawn([NotNull] NetworkPawn newPawn)
        {
            Debug.Assert(newPawn != null, "newPawn != null");

            // we may have a preexisting subscription
            if (pawn!= null)
            {
                pawn.n_name.OnValueChanged -= OnNameChanged;
                pawn.n_role.OnValueChanged -= OnRoleChanged;
            }

            pawn = newPawn;
            pawn.n_name.OnValueChanged += OnNameChanged;
            pawn.n_role.OnValueChanged += OnRoleChanged;
            OnDescriptionChanged();
        }
    }
}