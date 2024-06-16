using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkyScraper))]
public class HouseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SkyScraper skyScraper = (SkyScraper)target;

        if (GUILayout.Button("Reset Building"))
        {
            if (Application.isPlaying)
            {
                skyScraper.RedoBuilding();
            }
        }
    }
}
