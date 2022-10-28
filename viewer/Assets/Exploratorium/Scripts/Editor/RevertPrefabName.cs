using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class RevertPrefabName
{
    [MenuItem("Prefab/Remove Name Modifications", false, 0)]
    public static void RemoveNameModificationsOnSelected()
    {
        var objects = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable);
        foreach (var o in objects)
        {
            RemoveNameModification(o);
        }
    }

    public static void RemoveNameModification(UnityEngine.Object aObj)
    {
        if (aObj == null)
            return;

        if (!PrefabUtility.IsPartOfAnyPrefab(aObj))
            return;

        var mods = new List<PropertyModification>(UnityEditor.PrefabUtility.GetPropertyModifications(aObj));
        for (int i = 0; i < mods.Count; i++)
        {
            if (mods[i].propertyPath == "m_Name")
                mods.RemoveAt(i);
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(aObj);
        PrefabUtility.SetPropertyModifications(aObj, mods.ToArray());
        EditorUtility.SetDirty(aObj);
    }
}