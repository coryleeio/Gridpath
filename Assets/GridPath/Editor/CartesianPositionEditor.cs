using GridPath.Example;
using UnityEditor;

[CustomEditor(typeof(CartesianPosition))]
public class CartesianPositionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CartesianPosition thing = (CartesianPosition)target;
        DrawDefaultInspector();
        thing.X = EditorGUILayout.IntField("X", thing.X);
        thing.Y = EditorGUILayout.IntField("Y", thing.Y);
    }
}
