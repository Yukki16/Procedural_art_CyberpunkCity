using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WFC))]
public class WFCEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WFC wfc = (WFC)target;

        if(GUILayout.Button("Reset Genenartion"))
        {
            if(Application.isPlaying)
            {
                wfc.ResetGrid();
            }
        }
    }
}
