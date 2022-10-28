using System;
using System.Collections;
using Sirenix.Utilities;
using TMPro;
using UnityAtoms;
using UnityEngine;

[EditorIcon("atom-icon-delicate")]
[RequireComponent(typeof(TMP_Text))]
public sealed class TextObjectBinding : MonoBehaviour
{
    [SerializeField] private AtomBaseVariable variable;
    [SerializeField] private AtomEventBase changedEvent;
    [SerializeField] private TMP_Text text;

    private void Awake()
    {
        if (variable != null)
        {
            if (changedEvent != null)
                changedEvent.OnEventNoValue += OnChangedAdapter;
            else
                Debug.LogWarning(
                    $"{nameof(TextObjectBinding)} : A variable changed event is required to make variable bindings work.");
        }
    }

    private void OnVariableChanged(object value)
    {
        if (text == null)
            return;

        if (value.GetType().IsEnum)
        {
            text.text = $"{Enum.GetName(value.GetType(), value)}";
        }
        else if (value is ICollection c)
        {
            text.text = $"{c.Count} items";
        }
        else
        {
            text.text = value switch
            {
                string x => $"{x}",
                float x => $"{x:F1}",
                int x => $"{x:D}",
                Vector2 x => $"{x}",
                Vector3 x => $"{x}",
                _ => value.GetType().GetNiceName()
            };
        }
    }

    private void OnChangedAdapter() => OnVariableChanged(variable.BaseValue);

    private void OnDestroy()
    {
        if (changedEvent != null)
            changedEvent.OnEventNoValue += OnChangedAdapter;
    }
}