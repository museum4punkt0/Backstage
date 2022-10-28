using System;
using TMPro;
using UnityEngine;

namespace Atoms.Persistence
{
    public class VariablePersistencePresenter : MonoBehaviour
    {
        [SerializeField] private VariablePersistence persistence;
        [SerializeField] private TMP_Text status;
        [SerializeField] private TMP_Text error;

        private void OnEnable()
        {
            persistence.Saved += UpdateStatus;
            persistence.Applied += UpdateStatus;
            persistence.Error += UpdateError;
        }

        private void OnDisable()
        {
            persistence.Saved -= UpdateStatus;
            persistence.Applied -= UpdateStatus;
            persistence.Error -= UpdateError;
        }

        private void UpdateError(string message)
        {
            UpdateStatus();
            error.text = message;
        }

        private void UpdateStatus()
        {
            status.text = $"Path: {persistence.FilePath}\n" +
                          $"Profile: {persistence.Profile}";
        }
    }
}