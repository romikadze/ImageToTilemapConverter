using UnityEngine;

[RequireComponent(typeof(TilemapDrawer))]
public class EditorTool : MonoBehaviour
{
    private TilemapDrawer _tilemapDrawer;

    public void GenerateTilePalette()
    {
        if (_tilemapDrawer == null)
            _tilemapDrawer = GetComponent<TilemapDrawer>();
            
        _tilemapDrawer.GeneratePalette();
    }

    public void GenerateTilemap()
    {
        if (_tilemapDrawer == null)
            _tilemapDrawer = GetComponent<TilemapDrawer>();
            
        _tilemapDrawer.GenerateTilemap();
    }
}