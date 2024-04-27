using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorTool))]
public class ConverterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorTool _editorTool = (EditorTool)target;
        
        if (GUILayout.Button("Generate Tilemap"))
            _editorTool.GenerateTilemap();
        
        if (GUILayout.Button("Generate Palette"))
            _editorTool.GenerateTilePalette();
    }
}