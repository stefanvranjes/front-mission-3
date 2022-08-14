using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

[CustomEditor(typeof(IMP_TIM))]
public class TimImporterEditor : ScriptedImporterEditor
{
    public override void OnInspectorGUI()
    {
        var clutReference = new GUIContent("CLUT");
        var prop1 = serializedObject.FindProperty("clut");
        EditorGUILayout.PropertyField(prop1, clutReference);
        var scale = new GUIContent("Scale");
        var prop2 = serializedObject.FindProperty("scale");
        EditorGUILayout.PropertyField(prop2, scale);
        base.ApplyRevertGUI();
    }
}
