
using Assets;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(IsoObj))]
public class Snapper : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        IsoObj thing = (IsoObj)target;
        if (GUILayout.Button("Snap"))
        {
            Snap(thing);
        }
    }

    private static void Snap(IsoObj thing)
    {
        thing.transform.position = IsoUtil.IsoSnapToGridPosition(thing.transform.position);
    }
}