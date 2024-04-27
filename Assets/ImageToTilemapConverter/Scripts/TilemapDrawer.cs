using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDrawer : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Sprite _tileSprite;
    [SerializeField] private string _pathFromResourcesFolder = "";
    [SerializeField] private Converter _converter;
    
    private const string ASSET_EXTENSION = ".asset";
    private const string BASIC_PATH = "Assets/Resources/";

    public void GenerateTilemap()
    {
        Tile[] tileList = Resources.LoadAll<Tile>(_pathFromResourcesFolder);
        Dictionary<Color, Tile> tiles = new Dictionary<Color, Tile>();

        foreach (var tile in tileList)
        {
            tiles.Add(tile.color, tile);
        }
            
        foreach (var data in _converter.TilemapData)
        {
            _tilemap.SetTile(data.Key, tiles[data.Value]);
        }
    }

    public void GeneratePalette()
    {
        for (int i = 0; i < _converter.PaletteData.Count; i++)
        {
            Tile tile = new Tile();
            tile.sprite = _tileSprite;
            tile.color = _converter.PaletteData[i];
            string path = BASIC_PATH + _pathFromResourcesFolder;
            UnityEditor.AssetDatabase.CreateAsset(tile,  path + (i+1) + ASSET_EXTENSION);
        }
    }
}